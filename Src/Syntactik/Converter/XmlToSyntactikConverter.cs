using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml;

namespace Syntactik.Converter
{
    internal class ElementInfo
    {
        public int BlockCounter;
        public string DefaultNamespace; // Used to track default ns declarations
        public ListDictionary NsDeclarations; // Used to track ns declarations
    }
    /// <summary>
    /// 
    /// </summary>
    public class XmlToSyntactikConverter
    {
        //private readonly bool _validate;
        private readonly bool _withNamespaces;
        private readonly string _text;
        private StringBuilder _sb;
        private StringBuilder _value;
        private bool _newLine = true;
        private string _indent;
        private int _currentIndent = -1;
        private Stack<ElementInfo> _elementStack;
        private char _indentChar;
        private int _indentMultiplicity;
        private ListDictionary _declaredNamespaces;
        private bool _inElement;
        /// <summary>
        /// Converts xml to syntactik format
        /// </summary>
        /// <param name="text">xml to convert.</param>
        /// <param name="validate">If true will validate as the whole xml document, otherwise can accept any xml fragment.</param>
        /// <param name="withNamespaces">If true then output will include namespace declarations.</param>
        public XmlToSyntactikConverter(string text, bool validate = true, bool withNamespaces = false)
        {
            //_validate = validate;
            _withNamespaces = withNamespaces;
            _text = !validate ? Regex.Replace(text, @"<\?[^?]*(?:\?[^>]+)*\?+>(?:\r|\n)*", "") : text;
        }

