namespace ClashersRepublic.Magic.Services.Account.Message
{
    using ClashersRepublic.Magic.Logic.Message;
    using ClashersRepublic.Magic.Services.Account.Logic.Account;
    using ClashersRepublic.Magic.Services.Account.Service;
    using ClashersRepublic.Magic.Services.Logic.Account;
    using ClashersRepublic.Magic.Services.Logic.Message.Account;

    internal static class MessageManager
    {
        /// <summary>
        ///     Receives a service message.
        /// </summary>
        internal static void ReceiveMessage(PiranhaMessage message, string routingKey)
        {
            switch (message.GetMessageType())
            {
                case 10200:
                {
                    MessageManager.StartSessionReceived((StartSessionMessage) message, routingKey);
                    break;
                }
            }
        }

        /// <summary>
        ///     Called when a <see cref="StartSessionMessage"/> has been received.
        /// </summary>
        private static void StartSessionReceived(StartSessionMessage message, string routingKey)
        {
            if (message.AccountId.IsZero())
            {
                if (message.PassToken == null)
                {
                    GameAccount account = GameAccountManager.GetAccount(message.AccountId);

                    if (account != null)
                    {
                        if (account.PassToken.Equals(message.PassToken))
                        {
                            MessageManager.SendMessage(new StartSessionOkMessage
                            {
                                Account = account,
                                SessionId = message.SessionId
                            }, routingKey);
                        }
                        else
                        {
                            MessageManager.SendMessage(new StartSessionFailedMessage
                            {
                                ErrorCode = 3
                            }, routingKey);
                        }
                    }
                    else
                    {
                        MessageManager.SendMessage(new StartSessionFailedMessage
                        {
                            ErrorCode = 2
                        }, routingKey);
                    }
                }
                else
                {
                    MessageManager.SendMessage(new StartSessionFailedMessage
                    {
                        ErrorCode = 1
                    }, routingKey);

                    return;
                }
            }
            else
            {

            }
        }

        /// <summary>
        ///     Sends the specified message to service.
        /// </summary>
        private static void SendMessage(PiranhaMessage message, string routingKey)
        {
            ServiceConnectionProcessor.EnqueueSentMessage(message, string.Empty, routingKey);
        }
    }
}