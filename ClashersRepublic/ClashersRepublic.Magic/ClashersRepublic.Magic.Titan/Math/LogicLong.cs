﻿namespace ClashersRepublic.Magic.Titan.Math
{
    using System.Runtime.InteropServices;
    using ClashersRepublic.Magic.Titan.DataStream;
    using ClashersRepublic.Magic.Titan.Util;

    [ComVisible(true)]
    [StructLayout(LayoutKind.Sequential, Size = 8)]
    public struct LogicLong
    {
        private int _highInteger;
        private int _lowInteger;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicLong" /> struct.
        /// </summary>
        /// <param name="Long">The long.</param>
        public LogicLong(long Long)
        {
            this._highInteger = (int) (Long >> 32);
            this._lowInteger = (int) Long;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicLong" /> struct.
        /// </summary>
        /// <param name="highInteger">The high integer.</param>
        /// <param name="lowInteger">The low integer.</param>
        public LogicLong(int highInteger, int lowInteger)
        {
            this._highInteger = highInteger;
            this._lowInteger = lowInteger;
        }

        /// <summary>
        ///     Gets a value indicating whether this instance is equals at zero.
        /// </summary>
        public bool IsZero()
        {
            return this._highInteger == 0 && this._lowInteger == 0;
        }

        /// <summary>
        ///     Gets the higher int value.
        /// </summary>
        public int GetHigherInt()
        {
            return this._highInteger;
        }

        /// <summary>
        ///     Gets the lower int value.
        /// </summary>
        public int GetLowerInt()
        {
            return this._lowInteger;
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public void Decode(ByteStream stream)
        {
            this._highInteger = stream.ReadInt();
            this._lowInteger = stream.ReadInt();
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public void Encode(ChecksumEncoder stream)
        {
            stream.WriteInt(this._highInteger);
            stream.WriteInt(this._lowInteger);
        }

        /// <summary>
        ///     Gets a value indicating whether the specified object is equal to this instance.
        /// </summary>
        public bool Equals(LogicLong logicLong)
        {
            return this._highInteger == logicLong._highInteger && this._lowInteger == logicLong._lowInteger;
        }

        /// <summary>
        ///     Gets the hash code of this instance.
        /// </summary>
        public int HashCode()
        {
            return this._lowInteger + 31 * this._highInteger;
        }

        /// <summary>
        ///     Returns a <see cref="string" /> that represents this instance.
        /// </summary>
        public override string ToString()
        {
            return "LogicLong(" + this._highInteger + "-" + this._lowInteger + ")";
        }

        public static implicit operator LogicLong(long Long)
        {
            return new LogicLong(Long);
        }

        public static implicit operator long(LogicLong Long)
        {
            return (long) Long._highInteger | (uint) Long._lowInteger;
        }
    }
}