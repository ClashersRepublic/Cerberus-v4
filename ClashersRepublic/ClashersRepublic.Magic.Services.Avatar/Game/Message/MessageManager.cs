namespace ClashersRepublic.Magic.Services.Avatar.Game.Message
{
    using ClashersRepublic.Magic.Services.Avatar.Network.Session;
    using ClashersRepublic.Magic.Titan.Message;

    internal class MessageManager
    {
        private NetAvatarSession _session;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MessageManager"/> class.
        /// </summary>
        internal MessageManager(NetAvatarSession session)
        {
            this._session = session;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        internal void Destruct()
        {
            this._session = null;
        }

        /// <summary>
        ///     Receives the specified <see cref="PiranhaMessage"/>.
        /// </summary>
        internal void ReceiveMessage(PiranhaMessage message)
        {
            switch (message.GetMessageType())
            {
                    
            }
        }
    }
}