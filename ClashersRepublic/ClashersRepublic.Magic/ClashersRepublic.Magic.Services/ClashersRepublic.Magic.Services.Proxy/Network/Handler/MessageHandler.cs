namespace ClashersRepublic.Magic.Services.Proxy.Network.Handler
{
    using System.Threading;
    using ClashersRepublic.Magic.Titan.Message;

    internal static class MessageHandler
    {
        private static Thread _sendThread;
        private static Thread _receiveThread;
        
        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            MessageHandler._sendThread = new Thread(MessageHandler.SendTask);
            MessageHandler._receiveThread = new Thread(MessageHandler.ReceiveTask);

            MessageHandler._sendThread.Start();
            MessageHandler._receiveThread.Start();
        }

        /// <summary>
        ///     Task for the receive thread.
        /// </summary>
        private static void ReceiveTask()
        {
            while (true)
            {
                NetworkToken[] connections = NetworkManager.Get();
                
                for (int i = 0; i < connections.Length; i++)
                {
                    NetworkMessaging messaging = connections[i].Messaging;

                    while (messaging.NextMessage(out PiranhaMessage message))
                    {
                        messaging.MessageManager.ReceiveMessage(message);
                    }
                }

                Thread.Sleep(1);
            }
        }

        /// <summary>
        ///     Task for the send thread.
        /// </summary>
        private static void SendTask()
        {
            while (true)
            {
                NetworkToken[] connections = NetworkManager.Get();

                for (int i = 0; i < connections.Length; i++)
                {
                    connections[i].Messaging.OnWakeup();
                }

                Thread.Sleep(1);
            }
        }
    }
}