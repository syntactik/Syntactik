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
using System.Text;
using Syntactik.DOM;
using Syntactik.DOM.Mapped;
using Alias = Syntactik.DOM.Mapped.Alias;
using Argument = Syntactik.DOM.Argument;
using Attribute = Syntactik.DOM.Attribute;
using Document = Syntactik.DOM.Mapped.Document;
using Element = Syntactik.DOM.Mapped.Element;
using Parameter = Syntactik.DOM.Mapped.Parameter;
using Scope = Syntactik.DOM.Mapped.Scope;
using ValueType = Syntactik.DOM.Mapped.ValueType;

namespace Syntactik.Compiler.Steps
{
    public class AliasResolvingVisitor : SyntactikDepthFirstVisitor
    {

        private Stack<Alias> _aliasContext;
        private Stack<Scope> _scopeContext;
        protected CompilerContext Context;
        protected Document _currentDocument;
        protected readonly NamespaceResolver NamespaceResolver;

        protected Stack<Alias> AliasContext
        {
            get
            {
                if (_aliasContext != null) return _aliasContext;

                _aliasContext = new Stack<Alias>();
                _aliasContext.Push(null);

                return _aliasContext;
            }
        }

        protected Stack<Scope> ScopeContext
        {
            get
            {
                if (_scopeContext != null) return _scopeContext;

                _scopeContext = new Stack<Scope>();
                _scopeContext.Push(null);
                return _scopeContext;
            }
        }

        public AliasResolvingVisitor(CompilerContext context)
        {
            Context = context;
            NamespaceResolver = (NamespaceResolver)context.Properties["NamespaceResolver"];
        }


        protected string ResolveNodeValue(IMappedPair pair, out ValueType valueType)
        {
            if (pair.ValueType != ValueType.DoubleQuotedString && 
                pair.ValueType != ValueType.Concatenation)
            {
                if (((Pair) pair).PairValue != null)
                {
                    return ResolvePairValue(((Pair)pair).PairValue, out valueType);
                }
                valueType = pair.ValueType;
                return ((Pair) pair).Delimiter != DelimiterEnum.None ? ((Pair) pair).Value : ((Pair) pair).Name;
            }

            return ResolveValueInInterpolation(pair, out valueType);

        }

        protected virtual string ResolveChoiceValue(Pair pair, out ValueType valueType)
        {
            throw new NotImplementedException();
        }

        private string ResolveValueInInterpolation(IMappedPair node, out ValueType valueType)
        {
            var sb = new StringBuilder();
            object previousLine = null;
            int eolCount = 0;
            valueType = node.ValueType; //default value
            if (((IPairWithInterpolation) node).InterpolationItems != null)
            {
                foreach (var item in ((IPairWithInterpolation) node).InterpolationItems)
                {
                    var escapeMatch = item as EscapeMatch;
                    if (escapeMatch != null)
                    {
                        var match = escapeMatch as EolEscapeMatch;
                        if (match != null)
                        {
                            ResolveDqsEol(match, sb, node.ValueIndent, previousLine);
                            eolCount++;
                        }
                        else
                        {
                            if (eolCount == 1) sb.Append(" ");
                            ResolveDqsEscape(escapeMatch, sb);
                            eolCount = 0;
                        }
                        previousLine = item;
                        continue;
                    }

                    if (eolCount == 1) sb.Append(" ");
                    eolCount = 0;
                    var alias = item as Alias;
                    if (alias != null)
                    {
                        sb.Append((string) ResolveValueAlias(alias, out valueType));
                        previousLine = item;
                        continue;
                    }
                    var param = item as Parameter;
                    if (param != null)
                    {
                        sb.Append((string) ResolveValueParameter(param, out valueType));
                        previousLine = item;
                        continue;
                    }
                    var element = item as Element;
                    if (element != null)
                    {
                        sb.Append((string) ResolveNodeValue(element, out valueType));
                        previousLine = item;
                        continue;
                    }

                    sb.Append(item);
                    previousLine = item;
                }
                if (((IPairWithInterpolation)node).InterpolationItems.Count > 1)
                    valueType = node.ValueType; //restore default value if there are more then 1 interp. items
            }
            return sb.ToString();
        }

        protected virtual void ResolveDqsEscape(EscapeMatch escapeMatch, StringBuilder sb)
        {
            char c = ResolveDqsEscapeChar(escapeMatch);
            sb.Append(c);
        }

        protected char ResolveDqsEscapeChar(EscapeMatch escapeMatch)
        {
            var text = escapeMatch.Value;
            if (text[0] == '$') return ResolveDqsEscapeCode(escapeMatch);

            //This is EscSeq
            switch (text[1])
            {   //'$'|'b'|'f'|'n'|'r'|'t'|'v'
                case '"':
                case '\'':
                case '\\':
                case '/':
                case '$':
                    return text[1];
                case 'b':
                    return (char)8;
                case 'f':
                    return (char)0xC;
                case 'n':
                    return (char)0xA;
                case 'r':
                    return (char)0xD;
                case 't':
                    return (char)0x9;
                case 'v':
                    return (char)0xB;
                case 'u':
                    return Convert.ToChar(Convert.ToUInt32(text.Substring(2), 16));

            }

            return (char)0;//should never reach this code if lexer works correctly
        }

