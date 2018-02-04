namespace ClashersRepublic.Magic.Client
{
    using System;
    using System.Threading;
    using ClashersRepublic.Magic.Client.Game;
    using ClashersRepublic.Magic.Titan.Math;
    using ClashersRepublic.Magic.Titan.Util;

    internal static class ClientManager
    {
        private static LogicArrayList<GameMain> _clients;
        private static LogicArrayList<Thread> _threads;
        private static LogicArrayList<DateTime> _lastUpdates;

        public const int THREAD_COUNT = 4;
        public const int SLEEP_TIME = 20;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            ClientManager._clients = new LogicArrayList<GameMain>(4096);
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

                        float time = (float) DateTime.UtcNow.Subtract(ClientManager._lastUpdates[threadIndex]).TotalSeconds;

                        ClientManager.Update(time, startIndex, endIndex);
                        ClientManager._lastUpdates[threadIndex] = DateTime.UtcNow;

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
        private static void Update(float deltaTime, int startIndex, int cnt)
        {
            for (int i = startIndex; i < cnt; i++)
            {
                ClientManager._clients[i].Update(deltaTime);
            }
        }

        /// <summary>
        ///     Creates a new client.
        /// </summary>
        internal static void Create()
        {
            ClientManager._clients.Add(new GameMain());
        }

        /// <summary>
        ///     Sends the specified message to server.
        /// </summary>
        internal static void ForEeach(Action<GameMain> action)
        {
            for (int i = 0; i < ClientManager._clients.Count; i++)
            {
                action(ClientManager._clients[i]);
            }
        }
    }
}