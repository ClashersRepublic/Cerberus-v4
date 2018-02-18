namespace ClashersRepublic.Magic.Services.Home.Service
{
    using System.Net;

    using ClashersRepublic.Magic.Services.Home.Debug;
    using ClashersRepublic.Magic.Services.Logic;
    using ClashersRepublic.Magic.Services.Logic.Service;

    using ClashersRepublic.Magic.Titan.Util;

    using NetMQ;
    using NetMQ.Sockets;

    internal static class ServiceManager
    {
        private static ServiceGateway _gateway;
        private static LogicArrayList<NetMQSocket>[] _services;

        public const int SERVICE_TYPE = 10;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            ServiceManager._services = new LogicArrayList<NetMQSocket>[28];

            for (int i = 0; i < 28; i++)
            {
                ServiceManager._services[i] = new LogicArrayList<NetMQSocket>(27);
            }

            LogicArrayList<string> proxyHosts = ServiceConfig.GetServiceHosts(1);
            LogicArrayList<string> globalChatHosts = ServiceConfig.GetServiceHosts(6);
            LogicArrayList<string> avatarHosts = ServiceConfig.GetServiceHosts(9);
            LogicArrayList<string> homeHosts = ServiceConfig.GetServiceHosts(10);
            LogicArrayList<string> allianceHosts = ServiceConfig.GetServiceHosts(11);
            LogicArrayList<string> leagueHosts = ServiceConfig.GetServiceHosts(13);
            LogicArrayList<string> battleHosts = ServiceConfig.GetServiceHosts(27);

            if (proxyHosts != null)
            {
                for (int i = 0; i < proxyHosts.Count; i++)
                {
                    ServiceManager.Connect(1, proxyHosts[i]);
                }
            }

            if (globalChatHosts != null)
            {
                for (int i = 0; i < globalChatHosts.Count; i++)
                {
                    ServiceManager.Connect(6, globalChatHosts[i]);
                }
            }

            if (avatarHosts != null)
            {
                for (int i = 0; i < avatarHosts.Count; i++)
                {
                    ServiceManager.Connect(9, avatarHosts[i]);
                }
            }

            if (homeHosts != null)
            {
                for (int i = 0; i < homeHosts.Count; i++)
                {
                    ServiceManager.Connect(10, homeHosts[i]);
                }
            }

            if (allianceHosts != null)
            {
                for (int i = 0; i < allianceHosts.Count; i++)
                {
                    ServiceManager.Connect(11, allianceHosts[i]);
                }
            }

            if (leagueHosts != null)
            {
                for (int i = 0; i < leagueHosts.Count; i++)
                {
                    ServiceManager.Connect(13, leagueHosts[i]);
                }
            }

            if (battleHosts != null)
            {
                for (int i = 0; i < battleHosts.Count; i++)
                {
                    ServiceManager.Connect(27, battleHosts[i]);
                }
            }

            using (WebClient www = new WebClient())
            {
                ServiceConfig.LoadServiceConfig(www.DownloadString(Config.ConfigServer + ServiceHelper.GetServiceTypeName(ServiceManager.SERVICE_TYPE) + ".json"));
            }

            ServiceManager._gateway = new ServiceGateway(ServiceHelper.GetServiceHostPort(ServiceManager.SERVICE_TYPE));
        }
        
        /// <summary>
        ///     Connects to the specified service node.
        /// </summary>
        private static void Connect(int serviceType, string host)
        {
            int servicePort = ServiceHelper.GetServiceHostPort(serviceType);

            if (servicePort != 0)
            {
                ServiceManager._services[serviceType].Add(new DealerSocket(">tcp://" + host + ":" + servicePort));
            }
            else
            {
                Logging.Warning(typeof(ServiceManager), "ServiceManager::connect invalid service type, type: " + serviceType);
            }
        }
        
        /// <summary>
        ///     Gets the service socket.
        /// </summary>
        public static NetMQSocket GetServiceSocket(int serviceType, int serverId)
        {
            if (serviceType >= 0 && serviceType < 28)
            {
                return ServiceManager._services[serviceType][serverId];
            }
            
            Logging.Error(typeof(ServiceManager), "ServiceManager::getServiceSocket index are out of bands " + serviceType + "/" + 28);

            return null;
        }
    }
}