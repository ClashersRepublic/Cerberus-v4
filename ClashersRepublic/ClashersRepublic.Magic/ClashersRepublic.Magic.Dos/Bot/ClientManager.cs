namespace ClashersRepublic.Magic.Dos.Bot
{
    using System;
    using System.Threading;
    using ClashersRepublic.Magic.Titan.Math;
    using ClashersRepublic.Magic.Titan.Util;

    internal static class ClientManager
    {
        private static LogicArrayList<Client> _clients;
        private static LogicArrayList<Thread> _threads;
        private static LogicArrayList<DateTime> _lastUpdates;

        public const int THREAD_COUNT = 4;
        public const int SLEEP_TIME = 1000;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            ClientManager._clients = new LogicArrayList<Client>(8192);
            ClientManager._threads = new LogicArrayList<Thread>(ClientManager.THREAD_COUNT);
            ClientManager._lastUpdates = new LogicArrayList<DateTime>(ClientManager.THREAD_COUNT);

            for (int i = 0; i < ClientManager.THREAD_COUNT; i++)
            {
                int threadIndex = i;

                ClientManager._threads.Add(new Thread(() =>
                {
                    while (true)
                    {
                        int startIndex = 1000 * threadIndex;
                        int endIndex = LogicMath.Min(1000 * (threadIndex + 1), ClientManager._clients.Count - startIndex);

                        ClientManager._lastUpdates[threadIndex] = DateTime.UtcNow;
                        ClientManager.Update(startIndex, endIndex);

                        Thread.Sleep(ClientManager.SLEEP_TIME);
                    }
                }));

                ClientManager._lastUpdates.Add(DateTime.UtcNow);
                ClientManager._threads[threadIndex].Start();
            }
        }

        /// <summary>
        ///     Updates clients.
        /// </summary>
        internal static void Update(int startIndex, int count)
        {
            for (int i = startIndex; i < startIndex + count; i++)
            {
                Client client = ClientManager._clients[i];

                if (client.IsConnected())
                {
                    client.MessageManager.SendKeepAliveMessage();
                }
            }
        }

        /// <summary>
        ///     Gets a connected client.
        /// </summary>
        internal static Client Get()
        {
            for (int i = 0; i < ClientManager._clients.Count; i++)
            {
                if (ClientManager._clients[i].IsConnected())
                {
                    return ClientManager._clients[i];
                }
            }

            return null;
        }
        
        /// <summary>
        ///     Creates a new client.
        /// </summary>
        internal static void Create()
        {
            Client client = new Client();
            client.ConnectToLocalServer();
            ClientManager._clients.Add(client);
        }

        /// <summary>
        ///     Sends the specified message to server.
        /// </summary>
        internal static void ForEeach(Action<Client> action)
        {
            for (int i = 0; i < ClientManager._clients.Count; i++)
            {
                action(ClientManager._clients[i]);
            }
        }
    }
}