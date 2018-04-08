﻿namespace RivieraStudio.Magic.Services.Core.Message.Admin
{
    using RivieraStudio.Magic.Services.Core.Utils;
    using RivieraStudio.Magic.Titan.DataStream;

    public class AdminResetZoneCommand : AdminCommand
    {
        /// <summary>
        ///     Initialzes a new instance of the <see cref="AdminResetZoneCommand"/> class.
        /// </summary>
        public AdminResetZoneCommand()
        {
            // AdminResetZoneCommand.
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