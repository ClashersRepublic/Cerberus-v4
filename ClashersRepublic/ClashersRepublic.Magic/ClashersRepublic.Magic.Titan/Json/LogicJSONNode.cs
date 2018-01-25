namespace ClashersRepublic.Magic.Titan.Json
{
    using System.Text;

    public class LogicJSONNode
    {
        /// <summary>
        ///     Gets the json node type.
        /// </summary>
        public virtual int GetJSONNodeType()
        {
            return 0;
        }

        /// <summary>
        ///     Writes this node to builder.
        /// </summary>
        public virtual void WriteToString(StringBuilder builder)
        {
            // WriteToString.
        }
    }
}