namespace RivieraStudio.Magic.Titan.Json
{
    using System.Text;

    public class LogicJSONBoolean : LogicJSONNode
    {
        private readonly bool _value;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicJSONBoolean" /> class.
        /// </summary>
        public LogicJSONBoolean(bool value)
        {
            this._value = value;
        }

        /// <summary>
        ///     Gets a value indicating whether the value is true.
        /// </summary>
        public bool IsTrue()
        {
            return this._value;
        }

        /// <summary>
        ///     Gets the json node type.
        /// </summary>
        public override int GetJSONNodeType()
        {
            return 5;
        }

        /// <summary>
        ///     Writes this node to builder.
        /// </summary>
        public override void WriteToString(StringBuilder builder)
        {
            builder.Append(this._value ? "true" : "false");
        }
    }
}