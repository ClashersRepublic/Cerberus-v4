namespace ClashersRepublic.Magic.Services.Home.Message
{
    using System;
    using ClashersRepublic.Magic.Logic.Message.Home;
    using ClashersRepublic.Magic.Logic.Message.Battle;

    using ClashersRepublic.Magic.Logic.Data;

    using ClashersRepublic.Magic.Services.Home.Debug;
    using ClashersRepublic.Magic.Services.Home.Session;

    using ClashersRepublic.Magic.Titan.Message;

    internal static class MessageManager
    {
        /// <summary>
        ///     Receives the game <see cref="PiranhaMessage"/>.
        /// </summary>
        internal static void ReceiveMessage(PiranhaMessage message, GameSession session)
        {
            int messageType = message.GetMessageType();

            if (message.GetServiceNodeType() != 10)
            {
                Logging.Warning(typeof(MessageManager), "MessageManager::receiveMessage invalid message service node, type: " + messageType);
                return;
            }

            switch (messageType)
            {
                case 14102:
                {
                    MessageManager.EndClientTurnMessageReceived((EndClientTurnMessage) message, session);
                    break;
                }

                case 14134:
                {
                    MessageManager.AttackNpcMessageReceived((AttackNpcMessage)message, session);
                    break;
                }

                default:
                {
                    Logging.Warning(typeof(MessageManager), "MessageManager::receiveMessage no case exist for message type " + messageType);
                    break;
                }
            }
        }

        /// <summary>
        ///     Called when a <see cref="EndClientTurnMessage"/> has been received.
        /// </summary>
        internal static void EndClientTurnMessageReceived(EndClientTurnMessage message, GameSession session)
        {
            Console.WriteLine("EndClientTurnMessage: subtick: " + message.Subtick + " checksum: " + message.Checksum);
        }

        /// <summary>
        ///     Called when a <see cref="AttackNpcMessage"/> has been received.
        /// </summary>
        internal static void AttackNpcMessageReceived(AttackNpcMessage message, GameSession session)
        {
            Console.WriteLine("AttackNpcMessage: Name: "+ message.LogicNpcData.GetName() + " LevelFile: " + ((LogicNpcData)message.LogicNpcData).LevelFile);
        }
    }
}