namespace ClashersRepublic.Magic.Services.Logic
{
    public class ServiceExchangeName
    {
        public const string PROXY_EXCHANGE = "magic.int.services.proxy";
        public const string HOME_EXCHANGE = "magic.int.services.home";

        public const string START_PROXY_QUEUE_NAME = "magic.int.services.proxy.queue_";
        public const string START_HOME_QUEUE_NAME = "magic.int.services.home.queue_";

        public const string START_PROXY_ROUTING_KEY_NAME = "magic.int.services.rk.proxy_";
        public const string START_HOME_ROUTING_KEY_NAME = "magic.int.services.rk.proxy_";

        public const string PROXY_COMMON_QUEUE = "magic.int.services.com.proxy";
        public const string HOME_COMMON_QUEUE = "magic.int.services.com.home";
    }
}