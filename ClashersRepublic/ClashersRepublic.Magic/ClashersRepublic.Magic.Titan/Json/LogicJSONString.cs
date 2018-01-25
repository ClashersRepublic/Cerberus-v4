namespace ClashersRepublic.Magic.Titan.Json
{
    using System.Text;

    public class LogicJSONString : LogicJSONNode
    {
        private readonly string _value;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicJSONString" /> class.
        /// </summary>
        public LogicJSONString(string value)
        {
            this._value = value;
        }

        /// <summary>
        ///     Gets the string value.
        /// </summary>
        public string GetStringValue()
        {
            return this._value;
        }

        /// <summary>
        ///     Gets the json node type.
        /// </summary>
        public override int GetJSONNodeType()
        {
            return 4;
        }

        /// <summary>
        ///     Writes this node to builder.
        /// </summary>
        public override void WriteToString(StringBuilder builder)
        {
            LogicJSONParser.WriteString(this._value, builder);
        }
    }
}