        protected char ResolveDqsEscapeCode(EscapeMatch escapeMatch)
        {
            var code = escapeMatch.Value.TrimStart('$', '(', '\t', ' ').TrimEnd(')', '\t', ' ');

            if (code[0] != '#') return (char)int.Parse(code);
            return Convert.ToChar(Convert.ToUInt32(code.Substring(1), 16));
        }

        protected string ResolveValueAlias(Alias alias, out ValueType valueType)
        {
            //var aliasDef = NamespaceResolver.GetAliasDefinition(alias.Name);
            var aliasDef = alias.AliasDefinition;

            AliasContext.Push(alias);
            string result;

            if (aliasDef.ValueType == ValueType.LiteralChoice)
            {
                result = ResolveChoiceValue(alias, out valueType);
            }
            else
                result = aliasDef.PairValue == null ? ResolveNodeValue(aliasDef, out valueType) : ResolvePairValue(aliasDef.PairValue, out valueType);

            AliasContext.Pop();

            return result;
        }


        private string ResolvePairValue(object pairValue, out ValueType valueType)
        {
            var value = pairValue as Parameter;
            if (value != null)
            {
                return ResolveValueParameter(value, out valueType);
            }

            var alias = pairValue as Alias;
            valueType = ValueType.None;
            return alias != null ? ResolveValueAlias(alias, out valueType) : null;
        }

        protected bool ResolveValue(Pair pair)
        {
            var attr = pair as DOM.Attribute;
            if (attr != null && attr.NsPrefix == "xsi" && attr.Name == "type")
            {
                ResolveTypeAttribute(attr);
                return true;
            }

            if (pair.Delimiter == DelimiterEnum.EC)
            {
                ResolveConcatenation(pair);
                return true;                
            }
            //Write element's value
            object value = pair.PairValue as Parameter;
            ValueType valueType;
            if (value != null)
            {
                OnValue(ResolveValueParameter((Parameter)value, out valueType), valueType);
                return true;
            }

            value = pair.PairValue as Alias;
            if (value != null)
            {
                OnValue(ResolveValueAlias((Alias)value, out valueType), valueType);
                return true;
            }
            if (((IMappedPair)pair).IsValueNode)
            {
                OnValue(ResolveNodeValue((IMappedPair)pair, out valueType), valueType);
                return true;
            }

            return false;
        }


        private void ResolveTypeAttribute(Attribute attribute)
        {
            var typeInfo = attribute.Value?.Split(':');
            if (!(typeInfo?.Length > 1)) return;
            var nsPrefix = typeInfo[0];
            NamespaceResolver.GetPrefixAndNs(nsPrefix, attribute, _currentDocument, out var prefix, out var _);
            OnValue($"{prefix}:{typeInfo[1]}", ValueType.FreeOpenString);
        }

        private void ResolveConcatenation(Pair pair)
        {
            var p = pair as IPairWithInterpolation;
            if (p?.InterpolationItems == null || p.InterpolationItems.Count == 0)
            {
                OnValue("", ValueType.Concatenation);
                return;
            }
            var sb = new StringBuilder();
            ValueType resultValueType = ValueType.None;
            foreach (var item in p.InterpolationItems)
            {
                ValueType valueType = ValueType.None;
                object value = item as Parameter;
                if (value != null)
                {
                    sb.Append((string) ResolveValueParameter((Parameter) value, out valueType));
                }
                else
                {
                    value = item as Alias;
                    if (value != null)
                    {
                        sb.Append((string) ResolveValueAlias((Alias)value, out valueType));
                    } else if (((IMappedPair)item).IsValueNode)
                    {
                        sb.Append((string) ResolveNodeValue((IMappedPair)item, out valueType));
                    }
                }
                resultValueType = resultValueType == ValueType.None ? valueType : ValueType.Concatenation;
            }
            if (resultValueType == ValueType.None) resultValueType = ValueType.Concatenation;
            OnValue(sb.ToString(), resultValueType);
        }

        public virtual void OnValue(string value, ValueType type)
        {
        }

        protected string ResolveValueParameter(Parameter parameter, out ValueType valueType)
        {
            var aliasContext = GetAliasContextForParameter(parameter);

            if (parameter.Name == "_")
            {
                //Resolving default value parameter
                if (aliasContext.PairValue != null)
                {
                    return ResolvePairValue(aliasContext.PairValue, out valueType);
                }
                return ResolveNodeValue(aliasContext, out valueType);
            }


            var argument = Enumerable.FirstOrDefault<Argument>(aliasContext.Arguments, a => a.Name == parameter.Name);
            if (argument != null)
            {
                if (argument.PairValue != null)
                {
                    return ResolvePairValue(argument.PairValue, out valueType);
                }
                valueType = ((IMappedPair)argument).ValueType;
                return argument.Value;
            }

            //if argument is not found lookup default value in the Alias Definition
            //var paramDef = aliasContext.AliasDefinition.Parameters.First(p => p.Name == parameter.Name);

            //If parameteres default value is Parameter or Alias then resolve it
            if (parameter.PairValue != null)
            {
                return ResolvePairValue(parameter.PairValue, out valueType);
            }

            valueType = parameter.ValueType;
            return parameter.Value;
        }


