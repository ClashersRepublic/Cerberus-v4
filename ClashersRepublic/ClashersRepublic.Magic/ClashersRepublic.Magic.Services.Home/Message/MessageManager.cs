namespace ClashersRepublic.Magic.Services.Home.Message
{
    using ClashersRepublic.Magic.Services.Home.Game;
    using ClashersRepublic.Magic.Services.Home.Service;

    using ClashersRepublic.Magic.Services.Logic;
    using ClashersRepublic.Magic.Services.Logic.Message;
    using ClashersRepublic.Magic.Services.Logic.Message.Account;
    using ClashersRepublic.Magic.Services.Logic.Message.Avatar;
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
                case 10200:
                {
                    MessageManager.CreateAvatarMessageReceived((CreateAvatarMessage) message, args);
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
        ///     Called when a <see cref="CreateAvatarMessage" /> has been received.
        /// </summary>
        private static void CreateAvatarMessageReceived(CreateAvatarMessage message, BasicDeliverEventArgs args)
        {
            GameManager.CreatePlayer(message.AccountId);
        }
    }
}