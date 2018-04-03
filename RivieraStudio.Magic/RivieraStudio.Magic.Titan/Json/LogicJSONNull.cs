namespace RivieraStudio.Magic.Titan.Json
{
    using System.Text;

    public class LogicJSONNull : LogicJSONNode
    {
        /// <summary>
        ///     Gets the json node type.
        /// </summary>
        public override int GetJSONNodeType()
        {
            return 6;
        }

        /// <summary>
        ///     Writes this node to builder.
        /// </summary>
        public override void WriteToString(StringBuilder builder)
        {
            builder.Append("null");
        }
    }
}