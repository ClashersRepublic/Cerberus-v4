namespace ClashersRepublic.Magic.Proxy.Logic
{
    using System.Collections.Concurrent;
    using System.Threading;
    using ClashersRepublic.Magic.Services.Logic;
    using ClashersRepublic.Magic.Services.Logic.Util;

    internal static class ClientManager
    {
        private static bool _initialized;
        private static long _lastConnectionId;
        private static ConcurrentDictionary<string, Client> _clients;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            if (ClientManager._initialized)
            {
                return;
            }

            ClientManager._initialized = true;
            ClientManager._clients = new ConcurrentDictionary<string, Client>();
        }

        /// <summary>
        ///     Adds the specified client.
        /// </summary>
        internal static void AddClient(Client client)
        {
            client.SessionId = SessionUtil.CreateSessionId(Config.ServerId, Interlocked.Increment(ref ClientManager._lastConnectionId));

            if (!ClientManager._clients.TryAdd(client.SessionId, client))
            {
                Logging.Error(typeof(ClientManager), "AddClient client with same session id already added");
            }
        }

        /// <summary>
        ///     Removes the specified client.
        /// </summary>
        internal static void RemoveClient(Client client)
        {
            if (!ClientManager._clients.TryRemove(client.SessionId, out _))
            {
                Logging.Error(typeof(ClientManager), "RemoveClient client already removed");
            }
        }

        /// <summary>
        ///     Gets a client with session id.
        /// </summary>
        internal static Client GetClient(string sessionId)
        {
            ClientManager._clients.TryGetValue(sessionId, out Client client);
            return client;
        }

        /// <summary>
        ///     Gets a client with session id.
        /// </summary>
        internal static bool GetClient(string sessionId, out Client client)
        {
            return ClientManager._clients.TryGetValue(sessionId, out client);
        }
    }
}