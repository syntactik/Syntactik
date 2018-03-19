using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;


namespace Syntactik.Converter
{
    class TokenInfo
    {
        public JsonToken Token;
        public int BlockCounter;
        public bool IsArrayItem;
        public string FirstPropertyName;
        public object FirstPropertyValue;

    }
    public class JsonToSyntactikConverter
    {
        private readonly string _text;
        private StringBuilder _sb;
        private bool _newLine = true;
        private string _indent;
        private int _currentIndent = -1;
        private char _indentChar;
        private int _indentMultiplicity;
        readonly Stack<TokenInfo> _currentToken = new Stack<TokenInfo>();

        public JsonToSyntactikConverter(string text)
        {
            _text = text;
        }

        private bool CurrentTokenIsArray => _currentToken.Count > 0 && _currentToken.Peek().Token == JsonToken.StartArray;

        /// <summary>
        /// CurrentBlockToken returns either null or JsonToken.StartArray or JsonToken.StartObject
        /// </summary>
        private TokenInfo CurrentBlockToken => _currentToken.Count == 0 ? null : _currentToken.Peek();

        private int BlockCounter => _currentToken.Count == 0 ? 0 : _currentToken.Peek().BlockCounter;

        public bool Convert(int indent, char indentChar, int indentMultiplicity, bool insertNewLine, out string s4x)
        {
            _indentChar = indentChar;
            _indentMultiplicity = indentMultiplicity;

            _indent = new string(indentChar, indent * indentMultiplicity);
            _sb = new StringBuilder();
            if (insertNewLine)
            {
                _sb.AppendLine();
                _sb.Append(_indent);
            }
            using (var stringReader = new StringReader(_text))
            using (var reader = new JsonTextReader(stringReader))
            {
                while (reader.Read())
                {
                    switch (reader.TokenType)
                    {
                        case JsonToken.Boolean:
                        case JsonToken.Bytes:
                        case JsonToken.Date:
                        case JsonToken.Float:
                        case JsonToken.Integer:
                        case JsonToken.Null:
                        case JsonToken.String:
                            if (CurrentTokenIsArray)
                            {
                                StartWithNewLine();
                                IncreaseBlockCounter();
                            }

                            if (!CurrentTokenIsArray && CurrentBlockToken.BlockCounter == 1)
                            {
                                if (reader.TokenType == JsonToken.Null)
                                {
                                    CurrentBlockToken.FirstPropertyValue = "null";
                                }
                                else if (reader.TokenType == JsonToken.Boolean)
                                {
                                    if (reader.Value is bool val)
                                    {
                                        CurrentBlockToken.FirstPropertyValue = val ? "true" : "false";
                                    }
                                }
                                else if (reader.Value != null)
                                {
                                    CurrentBlockToken.FirstPropertyValue = reader.Value;
                                }
                                break;
                            }

                            if (!_newLine)
                            {
                                //There is property name
                                Append(" "); //space between element and value
                            }
                            if (reader.TokenType == JsonToken.Null)
                            {
                                WriteValue("null");
                            }
                            else if (reader.TokenType == JsonToken.Boolean)
                            {
                                if (reader.Value is bool val)
                                {
                                    WriteValue(val ? "true" : "false");
                                }
                            }
                            else if (reader.Value != null)
                            {
                                WriteValue(reader.Value.ToString());
                            }
                            break;
                        case JsonToken.Comment:
                            StartWithNewLine();
                            if (reader.Value.ToString().Contains("\n"))
                            {
                                Append("\"\"\"");
                                Append(reader.Value.ToString().Replace("\"\"\"", "'''"));
                                Append("\"\"\"");
                            }
                            else
                            {
                                Append("'''");
                                Append(reader.Value);
                            }
                            break;
                        case JsonToken.PropertyName:
                            if (CurrentBlockToken.BlockCounter == 0)
                            {
                                CurrentBlockToken.FirstPropertyName = reader.Value.ToString();
                            }
                            else
                            {
                                if (CurrentBlockToken.BlockCounter == 1)
                                {
                                    StartWithNewLine();
                                    AddFirstPropetyInBlock();
                                }
                                StartWithNewLine();
                                WritePropertyName(reader.Value.ToString());

                            }
                            IncreaseBlockCounter();
                            break;
                        case JsonToken.StartArray:
                            if (BlockCounter == 1)
                            {
                                //This object belongs to either array or property
                                if (CurrentBlockToken.FirstPropertyName != null)
                                {
                                    //Belongs to property. Start the new line
                                    StartWithNewLine();
                                    AddFirstPropetyInBlock();
                                }
                            }

                            IncreaseBlockCounter();
                            if (CurrentTokenIsArray)
                            {
                                StartWithNewLine();
                            }
                            _currentIndent++;

                            if (_currentIndent > 0) Append(":"); //don't add : if this is root object

                            _currentToken.Push(new TokenInfo { BlockCounter = 0, Token = JsonToken.StartArray, IsArrayItem = CurrentTokenIsArray });
                            break;
                        case JsonToken.StartObject:
                            if (BlockCounter == 1)
                            {
                                //This object belong to either array or property
                                if (CurrentBlockToken.FirstPropertyName != null)
                                {
                                    //Belongs to property. Start the new line
                                    StartWithNewLine();
                                    AddFirstPropetyInBlock();
                                }
                            }

                            IncreaseBlockCounter();
                            if (CurrentTokenIsArray)
                            {
                                StartWithNewLine();
                            }

                            _currentIndent++;

                            if (_currentIndent > 0) Append(":"); //don't add : if this is root object

                            _currentToken.Push(new TokenInfo { BlockCounter = 0, Token = JsonToken.StartObject, IsArrayItem = CurrentTokenIsArray });
                            break;
                        case JsonToken.EndArray:
                            if (BlockCounter == 0) Append(":::");

                            _currentToken.Pop();
                            _currentIndent--;
                            break;

                        case JsonToken.EndObject:
                            if (BlockCounter == 0) Append("()");
                            if (BlockCounter == 1)
                            {
                                //End of Block and there is only one property added.
                                if (CurrentBlockToken.IsArrayItem)
                                {
                                    Append("\t");
                                }
                                AddFirstPropetyInBlock();
                            }
                            _currentToken.Pop();
                            _currentIndent--;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
            s4x = _sb.ToString();
            return true;
        }

        private void Append(object str)
        {
            _sb.Append(str);
            _newLine = false;
        }
        private void AddFirstPropetyInBlock()
        {
            WritePropertyName(CurrentBlockToken.FirstPropertyName);
            if (CurrentBlockToken.FirstPropertyValue == null) return;
            _sb.Append(' ');
            WriteValue(CurrentBlockToken.FirstPropertyValue.ToString());
        }

        private void WriteValue(string s)
        {
            var conv = EncodeValue(s, out var escapeSymbolsFound);

            if (escapeSymbolsFound || s.StartsWith(" ") || s.EndsWith(" ") || s.StartsWith("\"") || s.StartsWith("'"))
            {
                _sb.Append("= \"");
                _sb.Append(conv);
                _sb.Append("\"");
            }
            else
            {
                _sb.Append("= ");
                _sb.Append(s);
            }
            _newLine = false;
        }

        private void WritePropertyName(string s)
        {
            var conv = EncodeValue(s, out var escapeSymbolsFound);

            if (escapeSymbolsFound || s.StartsWith(" ") || s.EndsWith(" ") || s.StartsWith("\"") || s.StartsWith("'"))
            {
                _sb.Append("\"");
                _sb.Append(conv);
                _sb.Append("\"");
            }
            else
            {
                _sb.Append(s);
            }
            _newLine = false;
        }

        private void IncreaseBlockCounter()
        {
            if (_currentToken.Count == 0) return;
            var ti = _currentToken.Peek();
            ti.BlockCounter++;
        }

        private void StartWithNewLine()
        {
            if (_newLine) return;
            _sb.AppendLine();
            _sb.Append(_indent);
            _sb.Append(_indentChar, _currentIndent * _indentMultiplicity);
            _newLine = true;
        }

        public static string EncodeValue(string s, out bool escapeSymbolsFound)
        {
            escapeSymbolsFound = false;
            if (string.IsNullOrEmpty(s)) return "";
            int i;
            var len = s.Length;
            var sb = new StringBuilder(len + 4);

            for (i = 0; i < len; i += 1)
            {
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
                        sb.Append("\\n");
                        escapeSymbolsFound = true;
                        break;
                    case '\f':
                        sb.Append("\\f");
                        escapeSymbolsFound = true;
                        break;
                    case '\r':
                        sb.Append("\\r");
                        escapeSymbolsFound = true;
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
            return sb.ToString();
        }
    }
}
