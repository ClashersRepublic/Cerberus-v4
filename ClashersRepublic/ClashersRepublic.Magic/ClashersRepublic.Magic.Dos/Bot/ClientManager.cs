namespace ClashersRepublic.Magic.Dos.Bot
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;

    using ClashersRepublic.Magic.Titan.Util;

    internal static class ClientManager
    {
        private static object _lock;
        private static LogicArrayList<Client> _clients;
        
        public const int THREAD_COUNT = 4;
        public const int SLEEP_TIME = 1200;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            ClientManager._lock = new object();
            ClientManager._clients = new LogicArrayList<Client>(25000);

            new Thread(() =>
            {
                while (true)
                {
                    lock (ClientManager._lock)
                    {
                        LogicArrayList<int> deadClients = new LogicArrayList<int>();

                        Parallel.For(0, ClientManager._clients.Count, new ParallelOptions { MaxDegreeOfParallelism = ClientManager.THREAD_COUNT }, i =>
                        {
                            Client client = ClientManager._clients[i];

                            if (client.IsConnected())
                            {
                                client.MessageManager.Update(1f);
                            }
                            else
                            {
                                deadClients.Add(i);
                            }
                        });

                        for (int i = 0; i < deadClients.Count; i++)
                        {
                            ClientManager._clients.Remove(i);
                        }
                    }

                    Thread.Sleep(ClientManager.SLEEP_TIME);
                }
            }).Start();
        }
        
        /// <summary>
        ///     Gets a connected client.
        /// </summary>
        internal static Client Get()
        {
            lock (ClientManager._lock)
            {
                for (int i = 0; i < ClientManager._clients.Count; i++)
                {
                    if (ClientManager._clients[i].IsConnected())
                    {
                        return ClientManager._clients[i];
                    }
                }
            }

            return null;
        }

        /// <summary>
        ///     Disconnects all clients.
        /// </summary>
        internal static void DisconnectAll()
        {
            lock (ClientManager._lock)
            {
                while (ClientManager._clients.Count != 0)
                {
                    try
                    {
                        ClientManager._clients[0].Disconnect();
                    }
                    finally
                    {
                        ClientManager._clients.Remove(0);
                    }
                }
            }
        }
        
        /// <summary>
        ///     Creates a new client.
        /// </summary>
        internal static void Create()
        {
            lock (ClientManager._lock)
            {
                Client client = new Client();
                client.ConnectToLocalServer();
                ClientManager._clients.Add(client);
            }
        }

        /// <summary>
        ///     Sends the specified message to server.
        /// </summary>
        internal static void ForEeach(Action<Client> action)
        {
            lock (ClientManager._lock)
            {
                for (int i = 0; i < ClientManager._clients.Count; i++)
                {
                    action(ClientManager._clients[i]);
                }
            }
        }
    }
}