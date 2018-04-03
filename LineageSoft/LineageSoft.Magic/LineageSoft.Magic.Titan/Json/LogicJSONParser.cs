namespace LineageSoft.Magic.Titan.Json
{
    using System;
    using System.Text;
    using LineageSoft.Magic.Titan.Debug;

    public class LogicJSONParser
    {
        /// <summary>
        ///     Creates a JSON string value.
        /// </summary>
        public static string CreateJSONString(LogicJSONNode root, int ensureCapacity = 20)
        {
            StringBuilder builder = new StringBuilder(ensureCapacity);
            root.WriteToString(builder);
            return builder.ToString();
        }

        /// <summary>
        ///     Writes a string on builder.
        /// </summary>
        public static void WriteString(string value, StringBuilder builder)
        {
            builder.Append('"');

            if (!string.IsNullOrEmpty(value))
            {
                for (int i = 0; i < value.Length; i++)
                {
                    char charValue = value[i];

                    if (charValue <= '\r' && charValue >= '\b')
                    {
                        switch (charValue)
                        {
                            case '\b':
                            {
                                builder.Append("\\b");
                                break;
                            }

                            case '\t':
                            {
                                builder.Append("\\t");
                                break;
                            }

                            case '\n':
                            {
                                builder.Append("\\n");
                                break;
                            }

                            case '\f':
                            {
                                builder.Append("\\f");
                                break;
                            }

                            case '\r':
                            {
                                builder.Append("\\r");
                                break;
                            }

                            default:
                            {
                                builder.Append(charValue);
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (charValue == '"')
                        {
                            builder.Append("\\\"");
                        }
                        else if (charValue == '/')
                        {
                            builder.Append("\\/");
                        }
                        else if (charValue == '\\')
                        {
                            builder.Append("\\\\");
                        }
                        else
                        {
                            builder.Append(charValue);
                        }
                    }
                }
            }

            builder.Append('"');
        }

        /// <summary>
        ///     Parses the error.
        /// </summary>
        public static void ParseError(string error)
        {
            Debugger.Warning("JSON Parse error: " + error);
        }

        /// <summary>
        ///     Parses the specified string.
        /// </summary>
        public static LogicJSONNode Parse(string json)
        {
            return LogicJSONParser.ParseValue(new CharStream(json));
        }

        private static LogicJSONNode ParseValue(CharStream stream)
        {
            stream.SkipWhitespace();

            char charValue = stream.NextChar();
            LogicJSONNode node = null;

            switch (charValue)
            {
                case '{':
                {
                    node = LogicJSONParser.ParseObject(stream);
                    break;
                }

                case '[':
                {
                    node = LogicJSONParser.ParseArray(stream);
                    break;
                }

                case 'n':
                {
                    node = LogicJSONParser.ParseNull(stream);
                    break;
                }

                case 'f':
                {
                    node = LogicJSONParser.ParseBoolean(stream);
                    break;
                }

                case 't':
                {
                    node = LogicJSONParser.ParseBoolean(stream);
                    break;
                }

                case '"':
                {
                    node = LogicJSONParser.ParseString(stream);
                    break;
                }

                case '-':
                {
                    node = LogicJSONParser.ParseNumber(stream);
                    break;
                }

                default:
                {
                    if (charValue >= '0' && charValue <= '9')
                    {
                        node = LogicJSONParser.ParseNumber(stream);
                    }
                    else
                    {
                        LogicJSONParser.ParseError("Not of any recognized value: " + charValue);
                    }

                    break;
                }
            }

            return node;
        }

        private static LogicJSONArray ParseArray(CharStream stream)
        {
            stream.SkipWhitespace();

            if (stream.Read() != '[')
            {
                LogicJSONParser.ParseError("Not an array");
                return null;
            }

            LogicJSONArray jsonArray = new LogicJSONArray();

            stream.SkipWhitespace();

            char nextChar = stream.NextChar();

            if (nextChar != '\0')
            {
                if (nextChar == ']')
                {
                    stream.Read();
                    return jsonArray;
                }

                while (true)
                {
                    LogicJSONNode node = LogicJSONParser.ParseValue(stream);

                    if (node != null)
                    {
                        jsonArray.Add(node);
                        stream.SkipWhitespace();

                        nextChar = stream.Read();

                        if (nextChar != ',')
                        {
                            if (nextChar == ']')
                            {
                                return jsonArray;
                            }

                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            LogicJSONParser.ParseError("Not an array");
            return null;
        }

        private static LogicJSONObject ParseObject(CharStream stream)
        {
            stream.SkipWhitespace();

            if (stream.Read() != '{')
            {
                LogicJSONParser.ParseError("Not an object");
                return null;
            }

            LogicJSONObject jsonObject = new LogicJSONObject();

            stream.SkipWhitespace();

            char nextChar = stream.NextChar();

            if (nextChar != '\0')
            {
                if (nextChar == '}')
                {
                    stream.Read();
                    return jsonObject;
                }

                while (true)
                {
                    LogicJSONString key = LogicJSONParser.ParseString(stream);

                    if (key != null)
                    {
                        stream.SkipWhitespace();

                        nextChar = stream.Read();

                        if (nextChar != ':')
                        {
                            break;
                        }

                        LogicJSONNode node = LogicJSONParser.ParseValue(stream);

                        if (node != null)
                        {
                            jsonObject.Put(key.GetStringValue(), node);
                            stream.SkipWhitespace();

                            nextChar = stream.Read();

                            if (nextChar != ',')
                            {
                                if (nextChar == '}')
                                {
                                    return jsonObject;
                                }

                                break;
                            }
                        }
                        else
                        {
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }
            }

            LogicJSONParser.ParseError("Not an object");
            return null;
        }

        private static LogicJSONString ParseString(CharStream stream)
        {
            stream.SkipWhitespace();

            if (stream.Read() != '"')
            {
                LogicJSONParser.ParseError("Not a string");
                return null;
            }

            StringBuilder builder = new StringBuilder();

            while (true)
            {
                char nextChar = stream.Read();

                if (nextChar != '\0')
                {
                    if (nextChar != '"')
                    {
                        if (nextChar == '\\')
                        {
                            nextChar = stream.Read();

                            switch (nextChar)
                            {
                                case 'n':
                                {
                                    nextChar = '\n';
                                    break;
                                }

                                case 'r':
                                {
                                    nextChar = '\r';
                                    break;
                                }

                                case 't':
                                {
                                    nextChar = '\t';
                                    break;
                                }

                                case 'u':
                                {
                                    throw new NotImplementedException();
                                }

                                case 'b':
                                {
                                    nextChar = '\b';
                                    break;
                                }

                                case 'f':
                                {
                                    nextChar = '\f';
                                    break;
                                }

                                case '\0':
                                {
                                    LogicJSONParser.ParseError("Not a string");
                                    return null;
                                }
                            }
                        }

                        builder.Append(nextChar);
                    }
                    else
                    {
                        break;
                    }
                }
                else
                {
                    LogicJSONParser.ParseError("Not a string");
                    return null;
                }
            }

            return new LogicJSONString(builder.ToString());
        }

        private static LogicJSONBoolean ParseBoolean(CharStream stream)
        {
            stream.SkipWhitespace();

            char nextChar = stream.Read();

            if (nextChar == 'f')
            {
                if (stream.Read() == 'a')
                {
                    if (stream.Read() == 'l')
                    {
                        if (stream.Read() == 's')
                        {
                            if (stream.Read() == 'e')
                            {
                                return new LogicJSONBoolean(false);
                            }
                        }
                    }
                }

                LogicJSONParser.ParseError("Not a boolean");
            }
            else if (nextChar == 't')
            {
                if (stream.Read() == 'r')
                {
                    if (stream.Read() == 'u')
                    {
                        if (stream.Read() == 'e')
                        {
                            return new LogicJSONBoolean(true);
                        }
                    }
                }

                LogicJSONParser.ParseError("Not a boolean");
            }
            else
            {
                LogicJSONParser.ParseError("Not a boolean");
            }

            return null;
        }

        private static LogicJSONNull ParseNull(CharStream stream)
        {
            stream.SkipWhitespace();

            char nextChar = stream.Read();

            if (nextChar == 'n')
            {
                if (stream.Read() == 'u')
                {
                    if (stream.Read() == 'l')
                    {
                        if (stream.Read() == 'l')
                        {
                            return new LogicJSONNull();
                        }
                    }
                }
            }

            LogicJSONParser.ParseError("Not a null");
            return null;
        }

        private static LogicJSONNumber ParseNumber(CharStream stream)
        {
            stream.SkipWhitespace();

            char nextChar = stream.NextChar();
            int multiplier = 1;

            if (nextChar == '-')
            {
                multiplier = -1;
                nextChar = stream.Read();
            }

            if (nextChar != ',')
            {
                int value = 0;

                while ((nextChar = stream.Read()) <= '9' && nextChar >= '0')
                {
                    value = nextChar - '0' + 10 * value;

                    if ((nextChar = stream.NextChar()) > '9' || nextChar < '0')
                    {
                        break;
                    }
                }

                if (nextChar == 'e' || nextChar == 'E' || nextChar == '.')
                {
                    LogicJSONParser.ParseError("JSON floats not supported");
                    return null;
                }

                return new LogicJSONNumber(value * multiplier);
            }

            LogicJSONParser.ParseError("Not a number");
            return null;
        }

        private class CharStream
        {
            private readonly string _string;
            private int _offset;

            /// <summary>
            ///     Initializes a new instance of the <see cref="CharStream" /> class.
            /// </summary>
            public CharStream(string str)
            {
                this._string = str;
            }

            /// <summary>
            ///     Reads the next char.
            /// </summary>
            public char Read()
            {
                if (this._offset >= this._string.Length)
                {
                    return '\0';
                }

                return this._string[this._offset++];
            }

            /// <summary>
            ///     Gets the next char without increment offset.
            /// </summary>
            public char NextChar()
            {
                if (this._offset >= this._string.Length)
                {
                    return '\0';
                }

                return this._string[this._offset];
            }

            /// <summary>
            ///     Gets the stream offset.
            /// </summary>
            public int GetOffset()
            {
                return this._offset;
            }

            /// <summary>
            ///     Sets the stream offset.
            /// </summary>
            public void SetOffset(int offset)
            {
                this._offset = offset;
            }

            /// <summary>
            ///     Skips the next whitespace.
            /// </summary>
            public void SkipWhitespace()
            {
                char charValue;

                do
                {
                    charValue = this.Read();
                } while (charValue <= ' ' && charValue != '\0');

                this._offset--;
            }
        }
    }
}