namespace ClashersRepublic.Magic.Titan.Util
{
    using System;
    using ClashersRepublic.Magic.Titan.Debug;
    using ClashersRepublic.Magic.Titan.Math;

    public class LogicLongToCodeConverterUtil
    {
        private string _hashTag;
        private string _conversionChars;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicLongToCodeConverterUtil"/> class.
        /// </summary>
        public LogicLongToCodeConverterUtil(string hashTag, string conversionChars)
        {
            this._hashTag = hashTag;
            this._conversionChars = conversionChars;
        }

        /// <summary>
        ///     Converts the <see cref="LogicLong"/> to code.
        /// </summary>
        public string ToCode(LogicLong logicLong)
        {
            int highValue = logicLong.GetHigherInt();

            if (highValue < 256)
            {
                return this._hashTag + this.Convert((long) logicLong.GetLowerInt() << 8 | (uint) highValue);
            }

            Debugger.Warning("Cannot convert the code to string. Higher int value too large");

            return null;
        }

        /// <summary>
        ///     Converts to id the specified code.
        /// </summary>
        public LogicLong ToId(string code)
        {
            if (code.Length < 14)
            {
                string idCode = code.Substring(1);
                long id = this.ConvertCode(idCode);

                if (id != -1)
                {
                   return new LogicLong((int) (id % 256), (int) (id >> 8 & 0x7FFFFFFF));
                }
            }
            else
            {
                Debugger.Warning("Cannot convert the string to code. String is too long.");
            }

            return new LogicLong(-1, -1);
        }

        /// <summary>
        ///     Converts the <see cref="string"/> to id.
        /// </summary>
        private long ConvertCode(string code)
        {
            long id = 0;
            int conversionCharsCount = this._conversionChars.Length;
            int codeCharsCount = code.Length;

            for (int i = 0; i < codeCharsCount; i++)
            {
                int charIndex = this._conversionChars.IndexOf(code[i]);

                if (charIndex == -1)
                {
                    Debugger.Warning("Cannot convert the string to code. String contains invalid character(s).");
                    id = -1;
                    break;
                }

                id = id * conversionCharsCount + charIndex;
            }

            return id;
        }

        /// <summary>
        ///     Converts the long to code.
        /// </summary>
        private string Convert(long value)
        {
            char[] code = new char[12];

            if (value > -1)
            {
                int conversionCharsCount = this._conversionChars.Length;

                for (int i = 11; i >= 0; i--)
                {
                    code[i] = this._conversionChars[(int)(value % conversionCharsCount)];
                    value /= conversionCharsCount;

                    if (value == 0)
                    {
                        return new string(code, i, 12 - i);
                    }
                }

                return new string(code);
            }
            else
            {
                Debugger.Warning("LogicLongToCodeConverter: value to convert cannot be negative");
            }

            return null;
        }
    }
}