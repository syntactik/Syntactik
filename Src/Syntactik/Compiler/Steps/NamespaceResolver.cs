#region license
// Copyright © 2017 Maxim O. Trushin (trushin@gmail.com)
//
// This file is part of Syntactik.
// Syntactik is free software: you can redistribute it and/or modify
// it under the terms of the GNU Lesser General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// Syntactik is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU Lesser General Public License for more details.

// You should have received a copy of the GNU Lesser General Public License
// along with Syntactik.  If not, see <http://www.gnu.org/licenses/>.
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Syntactik.DOM;
using Syntactik.DOM.Mapped;
using Alias = Syntactik.DOM.Mapped.Alias;
using AliasDefinition = Syntactik.DOM.Mapped.AliasDefinition;
using Argument = Syntactik.DOM.Mapped.Argument;
using Document = Syntactik.DOM.Mapped.Document;
using Module = Syntactik.DOM.Mapped.Module;
using NamespaceDefinition = Syntactik.DOM.NamespaceDefinition;
using Parameter = Syntactik.DOM.Mapped.Parameter;
using Scope = Syntactik.DOM.Mapped.Scope;

namespace Syntactik.Compiler.Steps
{
    /// <summary>
    /// Implements logic of collecting information about aliases and namespaces.
    /// </summary>
    public class NamespaceResolver
    {
        private List<NsInfo> _moduleMembersNsInfo;
        private ModuleMember _currentModuleMember;
        private DOM.Module _currentModule;
        private readonly CompilerContext _context;
        private readonly Stack<NsInfo> _aliasDefStack;
        private readonly long _syncTime;

        /// <summary>
        /// Creates an instance of <see cref="NamespaceResolver"/>.
        /// </summary>
        /// <param name="context"><see cref="CompilerContext"/> is used to report errors.</param>
        public NamespaceResolver(CompilerContext context)
        {
            _context = context;
            _aliasDefStack = new Stack<NsInfo>();
            _syncTime = DateTime.Now.Ticks;
        }

        /// <summary>
        /// NsInfo for all processed module members (Documents and AliasDef).
        /// </summary>
        public List<NsInfo> ModuleMembersNsInfo => _moduleMembersNsInfo ?? (_moduleMembersNsInfo = new List<NsInfo>());

        /// <summary>
        /// Resolves aliases, namespaces and do checks after the all nodes are visited.
        /// </summary>
        public void ResolveAliasesAndDoChecks()
        {
            foreach (var nsInfo in ModuleMembersNsInfo)
            {
                CheckModuleMember(nsInfo);

                ResolveAliasesInModuleMember(nsInfo);
            }
        }

        private void ResolveAliasesInModuleMember(NsInfo nsInfo)
        {
            foreach (var alias in nsInfo.Aliases)
            {
                NsInfo aliasNsInfo = ResolveAliasInModuleMember((Alias) alias, nsInfo);
                if (aliasNsInfo == null) continue;
                MergeNsInfo(nsInfo, aliasNsInfo);
            }
        }

        internal void ProcessParameter(Parameter node)
        {
            if (_currentModuleMember is Document)
            {
                _context.AddError(CompilerErrorFactory.ParametersCantBeDeclaredInDocuments(node, _currentModule.FileName));
            }
            else
            {
                var aliasDef = (AliasDefinition) _currentModuleMember;
                if (aliasDef.SyncTime < _syncTime)
                {
                    aliasDef.SyncTime = _syncTime;
                    aliasDef.Parameters.Clear();
                }
                aliasDef.Parameters.Add(node);
                node.AliasDefinition = (AliasDefinition) _currentModuleMember;
                if (node.Name == "_")
                {
                    if (!node.IsValueNode)
                        ((AliasDefinition) _currentModuleMember).HasDefaultBlockParameter = true;
                    else
                        ((AliasDefinition) _currentModuleMember).HasDefaultValueParameter = true;
                }
            }
        }

        /// <summary>
        /// Finds <see cref="NsInfo"/> related to the <see cref="ModuleMember"/>.
        /// </summary>
        /// <param name="moduleMember">Target <see cref="ModuleMember"/>.</param>
        /// <returns></returns>
        public NsInfo GetNsInfo(ModuleMember moduleMember)
        {
            return ModuleMembersNsInfo.FirstOrDefault(n => n.ModuleMember == moduleMember);
        }

        private void CheckModuleMember(NsInfo nsInfo)
        {
            if (!(nsInfo.ModuleMember is Document))
            {
                ValidateAliasDefDefaultParameter((AliasDefinition) nsInfo.ModuleMember);
                return;
            }
        }

