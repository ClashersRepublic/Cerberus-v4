namespace ClashersRepublic.Magic.Services.Account.Message
{
    using ClashersRepublic.Magic.Services.Account.Account;
    using ClashersRepublic.Magic.Services.Account.Service;

    using ClashersRepublic.Magic.Services.Logic;
    using ClashersRepublic.Magic.Services.Logic.Account;
    using ClashersRepublic.Magic.Services.Logic.Message;
    using ClashersRepublic.Magic.Services.Logic.Message.Account;
    using ClashersRepublic.Magic.Services.Logic.Util;

    using ClashersRepublic.Magic.Titan.Message;

    using RabbitMQ.Client.Events;

    internal static class MessageManager
    {
        /// <summary>
        ///     Receives a service message.
        /// </summary>
        internal static void ReceiveMessage(PiranhaMessage message, BasicDeliverEventArgs args)
        {
            switch (message.GetMessageType())
            {
                case 10150:
                {
                    MessageManager.CreateAccountMessageReceived((LoginAccountMessage) message, args);
                    break;
                }
            }
        }

        /// <summary>
        ///     Sends the specified message to service.
        /// </summary>
        private static void SendMessage(MagicServiceMessage message, string exchangeKey, string routingKey)
        {
            ServiceMessaging.SendMessage(message, exchangeKey, routingKey);
        }

        /// <summary>
        ///     Called when a <see cref="CreateAccountMessage" /> has been received.
        /// </summary>
        private static void CreateAccountMessageReceived(LoginAccountMessage message, BasicDeliverEventArgs args)
        {
            GameAccount createAccount = GameAccountManager.CreateAccount();

            if (createAccount != null)
            {
                createAccount.StartSession(message.ProxySessionId);

                SessionUtil.DecodeSessionId(message.ProxySessionId, out int proxyId, out long _);
                /*MessageManager.SendMessage(new CreateAccountOkMessage
                    {
                        ProxySessionId = message.ProxySessionId,
                        Account = createAccount
                    }, ServiceExchangeName.PROXY_EXCHANGE_NAME, ServiceExchangeName.PROXY_ROUTING_KEY_PREFIX + proxyId);*/
            }
        }
    }
}