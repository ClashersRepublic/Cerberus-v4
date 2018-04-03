namespace LineageSoft.Magic.Services.Party.Game.Message
{
    using LineageSoft.Magic.Services.Party.Network.Session;
    using LineageSoft.Magic.Titan.Message;

    internal class MessageManager
    {
        private NetPartySession _session;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MessageManager"/> class.
        /// </summary>
        internal MessageManager(NetPartySession session)
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