        private string DefaultNamespace
        {
            get
            {
                if (_elementStack == null || _elementStack.Count == 0) return null;
                return _elementStack.Peek().DefaultNamespace;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="indent"></param>
        /// <param name="indentChar"></param>
        /// <param name="indentMultiplicity"></param>
        /// <param name="insertNewLine"></param>
        /// <param name="declaredNamespaces"></param>
        /// <param name="s4x"></param>
        /// <returns></returns>
        public bool Convert(int indent, char indentChar, int indentMultiplicity, bool insertNewLine, ListDictionary declaredNamespaces, out string s4x)
        {
            _indentChar = indentChar;
            _indentMultiplicity = indentMultiplicity;

            _indent = new string(indentChar, indent * indentMultiplicity);
            var header = new StringBuilder();
            _sb = new StringBuilder();
            _elementStack = new Stack<ElementInfo>();
            _declaredNamespaces = declaredNamespaces;
            if (insertNewLine)
            {
                _sb.AppendLine("");
                _sb.Append(_indent);
            }
            using (var stringReader = new StringReader(_text))
            using (var xmlReader = XmlReader.Create(stringReader, 
                new XmlReaderSettings(){DtdProcessing = DtdProcessing.Ignore, IgnoreWhitespace = true, CheckCharacters = true}))
            {
                //if (!_validate)
                //{
                //    xmlReader.Namespaces = false;
                //    xmlReader.DtdProcessing = DtdProcessing.Ignore;
                //}
                try
                {
                    while (xmlReader.Read())
                    {
                        switch (xmlReader.NodeType)
                        {
                            case XmlNodeType.Element:
                                _currentIndent++;
                                StartWithNewLine();

                                if (_value != null)
                                {
                                    _inElement = false;
                                    WriteValue(_value.ToString());
                                    _value = null;
                                    StartWithNewLine();
                                }


                                _elementStack.Push(new ElementInfo { BlockCounter = 0, DefaultNamespace = DefaultNamespace });
                                GetNsAndName(xmlReader.Name, out var ns, out var name);
                                List<Tuple<string, string, string>> attributes = null;
                                string defaultNsPrefix = null;
                                var isEmptyElement = xmlReader.IsEmptyElement;
                                if (xmlReader.HasAttributes) attributes = ProcessAttributes(xmlReader, out defaultNsPrefix);
                                if (ns != null)
                                {
                                    var resolvedNs = ResolveNsPrefix(ns);
                                    if (resolvedNs == defaultNsPrefix)
                                    {
                                        _sb.Append("#");
                                    }
                                    else
                                    {
                                        if (defaultNsPrefix != null)
                                        {
                                            _sb.Append("#");
                                            _sb.Append(defaultNsPrefix);
                                            _sb.Append(":");
                                        }
                                    }
                                    _sb.Append(resolvedNs);
                                    _sb.Append(".");
                                }
                                else if (defaultNsPrefix != null)
                                {
                                    _sb.Append("#");
                                    _sb.Append(defaultNsPrefix);
                                    _sb.Append(".");
                                }
                                _sb.Append(name);
                                _value = null;
                                _newLine = false;
                                _inElement = true;
                                WriteAttributes(attributes);
                                if (isEmptyElement)
                                    ProcessEndElement();
                                break;
                            case XmlNodeType.EndElement:
                                ProcessEndElement();
                                break;
                            case XmlNodeType.Text:
                                if (_inElement)
                                {
                                    if (_value == null) _value = new StringBuilder();
                                    _value.Append(xmlReader.Value);
                                }
                                else
                                {
                                    _currentIndent++;
                                    StartWithNewLine();
                                    WriteValue(xmlReader.Value);
                                    _currentIndent--;
                                    IncreaseBlockCounter();
                                }
                                break;
                            case XmlNodeType.CDATA:
                                if (_value == null) _value = new StringBuilder();
                                _value.Append(xmlReader.Value);
                                break;
                            case XmlNodeType.ProcessingInstruction:
                                header.Append(@"'''");
                                header.Append("ProcessingInstruction ");
                                header.Append(xmlReader.Name);
                                header.Append(": ");
                                header.AppendLine(xmlReader.Value);
                                break;
                            case XmlNodeType.Comment:
                                if (!_newLine)
                                {
                                    _sb.AppendLine();
                                    _sb.Append(_indent);
                                    _sb.Append(_indentChar, (_currentIndent + 1) * _indentMultiplicity);
                                    _newLine = true;
                                }
                                if (xmlReader.Value.Contains("\n"))
                                {
                                    _sb.Append("\"\"\"");
                                    _sb.Append(xmlReader.Value.Replace("\"\"\"", "\"\" \" "));
                                    _sb.AppendLine("\"\"\"");
                                }
                                else
                                {
                                    _sb.Append("'''");
                                    _sb.Append(xmlReader.Value);
                                }
                                _newLine = false;
                                break;
                            case XmlNodeType.XmlDeclaration:
                                header.Append(@"'''");
                                header.Append("XmlDeclaration: ");
                                header.AppendLine(xmlReader.Value);
                                break;
                            case XmlNodeType.Document:
                                break;
                            case XmlNodeType.DocumentType:

                                break;
                            case XmlNodeType.EntityReference:

                                break;
                        }
                    }
                }
                catch (Exception)
                {
                    s4x = header + (_withNamespaces ? GetNamespaceDeclarations(_declaredNamespaces) + _sb : _sb.ToString());
                    return false;
                }
            }
            s4x = header + (_withNamespaces ? GetNamespaceDeclarations(_declaredNamespaces) + _sb : _sb.ToString());
            return true;
        }

        private void ProcessEndElement()
        {
            if (_value != null)
            {
                WriteValue(_value.ToString());
                _value = null;
            }
            _currentIndent--;
            if (_elementStack.Count > 0) _elementStack.Pop();
            IncreaseBlockCounter();
            _inElement = false;
        }

        private static string GetNamespaceDeclarations(ListDictionary declaredNamespaces)
        {
            var sb = new StringBuilder();
            foreach (DictionaryEntry ns in declaredNamespaces)
            {
                sb.Append("!#");
                sb.Append(ns.Key);
                sb.Append(" = ");
                sb.AppendLine(ns.Value.ToString());
            }
            return sb.ToString();
        }

        private void WriteAttributes(List<Tuple<string, string, string>> attributes)
        {
            if (attributes == null) return;
            _currentIndent++;
            foreach (var tuple in attributes)
            {
                StartWithNewLine();
                _sb.Append('@');
                if (tuple.Item1 != null) //namespace prefix
                {
                    _sb.Append(tuple.Item1);
                    _sb.Append(".");
                }
                _sb.Append(tuple.Item2); // name
                _newLine = false;
                IncreaseBlockCounter();
                if (!string.IsNullOrEmpty(tuple.Item3)) WriteValue(tuple.Item3);
            }
            _currentIndent--;
            _inElement = false;
        }

        private string ResolveNsPrefix(string ns)
        {
            if (ns == string.Empty) return ns;
            var @namespace = GetNamespace(ns);

            if (@namespace == null) return ns;
            foreach (var declaredNamespace in _declaredNamespaces)
            {
                var entry = (DictionaryEntry)declaredNamespace;
                if (entry.Value.ToString() == @namespace) return (string)entry.Key;
            }
            return ns;
        }

        private string LookupOrCreatePrefix(string @namespace)
        {
            string ns = null;
            foreach (var declaredNamespace in _declaredNamespaces)
            {
                var entry = (DictionaryEntry)declaredNamespace;
                if (entry.Value.ToString() == @namespace)
                {
                    ns = (string)entry.Key;
                }
            }
            if (ns != null) return ns;
            var i = 1;
            while (true)
            {
                var newNs = "ns" + i;
                if (GetDeclaredNamespace(newNs) == null)
                {
                    _declaredNamespaces[newNs] = @namespace;
                    return newNs;
                }
                i++;
            }
        }

        private string GetDeclaredNamespace(string prefix)
        {
            foreach (var declaredNamespace in _declaredNamespaces)
            {
                var entry = (DictionaryEntry)declaredNamespace;
                if (entry.Key.ToString() == prefix)
                {
                    return (string)entry.Value;
                }
            }
            return null;
        }

        private string GetNamespace(string ns)
        {
            foreach (var elementInfo in _elementStack)
            {
                if (elementInfo.NsDeclarations == null) continue;
                foreach (var item in elementInfo.NsDeclarations)
                {
                    var entry = (DictionaryEntry)item;
                    if (entry.Key.ToString() == ns) return (string)entry.Value;
                }
            }
            return null;
        }

        private void WriteValue(string s)
        {
            _newLine = false;
            var conv = EncodeValue(s, out var escapeSymbolsFound, out var mapping);

            if (_inElement) _sb.Append(" ");

            if (escapeSymbolsFound)
            {
                _sb.Append("= \"");
                WriteValue(conv, 1, s, mapping);
                _sb.Append("\"");
            }
            else if (s.StartsWith("\"") || s.StartsWith("'") || s.StartsWith(" ") || s.EndsWith(" "))
            {
                _sb.Append("= \'");
                WriteValue(conv, 2, s, mapping);
                _sb.Append("'");
            }
            else
            {
                _sb.Append("= ");
                WriteValue(conv, 0, s, mapping);
            }
        }

        private void WriteValue(List<string> list, int quoteType, string original, List<Tuple<int, int>> mapping)
        {
            if (list.Count == 0) return;
            var i = 0;
            do
            {
                if (i > 0)
                {
                    _sb.AppendLine();
                    if (quoteType == 1 && !string.IsNullOrEmpty(list[i - 1])) _sb.AppendLine();
                    _sb.Append(_indent);
                    _sb.Append(_indentChar, (_currentIndent + 1) * _indentMultiplicity);
                }
                _sb.Append(quoteType == 1
                    ? list[i]
                    : (mapping[i].Item2 - mapping[i].Item1 + 1) > 0 ? original.Substring(mapping[i].Item1, mapping[i].Item2 - mapping[i].Item1 + 1) : "");
                i++;
            } while (i < list.Count);

        }

        private List<Tuple<string, string, string>> ProcessAttributes(XmlReader xmlReader, out string defaultNsPrefix)
        {
            defaultNsPrefix = null;
            xmlReader.MoveToFirstAttribute();
            List<Tuple<string, string, string>> attributes = new List<Tuple<string, string, string>>();
            do
            {
                GetNsAndName(xmlReader.Name, out var ns, out var name);
                if (ns == "xsi" || ns == "xml")
                {
                    //Process xsi and xml attributes
                    var value = xmlReader.Value;
                    if (ns == "xsi" && name == "type")
                    {
                        var s = value.Split(new[] { ':', }, StringSplitOptions.RemoveEmptyEntries);
                        if (s.Length == 2)
                        {
                            var prefix = ResolveNsPrefix(s[0]);
                            value = $"{prefix}:{s[1]}";
                        }
                    }

                    if (attributes.Count > 0)
                        attributes.Insert(0, new Tuple<string, string, string>(ns, name, value));
                    else
                        attributes.Add(new Tuple<string, string, string>(ns, name, value));
                }
                else if (ns != "xmlns" && !(string.IsNullOrEmpty(ns) && name == "xmlns"))
                {
                    //process regular attributes
                    if (!string.IsNullOrEmpty(ns))
                    {
                        ns = ResolveNsPrefix(ns);
                    }
                    attributes.Add(new Tuple<string, string, string>(ns, name, xmlReader.Value));
                }
                else
                {
                    //Process namespace declaration
                    ProcessNsAttribute(ns, name, xmlReader.Value, out defaultNsPrefix);
                }
            } while (xmlReader.MoveToNextAttribute());
            return attributes;
        }

        private void ProcessNsAttribute(string ns, string name, string value, out string defaultNsPrefix)
        {
            defaultNsPrefix = null;
            if (string.IsNullOrEmpty(ns) && name == "xmlns")
            {
                _elementStack.Peek().DefaultNamespace = value;
                defaultNsPrefix = LookupOrCreatePrefix(value);
                return;
            }
            if (ns == "xmlns" && !string.IsNullOrEmpty(name) && !string.IsNullOrEmpty(value))
            {
                //if (name == "xsi") return; //ignoring xsi namespace declaration
                var elementInfo = _elementStack.Peek();
                if (elementInfo.NsDeclarations == null) elementInfo.NsDeclarations = new ListDictionary();
                elementInfo.NsDeclarations[name] = value;
                foreach (var declaredNamespace in _declaredNamespaces)
                {
                    var entry = (DictionaryEntry)declaredNamespace;
                    if (entry.Value.ToString() == value) return;
                }
                _declaredNamespaces[name] = value;
            }
        }

        private void GetNsAndName(string localName, out string ns, out string name)
        {
            var names = localName.Split(new[] { ":" }, StringSplitOptions.RemoveEmptyEntries);
            name = localName;
            ns = null;
            if (names.Length == 2)
            {
                ns = names[0];
                name = names[1];
            }
            if (ns == null && name.Contains(".")) ns = string.Empty;
        }

        private void IncreaseBlockCounter()
        {
            if (_elementStack.Count == 0) return;
            var elementInfo = _elementStack.Peek();
            elementInfo.BlockCounter++;
        }

        private void StartWithNewLine()
        {
            if (_newLine) return;
            if (FirstNodeInBlock) _sb.Append(":");
            _sb.AppendLine();
            _sb.Append(_indent);
            _sb.Append(_indentChar, _currentIndent * _indentMultiplicity);
            _newLine = true;
        }

        bool FirstNodeInBlock => _elementStack.Count > 0 && _elementStack.Peek().BlockCounter == 0;

        List<string> EncodeValue(string s, out bool escapeSymbolsFound, out List<Tuple<int, int>> mapping)
        {
            var result = new List<string>();
            mapping = new List<Tuple<int, int>>();
            escapeSymbolsFound = false;
            if (string.IsNullOrEmpty(s)) return result;
            //escapeSymbolsFound = s[0] == ' ' || s[0] == '"' || s[0] == '\'';
            int i;
            var len = s.Length;
            var sb = new StringBuilder();
            int lineStart = -1;
            for (i = 0; i < len; i += 1)
            {
                if (lineStart == -1) lineStart = i;
                var c = s[i];
                switch (c)
                {
                    case '\'':
                    case '\\':
                    case '/':
                        sb.Append('\\');
                        sb.Append(c);
                        break;
                    case '\b':
                        sb.Append("\\b");
                        escapeSymbolsFound = true;
                        break;
                    case '\t':
                        sb.Append("\\t");
                        escapeSymbolsFound = true;
                        break;
                    case '\n':
                        result.Add(sb.ToString());
                        mapping.Add(new Tuple<int, int>(lineStart, i - 1));
                        lineStart = -1;
                        sb = new StringBuilder();
                        break;
                    case '\f':
                        sb.Append("\\f");
                        escapeSymbolsFound = true;
                        break;
                    case '\r':
                        result.Add(sb.ToString());
                        mapping.Add(new Tuple<int, int>(lineStart, i - 1));
                        lineStart = -1;
                        sb = new StringBuilder();
                        if (i + 1 < len && s[i + 1] == '\n') i++;
                        break;
                    default:
                        if (c < ' ')
                        {
                            var t = "000" + $"{System.Convert.ToInt32(c):X}";
                            sb.Append("\\u" + t.Substring(t.Length - 4));
                            escapeSymbolsFound = true;
                        }
                        else
                        {
                            sb.Append(c);
                        }
                        break;
                }
            }
            if (sb.Length > 0)
            {
                result.Add(sb.ToString());
                mapping.Add(new Tuple<int, int>(lineStart, i - 1));
            }
            //if (s[len - 1] == ' ' || s[len - 1] == '\t') escapeSymbolsFound = true;
            return result;
        }
    }
}