        private void ValidateAliasDefDefaultParameter(AliasDefinition aliasDef)
        {
            if (!aliasDef.HasDefaultBlockParameter && !aliasDef.HasDefaultValueParameter) return;

            var hasNonDefaultParameter = aliasDef.Parameters.Any(p => p.Name != "_");
            if (!hasNonDefaultParameter) return;

            foreach (var param in aliasDef.Parameters)
            {
                if (param.Name == "_")
                {
                    _context.AddError(CompilerErrorFactory.DefaultParameterMustBeOnly(param, aliasDef.Module.FileName));
                }
            }
        }

        /// <summary>
        /// Gets <see cref="AliasDefinition"/> by name.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public AliasDefinition GetAliasDefinition(string name)
        {
            NsInfo resultInfo =
                ModuleMembersNsInfo.FirstOrDefault(a => (a.ModuleMember is DOM.AliasDefinition) && a.ModuleMember.Name == name);
            return (AliasDefinition) resultInfo?.ModuleMember;
        }

        private void MergeNsInfo(NsInfo destNsInfo, NsInfo nsInfo)
        {
            foreach (var ns in nsInfo.Namespaces)
            {
                var uri = ns.Value;
                var destNs = destNsInfo.Namespaces.FirstOrDefault(n => n.Value == uri);

                if (destNs != null) continue;

                var prefix = FindFreePrefix(ns.Name, destNsInfo.Namespaces);

                destNsInfo.Namespaces.Add(new NamespaceDefinition(prefix, AssignmentEnum.E, ns.Value));
            }
        }

        private string FindFreePrefix(string name, List<DOM.NamespaceDefinition> namespaces)
        {
            var i = 1;
            while (namespaces.Any(n => n.Name == name))
            {
                name = name + i++;
            }

            return name;
        }

        private NsInfo ResolveAliasInModuleMember(Alias alias, NsInfo memberNsInfo)
        {
            //Finding AliasDef
            var aliasDef = LookupAliasDef(alias);

            if (aliasDef == null)
            {
                //Report Error if alias is not defined
                //_context.AddError(CompilerErrorFactory.AliasIsNotDefined(alias, memberNsInfo.ModuleMember.Module.FileName));
                return null;
            }

            if (aliasDef.IsValueNode != alias.IsValueNode && alias.IsValueNode)
            {
                _context.AddError(CompilerErrorFactory.CantUseBlockAliasAsValue(alias,
                    memberNsInfo.ModuleMember.Module.FileName));
            }
            return ResolveAliasesInAliasDefinition(aliasDef);
        }

        private AliasDefinition LookupAliasDef(Alias alias)
        {
            if (alias.AliasDefinition != null && _syncTime <= alias.AliasDefinition.SyncTime)
                return alias.AliasDefinition;

            var result = GetAliasDefinition(alias.Name);
            if (result != null) result.SyncTime = _syncTime;
            alias.AliasDefinition = result;
            return result;
        }

        private NsInfo ResolveAliasesInAliasDefinition(DOM.AliasDefinition aliasDef)
        {
            _currentModuleMember = aliasDef;
            var aliasDefNsInfo = CurrentModuleMemberNsInfo;

            if (aliasDefNsInfo.AliasesResolved) return aliasDefNsInfo;

            return ResolveAliasesInAliasDefinition(aliasDefNsInfo);
        }

        private NsInfo ResolveAliasesInAliasDefinition(NsInfo aliasDefNsInfo)
        {
            //Check if Alias is already being resolved (circular reference)

            if (_aliasDefStack.Any(n => n == aliasDefNsInfo))
            {
                //Report Error
                foreach (var info in _aliasDefStack)
                {
                    _context.AddError(CompilerErrorFactory.AliasDefHasCircularReference(info));
                    ((AliasDefinition) info.ModuleMember).HasCircularReference = true;
                    if (info == aliasDefNsInfo) break;
                }
                return aliasDefNsInfo;
            }

            _aliasDefStack.Push(aliasDefNsInfo);

            foreach (var alias in aliasDefNsInfo.Aliases)
            {
                NsInfo aliasNsInfo = ResolveAliasInAliasDefinition(alias);
                if (aliasNsInfo == null) continue;
                MergeNsInfo(aliasDefNsInfo, aliasNsInfo);
            }

            _aliasDefStack.Pop();

            aliasDefNsInfo.AliasesResolved = true;

            return aliasDefNsInfo;
        }

