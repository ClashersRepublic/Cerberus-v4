namespace ClashersRepublic.Magic.Services.Proxy.Network.Message
{
    using ClashersRepublic.Magic.Services.Core.Message;
    using ClashersRepublic.Magic.Services.Core.Message.Account;

    internal class NetMessageManager : INetMessageManager
    {
        /// <summary>
        ///     Receives the specicified <see cref="NetMessage"/>.
        /// </summary>
        public void ReceiveMessage(NetMessage message)
        {
            switch (message.GetMessageType())
            {
                case 20101:
                    this.CreateAccountOkMessageReceived((CreateAccountOkMessage) message);
                    break;
            }
        }

        /// <summary>
        ///     Called when the <see cref="CreateAccountOkMessage"/> is received.
        /// </summary>
        internal void CreateAccountOkMessageReceived(CreateAccountOkMessage message)
        {

        }
    }
}