        /// <summary>
        /// Go through all entities and resolve attributes for the current node.
        /// </summary>
        /// <param name="entities">List of entities. Looking for alias or parameter because they potentially can hold the attributes.</param>
        protected void ResolveAttributes(IEnumerable<DOM.Entity> entities)
        {
            foreach (var entity in entities)
            {
                if (entity is DOM.Mapped.Attribute)
                {
                    Visit(entity);
                }
                else if (entity is Alias)
                {
                    ResolveAttributesInAlias(entity as Alias);
                }
                else if (entity is Parameter)
                {
                    ResolveAttributesInParameter(entity as Parameter);
                }
            }
        }

        protected void ResolveAttributesInParameter(Parameter parameter)
        {
            var aliasContext = GetAliasContextForParameter(parameter);

            if (aliasContext == null) return;

            var argument = Enumerable.FirstOrDefault<Argument>(aliasContext.Arguments, a => a.Name == parameter.Name);
            ResolveAttributes(argument != null ? argument.Entities : parameter.Entities);
        }

        private Alias GetAliasContextForParameter(Parameter parameter)
        {
            foreach (var context in AliasContext)
            {
                if (context == null) return null;
                if (context.AliasDefinition == parameter.AliasDefinition) return context;
            }
            return null;
        }

        protected void ResolveAttributesInAlias(Alias alias)
        {
            var aliasDef = alias.AliasDefinition;

            //Do not resolve alias without AliasDef or having circular reference
            if (aliasDef == null || aliasDef.HasCircularReference) return;

            AliasContext.Push(alias);
            ResolveAttributes(aliasDef.Entities);
            AliasContext.Pop();
        }


        ///// <summary>
        ///// Resolves EolEscapeMatch token. Returns string with new line and indentation symbols.
        ///// EolEscapeMatch can spread across several lines.
        ///// The first newline symbol is ignored because DQS is "folded" string. 
        ///// If there are only one new line symbol in SQS_EOL then it is ignored.
        ///// </summary>
        ///// <param name="token"></param>
        ///// <param name="valueIndent"></param>
        ///// <returns></returns>
        private static void ResolveDqsEol(EolEscapeMatch escapeMatch, StringBuilder sb, int indent, object previousLine)
        {
            var value = escapeMatch.Value.TrimStart('\r', '\n');
            var previousEol = previousLine as EolEscapeMatch;
            if (previousLine == null) //First line of string
            {
                if (value.Length <= indent) return;
                sb.Append(value.Substring(indent));
                return;
            }
            if (previousEol == null)
            {
                if (value.Length <= indent)
                {
                    return;
                }
                sb.Append(value.Substring(indent));
                return;
            }

            if (value.Length <= indent)
            {
                sb.AppendLine();
                return;
            }
            sb.AppendLine();
            sb.Append(value.Substring(indent));
            return;
        }



        public override void OnScope(DOM.Scope pair)
        {
            ScopeContext.Push((Scope) pair);
            base.OnScope(pair);
            ScopeContext.Pop();
        }

        public override void OnElement(DOM.Element pair)
        {
            ResolveAttributes(pair.Entities);
            Visit(pair.Entities.Where(e => !(e is DOM.Attribute)));
        }

        public override void OnAlias(DOM.Alias alias)
        {
            var aliasDef = ((Alias)alias).AliasDefinition;

            //Do not resolve alias without AliasDef or having circular reference
            if (aliasDef == null || aliasDef.HasCircularReference) return;

            AliasContext.Push((Alias) alias);
            Visit(aliasDef.Entities.Where(e => !(e is DOM.Attribute)));
            AliasContext.Pop();
        }

        public override void OnParameter(DOM.Parameter parameter)
        {
            var aliasContext = GetAliasContextForParameter((Parameter) parameter);

            if (aliasContext == null) return;

            if (parameter.Name == "_") //Default parameter. Value is passed in the body of the alias
            {
                Visit(Enumerable.Where<Entity>(aliasContext.Entities, e => !(e is DOM.Attribute) && !(e is DOM.Comment)));
                return;
            }

            var argument = Enumerable.FirstOrDefault<Argument>(aliasContext.Arguments, a => a.Name == parameter.Name);

            Visit(argument?.Entities.Where(e => !(e is DOM.Attribute)) ?? parameter.Entities.Where(e => !(e is DOM.Attribute)));
        }

        public override void OnAliasDefinition(DOM.AliasDefinition node)
        {
            //Doing nothing for Alias Definition        
        }
    }
}
