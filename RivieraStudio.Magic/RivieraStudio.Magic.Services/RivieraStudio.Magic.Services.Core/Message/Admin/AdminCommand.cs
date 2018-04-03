namespace RivieraStudio.Magic.Services.Core.Message.Admin
{
    using RivieraStudio.Magic.Titan.DataStream;

    public class AdminCommand
    {
        /// <summary>
        ///     Initialzes a new instance of the <see cref="AdminCommand"/> class.
        /// </summary>
        public AdminCommand()
        {
            // AdminCommand.
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