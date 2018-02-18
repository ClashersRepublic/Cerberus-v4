namespace ClashersRepublic.Magic.Services.Logic.Service
{
    public static class ServiceHelper
    {
        /// <summary>
        ///     Gets the service host port.
        /// </summary>
        public static int GetServiceHostPort(int serviceType)
        {
            switch (serviceType)
            {
                case 1: return 5000;
                case 6: return 5001;
                case 9: return 5002;
                case 10: return 5003;
                case 11: return 5004;
                case 13: return 5005;
                case 27: return 5006;
                default: return 0;
            }
        }

        /// <summary>
        ///     Gets the service type name.
        /// </summary>
        public static string GetServiceTypeName(int serviceType)
        {
            switch (serviceType)
            {
                case 1: return "proxy";
                case 6: return "global_chat";
                case 9: return "avatar";
                case 10: return "home";
                case 11: return "alliance";
                case 13: return "league";
                case 27: return "battle";
                default: return string.Empty;
            }
        }
    }
}