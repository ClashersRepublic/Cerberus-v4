namespace RivieraStudio.Magic.Services.Core.Message.Session
{
    public class UpdateServerEndPointMessage : NetMessage
    {
        private int _serverType;
        private int _serverId;

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
            this.Stream.WriteVInt(this._serverType);
            this.Stream.WriteVInt(this._serverId);
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
            this._serverType = this.Stream.ReadVInt();
            this._serverId = this.Stream.ReadVInt();
        }

        /// <summary>
        ///     Gets the message type of this instance.
        /// </summary>
        public override int GetMessageType()
        {
            return 10303;
        }

        /// <summary>
        ///     Gets the service node type.
        /// </summary>
        public int GetServerType()
        {
            return this._serverType;
        }

        /// <summary>
        ///     Sets the service node type.
        /// </summary>
        public void SetServerType(int value)
        {
            this._serverType = value;
        }

        /// <summary>
        ///     Gets the server id.
        /// </summary>
        public int GetServerId()
        {
            return this._serverId;
        }

        /// <summary>
        ///     Sets the server id.
        /// </summary>
        public void SetServerId(int value)
        {
            this._serverId = value;
        }
    }
}