        private NsInfo ResolveAliasInAliasDefinition(DOM.Alias alias)
        {
            //Finding AliasDef
            var aliasDef = GetAliasDefinition(alias.Name);
            if (aliasDef == null)
            {
                //Report Error
                //_context.AddError(CompilerErrorFactory.AliasIsNotDefined((Alias)alias, aliasDefNsInfo.ModuleMember.Module.FileName));
                return null;
            }
            return ResolveAliasesInAliasDefinition(aliasDef);
        }

        private NsInfo _currentModuleMemberNsInfo;

        private NsInfo CurrentModuleMemberNsInfo
        {
            get
            {
                if (_currentModuleMemberNsInfo != null &&
                    _currentModuleMemberNsInfo.ModuleMember == _currentModuleMember) return _currentModuleMemberNsInfo;

                var result = ModuleMembersNsInfo.FirstOrDefault(n => n.ModuleMember == _currentModuleMember);
                _currentModuleMemberNsInfo = result;
                return result;
            }
        }

        /// <summary>
        /// Tells <see cref="NamespaceResolver"/> to check validity of <see cref="Alias"/> and add it
        /// to the list of aliases of the current <see cref="ModuleMember"/>.
        /// Method should be called from the visitor.
        /// </summary>
        /// <param name="alias">The alias to be added.</param>
        public void ProcessAlias(Alias alias)
        {
            CheckDuplicateArguments(alias);

            // Adds Alias to the Namespace Info of the current Module Member.
            CurrentModuleMemberNsInfo.Aliases.Add(alias);
        }

        private void CheckDuplicateArguments(Alias alias)
        {
            var dups = alias.Arguments.GroupBy(a => a.Name).Where(g => g.Count() > 1).SelectMany(g => g).ToList();
            dups.ForEach(
                a =>
                    _context.AddError(CompilerErrorFactory.DuplicateArgumentName((Argument) a, _currentModule.FileName)));
        }

        /// <summary>
        /// Sets current <see cref="ModuleMember"/>.
        /// Method should be called from the visitor.
        /// </summary>
        /// <param name="document">Current <see cref="Document"/>.</param>
        public void EnterDocument(Document document)
        {
            _currentModuleMember = document;
            _currentModuleMemberNsInfo = new NsInfo(_currentModuleMember);
            ModuleMembersNsInfo.Add(_currentModuleMemberNsInfo);
        }

        /// <summary>
        /// Sets current <see cref="ModuleMember"/>.
        /// Method should be called from the visitor.
        /// </summary>
        /// <param name="aliasDefinition">Current <see cref="AliasDefinition"/>.</param>
        public void EnterAliasDef(AliasDefinition aliasDefinition)
        {
            _currentModuleMember = aliasDefinition;
            _currentModuleMemberNsInfo = new NsInfo(_currentModuleMember);
            ModuleMembersNsInfo.Add(_currentModuleMemberNsInfo);
        }


        /// <summary>
        /// Sets current <see cref="Module"/>.
        /// Method should be called from the visitor.
        /// </summary>
        /// <param name="module">Current <see cref="Module"/>.</param>
        public void EnterModule(DOM.Module module)
        {
            _currentModule = module;
            _currentModuleMember = null;
        }

        /// <summary>
        /// For the Namespace Prefix of the Node:
        /// - reports error if it is not defined
        /// - finds Namespace DOM object in the Module or ModuleMember and adds it to NsINFo of the current ModuleMember
        /// if the namespace is not added yet
        /// </summary>
        internal void ProcessNsPrefix(IMappedPair pair)
        {
            var nsPrefix = ((INsNode) pair).NsPrefix;

            if (string.IsNullOrEmpty(nsPrefix) ||
                nsPrefix.StartsWith("xml", StringComparison.OrdinalIgnoreCase)) return;

            var ns = LookupNamespace(nsPrefix);
            if (ns == null)
            {
                _context.AddError(CompilerErrorFactory.NsPrefixNotDefined(pair.NameInterval, nsPrefix, _currentModule.FileName));
                return;
            }

            var foundNs = CurrentModuleMemberNsInfo.Namespaces.FirstOrDefault(n => n.Value == ns.Value);

            if (foundNs == null)
                CurrentModuleMemberNsInfo.Namespaces.Add(ns);
            else
                //If namespace is already defined with different prefix then changing prefix on pair
            if (foundNs.Name != nsPrefix) ((INsNodeOverridable) pair).OverrideNsPrefix(foundNs.Name);

            if (!(pair is DOM.Mapped.Attribute attribute) || attribute.Name != "type" || nsPrefix != "xsi") return;
            
            var typeInfo = attribute.Value?.Split(':');
            if (!(typeInfo?.Length > 1)) return;
            
            nsPrefix = typeInfo[0];
            ns = LookupNamespace(nsPrefix);
            if (ns == null)
            {
                _context.AddError(CompilerErrorFactory.NsPrefixNotDefined(pair.ValueInterval, nsPrefix, _currentModule.FileName));
                return;
            }
            foundNs = CurrentModuleMemberNsInfo.Namespaces.FirstOrDefault(n => n.Value == ns.Value);

            if (foundNs == null)
                CurrentModuleMemberNsInfo.Namespaces.Add(ns);
            else if (foundNs.Name != nsPrefix)
            {
                attribute.OverrideValue($"{foundNs.Name}:{typeInfo[1]}");
            }
            
        }

