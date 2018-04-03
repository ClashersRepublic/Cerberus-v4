namespace ClashersRepublic.Magic.Services.Core.Message.Session
{
    public class AskForBindServerMessage : NetMessage
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
            return 10305;
        }

        /// <summary>
        ///     Sets the server type.
        /// </summary>
        public void SetServerType(int type)
        {
            this._serverType = type;
        }

        /// <summary>
        ///     Gets the server type.
        /// </summary>
        public int GetServerType()
        {
            return this._serverType;
        }

        /// <summary>
        ///     Sets the server id.
        /// </summary>
        public void SetServerId(int id)
        {
            this._serverId = id;
        }

        /// <summary>
        ///     Gets the server id.
        /// </summary>
        public int GetServerId()
        {
            return this._serverId;
        }
    }
}