namespace RivieraStudio.Magic.Services.Core.Network
{
    using RivieraStudio.Magic.Titan.Math;
    using RivieraStudio.Magic.Titan.Util;

    public static class NetManager
    {
        private static int[] _scrambler;
        
        private static LogicArrayList<NetSocket>[] _endPoints;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        public static void Initialize()
        {
            NetManager._scrambler = new int[28];
            NetManager._endPoints = new LogicArrayList<NetSocket>[28];

            for (int i = 0; i < 28; i++)
            {
                NetManager._endPoints[i] = new LogicArrayList<NetSocket>(10);
            }

            string[][] ips = ServiceSettings.GetServerIPs();

            for (int i = 0; i < ips.Length; i++)
            {
                int size = ips[i].Length;

                for (int j = 0; j < size; j++)
                {
                    NetManager.CreateSocket(ips[i][j], i);
                }
            }
        }

        /// <summary>
        ///     Starts the manager.
        /// </summary>
        public static void Start()
        {
            NetGateway.Initialize();
        }
        
        /// <summary>
        ///     Creates a new socket for the specified service node.
        /// </summary>
        private static void CreateSocket(string host, int serviceNodeType)
        {
            if (serviceNodeType > -1 && serviceNodeType < NetManager._endPoints.Length)
            {
                NetManager._endPoints[serviceNodeType].Add(new NetSocket(serviceNodeType, NetManager._endPoints[serviceNodeType].Count, host));
            }
        }
        
        /// <summary>
        ///     Gets the number of server for the specified service node.
        /// </summary>
        public static int GetServerCount(int serviceNodeType)
        {
            return NetManager._endPoints[serviceNodeType].Count;
        }

        /// <summary>
        ///     Gets the <see cref="NetSocket" /> of the specified service node.
        /// </summary>
        public static NetSocket GetServiceNodeEndPoint(int serviceNodeType, int serviceNodeId)
        {
            if (serviceNodeType > -1 && serviceNodeType < NetManager._endPoints.Length)
            {
                return NetManager._endPoints[serviceNodeType][serviceNodeId];
            }

            Logging.Warning("NetManager::getServiceNodeEndPoint serviceNodeType out of bands " + serviceNodeType + "/" + NetManager._endPoints.Length);

            return null;
        }

        /// <summary>
        ///     Gets a random <see cref="NetSocket" /> instance.
        /// </summary>
        public static NetSocket GetRandomEndPoint(int serviceNodeType)
        {
            if (serviceNodeType > -1 && serviceNodeType < NetManager._endPoints.Length)
            {
                if (NetManager._endPoints[serviceNodeType].Count != 0)
                {
                    NetSocket socket = NetManager._endPoints[serviceNodeType][NetManager._scrambler[serviceNodeType]];
                    NetManager._scrambler[serviceNodeType] = (NetManager._scrambler[serviceNodeType] + 1) % NetManager._endPoints[serviceNodeType].Count;
                    return socket;
                }
            }
            else
            {
                Logging.Warning("NetManager::getRandomEndPoint serviceNodeType out of bands " + serviceNodeType + "/" + NetManager._endPoints.Length);
            }

            return null;
        }

        /// <summary>
        ///     Gets the service node id with document id.
        /// </summary>
        public static int GetDocumentOwnerId(int serviceNodeType, LogicLong documentId)
        {
            if (serviceNodeType > -1 && serviceNodeType < NetManager._endPoints.Length)
            {
                if (documentId.GetLowerInt() > 0)
                {
                    return (documentId.GetLowerInt() - 1) % NetManager._endPoints[serviceNodeType].Count;
                }
            }
            else
            {
                Logging.Warning("NetManager::getServiceNodeId serviceNodeType out of bands " + serviceNodeType + "/" + NetManager._endPoints.Length);
            }

            return -1;
        }

        /// <summary>
        ///     Gets the document owner.
        /// </summary>
        public static NetSocket GetDocumentOwner(int serviceNodeType, LogicLong documentId)
        {
            if (serviceNodeType > -1 && serviceNodeType < NetManager._endPoints.Length)
            {
                if (documentId.GetLowerInt() > 0)
                {
                    return NetManager._endPoints[serviceNodeType][(documentId.GetLowerInt() - 1) % NetManager._endPoints[serviceNodeType].Count];
                }
            }
            else
            {
                Logging.Warning("NetManager::getServiceNodeId serviceNodeType out of bands " + serviceNodeType + "/" + NetManager._endPoints.Length);
            }

            return null;
        }
    }
}