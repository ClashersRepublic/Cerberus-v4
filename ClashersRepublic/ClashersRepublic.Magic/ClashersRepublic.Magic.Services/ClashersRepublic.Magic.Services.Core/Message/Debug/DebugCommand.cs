namespace ClashersRepublic.Magic.Services.Core.Message.Debug
{
    using ClashersRepublic.Magic.Titan.DataStream;

    public class DebugCommand
    {
        /// <summary>
        ///     Initialzes a new instance of the <see cref="DebugCommand"/> class.
        /// </summary>
        public DebugCommand()
        {
            // DebugCommand.
        }

        /// <summary>
        ///     Destucts this instance.
        /// </summary>
        public virtual void Destruct()
        {
            // Destruct.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public virtual void Decode(ByteStream stream)
        {
            // Decode.
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public virtual void Encode(ChecksumEncoder encoder)
        {
            // Encode.
        }

        /// <summary>
        ///     Gets the command type.
        /// </summary>
        public virtual int GetCommandType()
        {
            return 0;
        }

        /// <summary>
        ///     Gets the service node type.
        /// </summary>
        public virtual int GetServiceNodeType()
        {
            return -1;
        }
    }
}