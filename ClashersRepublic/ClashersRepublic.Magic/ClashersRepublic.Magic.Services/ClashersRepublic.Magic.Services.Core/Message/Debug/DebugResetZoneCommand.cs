namespace ClashersRepublic.Magic.Services.Core.Message.Debug
{
    using ClashersRepublic.Magic.Services.Core.Utils;
    using ClashersRepublic.Magic.Titan.DataStream;

    public class DebugResetZoneCommand : DebugCommand
    {
        /// <summary>
        ///     Initialzes a new instance of the <see cref="DebugResetZoneCommand"/> class.
        /// </summary>
        public DebugResetZoneCommand()
        {
            // DebugResetZoneCommand.
        }

        /// <summary>
        ///     Destucts this instance.
        /// </summary>
        public override void Destruct()
        {
            // Destruct.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode(ByteStream stream)
        {
            // Decode.
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode(ChecksumEncoder encoder)
        {
            // Encode.
        }

        /// <summary>
        ///     Gets the command type.
        /// </summary>
        public override int GetCommandType()
        {
            return 1000;
        }

        /// <summary>
        ///     Gets the service node type.
        /// </summary>
        public override int GetServiceNodeType()
        {
            return NetUtils.SERVICE_NODE_TYPE_ZONE_CONTAINER;
        }
    }
}