namespace ClashersRepublic.Magic.Services.Logic
{
    public static class ServiceExchangeName
    {
        public const string SERVICE_PROXY_NAME = "proxy";
        public const string SERVICE_ALLIANCE_NAME = "alliance";
        public const string SERVICE_HOME_NAME = "home";
        public const string SERVICE_AVATAR_NAME = "avatar";
        public const string SERVICE_GLOBAL_CHAT_NAME = "gc";
        public const string SERVICE_LEAGUE_NAME = "league";
        public const string SERVICE_BATTLE_NAME = "battle";
        
        public const string SERVICE_CODE_NAME = "magic";
        public const string SERVICE_EXCHANGE_PREFIX = "ex";
        public const string SERVICE_QUEUE_PREFIX = "q";

        /// <summary>
        ///     Builds a name of queue.
        /// </summary>
        public static string BuildQueueName(string serviceType, int serverId)
        {
            return ServiceExchangeName.SERVICE_CODE_NAME + "." +
                   ServiceExchangeName.SERVICE_QUEUE_PREFIX + "." +
                   serviceType + "." + serverId;
        }

        /// <summary>
        ///     Builds a name of exchange.
        /// </summary>
        public static string BuildExchangeName(string serviceType)
        {
            return ServiceExchangeName.SERVICE_CODE_NAME + "." +
                   ServiceExchangeName.SERVICE_EXCHANGE_PREFIX + "." +
                   serviceType;
        }

        /// <summary>
        ///     Gets the service name with service node type.
        /// </summary>
        public static string ServiceNodeTypeToServiceName(int serviceNodeType)
        {
            string serviceName = null;

            switch (serviceNodeType)
            {
                case 1:
                {
                    serviceName = ServiceExchangeName.SERVICE_PROXY_NAME;
                    break;
                }

                case 6:
                {
                    serviceName = ServiceExchangeName.SERVICE_GLOBAL_CHAT_NAME;
                    break;
                }

                case 9:
                {
                    serviceName = ServiceExchangeName.SERVICE_AVATAR_NAME;
                    break;
                }

                case 10:
                {
                    serviceName = ServiceExchangeName.SERVICE_HOME_NAME;
                    break;
                }

                case 11:
                {
                    serviceName = ServiceExchangeName.SERVICE_ALLIANCE_NAME;
                    break;
                }

                case 13:
                {
                    serviceName = ServiceExchangeName.SERVICE_LEAGUE_NAME;
                    break;
                }

                case 27:
                {
                    serviceName = ServiceExchangeName.SERVICE_BATTLE_NAME;
                    break;
                }
            }

            return serviceName;
        }
    }
}