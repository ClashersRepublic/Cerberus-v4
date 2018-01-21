﻿namespace ClashersRepublic.Magic.Titan.Json
{
    using System.Text;

    public class LogicJSONNumber : LogicJSONNode
    {
        private readonly int _value;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicJSONNumber"/> class.
        /// </summary>
        public LogicJSONNumber(int value)
        {
            this._value = value;
        }
        
        /// <summary>
        ///     Gets the integer value.
        /// </summary>
        public int GetIntValue()
        {
            return this._value;
        }

        /// <summary>
        ///     Gets the json node type.
        /// </summary>
        public override int GetJSONNodeType()
        {
            return 3;
        }

        /// <summary>
        ///     Writes this node to builder.
        /// </summary>
        public override void WriteToString(StringBuilder builder)
        {
            builder.Append(this._value);
        }
    }
}