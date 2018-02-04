namespace ClashersRepublic.Magic.Services.Logic
{
    public static class ServiceExchangeName
    {
        public const string SERVICE_PROXY_NAME = "proxy";
        public const string SERVICE_ALLIANCE_NAME = "alliance";
        public const string SERVICE_HOME_NAME = "home";
        public const string SERVICE_AVATAR_NAME = "avatar";
        public const string SERVICE_GLOBAL_CHAT_NAME = "avatar";
        
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
    }
}