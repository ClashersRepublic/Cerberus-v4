namespace ClashersRepublic.Magic.Services.Home.Service
{
    using System;
    using ClashersRepublic.Magic.Services.Home.Home;
    using ClashersRepublic.Magic.Services.Home.Message;
    using ClashersRepublic.Magic.Services.Home.Sessions;

    using ClashersRepublic.Magic.Services.Logic;
    using ClashersRepublic.Magic.Services.Logic.Log;

    using ClashersRepublic.Magic.Services.Logic.Message;
    using ClashersRepublic.Magic.Services.Logic.Message.Game;
    using ClashersRepublic.Magic.Services.Logic.Message.Messaging;
    using ClashersRepublic.Magic.Services.Logic.Message.Session;

    using NetMQ;

    internal static class ServiceMessageManager
    {
        /// <summary>
        ///     Receives the specified <see cref="ServiceMessage" /> class.
        /// </summary>
        internal static void ReceiveMessage(ServiceMessage message)
        {
            int messageType = message.GetMessageType();

            switch (messageType)
            {
                case 10100:
                    ServiceMessageManager.CreateDataMessageReceived((CreateDataMessage) message);
                    break;
                case 10201:
                    ServiceMessageManager.ServiceNodeBoundToSessionMessageReceived((ServiceNodeBoundToSessionMessage) message);
                    break;
                case 10202:
                    ServiceMessageManager.ServiceNodeUnboundToSessionMessageReceived((ServiceNodeUnboundToSessionMessage) message);
                    break;
                case 10300:
                    ServiceMessageManager.ForwardPiranhaMessageReceived((ForwardPiranhaMessage) message);
                    break;

                default:
                {
                    Logging.Warning(typeof(ServiceMessageManager), "ServiceMessageManager::receiveMessage no case exist for message type " + messageType);
                    break;
                }
            }
        }

        /// <summary>
        ///     Sends message to the specified socket.
        /// </summary>
        internal static void SendMessage(ServiceMessage message, NetMQSocket responseSocket)
        {
            message.SetServerId(Config.ServerId);
            message.SetServiceType(ServiceManager.SERVICE_TYPE);
            
            ServiceMessaging.Send(message, responseSocket);
        }

        /// <summary>
        ///     Sends message to the specified socket.
        /// </summary>
        internal static void SendMessage(ServiceMessage message, string sessionId, NetMQSocket responseSocket)
        {
            message.SetServerId(Config.ServerId);
            message.SetServiceType(ServiceManager.SERVICE_TYPE);
            message.SetSessionId(sessionId);

            ServiceMessaging.Send(message, responseSocket);
        }

        /// <summary>
        ///     Sends message to the specified service node.
        /// </summary>
        internal static void SendMessage(ServiceMessage message, int serviceType, int serverId)
        {
            NetMQSocket socket = ServiceManager.GetServiceSocket(serviceType, serverId);

            if (socket != null)
            {
                ServiceMessageManager.SendMessage(message, socket);
            }
        }

        /// <summary>
        ///     Sends message to the specified service node.
        /// </summary>
        internal static void SendMessage(ServiceMessage message, string sessionId, int serviceType, int serverId)
        {
            NetMQSocket socket = ServiceManager.GetServiceSocket(serviceType, serverId);

            if (socket != null)
            {
                ServiceMessageManager.SendMessage(message, sessionId, socket);
            }
        }

        /// <summary>
        ///     Called when a <see cref="CreateDataMessage"/> is received.
        /// </summary>
        private static void CreateDataMessageReceived(CreateDataMessage message)
        {
            if (!message.Id.IsZero())
            {
                if (message.Id.GetHigherInt() == Config.ServerId)
                {
                    GameHomeManager.CreateGameHome(message.Id);
                }
                else
                {
                    Logging.Warning(typeof(ServiceMessageManager), "ServiceMessageManager::createDataMessageReceived server id mismatch");
                }
            }
        }

        /// <summary>
        ///     Called when a <see cref="ServiceNodeBoundToSessionMessage"/> is received.
        /// </summary>
        private static void ServiceNodeBoundToSessionMessageReceived(ServiceNodeBoundToSessionMessage message)
        {
            GameSession gameSession = GameSessionManager.GetGameSession(message.GetSessionId());

            if (gameSession != null)
            {
                Logging.Warning(typeof(ServiceMessageManager), "ServiceMessageManager::serviceNodeBoundToSessionMessageReceived service node already bound, message received twice?");
            }
            else
            {
                gameSession = GameSessionManager.CreateGameSession(message.GetSessionId(), message.AccountId);

                if (gameSession == null)
                {
                    Logging.Warning(typeof(ServiceMessageManager), "ServiceMessageManager::serviceNodeBoundToSessionMessageReceived game session creation error");
                }
                else
                {
                    gameSession.SetServiceNode(message.GetServiceType(), message.GetServerId());
                    gameSession.SetServiceNode(ServiceManager.SERVICE_TYPE, Config.ServerId);
                    gameSession.BindServiceNode(9, Config.ServerId);

                    gameSession.GameMode.SetSession(gameSession);
                    gameSession.GameMode.SetOfflineMode(false);
                    gameSession.GameMode.LoadMode();
                }
            }
        }

        /// <summary>
        ///     Called when a <see cref="ServiceNodeUnboundToSessionMessage"/> is received.
        /// </summary>
        private static void ServiceNodeUnboundToSessionMessageReceived(ServiceNodeUnboundToSessionMessage message)
        {
            GameSession gameSession = GameSessionManager.GetGameSession(message.GetSessionId());

            if (gameSession != null)
            {
                GameSessionManager.RemoveGameSession(message.GetSessionId());
            }
        }

        /// <summary>
        ///     Called when a <see cref="ForwardPiranhaMessage"/> is received.
        /// </summary>
        private static void ForwardPiranhaMessageReceived(ForwardPiranhaMessage message)
        {
            if (message.PiranhaMessage != null)
            {
                GameSession gameSession = GameSessionManager.GetGameSession(message.GetSessionId());

                if (gameSession != null)
                {
                    try
                    {
                        MessageProcessor.ReceiveMessage(message.PiranhaMessage, gameSession);
                    }
                    catch (Exception exception)
                    {
                        Logging.Error(typeof(ServiceMessageManager), "ServiceMessageManager::forwardPiranhaMessageReceived message decode failed, trace: " + exception);
                    }
                }
                else
                {
                    Logging.Warning(typeof(ServiceMessageManager), "ServiceMessageManager::forwardPiranhaMessageReceived session cannot exist");
                }
            }
            else
            {
                Logging.Warning(typeof(ServiceMessageManager), "ServiceMessageManager::forwardPiranhaMessageReceived piranha message is NULL");
            }
        }
    }
}