        /// <summary>
        /// Looks up Namespace with the specified Prefix in the current ModuleMember first and then in the Module
        /// </summary>
        /// <param name="nsPrefix"></param>
        /// <returns></returns>
        private NamespaceDefinition LookupNamespace(string nsPrefix)
        {
            NamespaceDefinition ns;
            //Looking up in the ModuleMember (Document/AliasDef)
            if ((ns = _currentModuleMember.NamespaceDefinitions.FirstOrDefault(n => n.Name == nsPrefix)) != null)
                return ns;

            //Looking up in the Module
            if ((ns = _currentModule.NamespaceDefinitions.FirstOrDefault(n => n.Name == nsPrefix)) != null)
            {
                //Checking if this namespace can be replaced by namespace from ModuleMember because it has same URI
                NamespaceDefinition ns2;
                if ((ns2 = _currentModuleMember.NamespaceDefinitions.FirstOrDefault(n => n.Value == ns.Value)) != null)
                    return ns2;

                return ns;
            }

            return null;
        }

        /// <summary>
        /// Resolves namespace and prefix for <see cref="INsNode"/>.
        /// </summary>
        /// <param name="node">Target <see cref="INsNode"/>.</param>
        /// <param name="document">Current <see cref="Document"/>.</param>
        /// <param name="scope">Current <see cref="Scope"/>.</param>
        /// <param name="prefix">Output namespace prefix parameter.</param>
        /// <param name="ns">Output namespace parameter.</param>
        public void GetPrefixAndNs(INsNode node, DOM.Document document, Scope scope, out string prefix,
                out string ns)
        {
            prefix = null;
            ns = null;

            var nsPrefix = node.NsPrefix;
            if (nsPrefix == null)
            {
                if (scope == null) return;
                nsPrefix = scope.NsPrefix;
            }
            GetPrefixAndNs(nsPrefix, node, document, out prefix, out ns);
        }

        /// <summary>
        /// Resolves namespace and prefix for <see cref="INsNode"/>.
        /// </summary>
        /// <param name="nsPrefix">Effective namespace prefix of the node. Could be different than the actual prefix.</param>
        /// <param name="node">Target <see cref="INsNode"/>.</param>
        /// <param name="document">Current <see cref="Document"/>.</param>
        /// <param name="prefix">Output namespace prefix parameter.</param>
        /// <param name="ns">Output namespace parameter.</param>
        public void GetPrefixAndNs(string nsPrefix, INsNode node, DOM.Document document, out string prefix,
                out string ns)
        {
            prefix = null;
            ns = null;

            //Getting namespace info for the generated document.
            var targetNsInfo = ModuleMembersNsInfo.First(n => n.ModuleMember == document);
            var moduleMember = GetModuleMember((Pair)node);
            if (moduleMember is ModuleMember member)
            {
                //Resolving namespace first using aliasDef context NsInfo
                var contextNsInfo = ModuleMembersNsInfo.First(n => n.ModuleMember == moduleMember);
                var domNamespace = contextNsInfo.Namespaces.FirstOrDefault(n => n.Name == nsPrefix);

                if (domNamespace == null)
                {
                    //Prefix was defined in the module. Looking up in the module.
                    var moduleNamespace = member.Module.NamespaceDefinitions.FirstOrDefault(n => n.Name == nsPrefix);
                    if (moduleNamespace != null)
                        ns = moduleNamespace.Value;
                }
                else
                {
                    ns = domNamespace.Value;
                }
                //Resolving prefix using Document's NsInfo
                if (ns != null)
                {
                    var ns1 = ns;
                    prefix = targetNsInfo.Namespaces.First(n => n.Value == ns1).Name;
                }
            }
        }

        private Pair GetModuleMember(Pair node)
        {
            while (!(node.Parent is ModuleMember) && !(node.Parent is Module)) node = node.Parent;
            return (ModuleMember) node.Parent;
        }
    }
}