namespace RivieraStudio.Magic.Logic.Util
{
    using RivieraStudio.Magic.Titan.Math;
    using RivieraStudio.Magic.Titan.Util;

    public class HashTagCodeGenerator
    {
        private readonly LogicLongToCodeConverterUtil _codeConverterUtil;

        /// <summary>
        ///     Initializes a new instance of the <see cref="HashTagCodeGenerator" /> class.
        /// </summary>
        public HashTagCodeGenerator()
        {
            this._codeConverterUtil = new LogicLongToCodeConverterUtil("#", "0289PYLQGRJCUV");
        }

        /// <summary>
        ///     Converts to code the specified long.
        /// </summary>
        public string ToCode(LogicLong logicLong)
        {
            return this._codeConverterUtil.ToCode(logicLong);
        }

        /// <summary>
        ///     Converts to id the specified string.
        /// </summary>
        public LogicLong ToId(string value)
        {
            LogicLong id = this._codeConverterUtil.ToId(value);

            if (this.IsIdValid(id))
            {
                return id;
            }

            return null;
        }

        /// <summary>
        ///     Gets if the specified id is valid.
        /// </summary>
        public bool IsIdValid(LogicLong id)
        {
            return id.GetHigherInt() != -1 && id.GetHigherInt() != -1;
        }
    }
}