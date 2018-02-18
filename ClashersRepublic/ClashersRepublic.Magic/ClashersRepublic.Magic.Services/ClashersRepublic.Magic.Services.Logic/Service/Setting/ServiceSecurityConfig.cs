namespace ClashersRepublic.Magic.Services.Logic.Service.Setting
{
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Titan.Json;

    public static class ServiceSecurityConfig
    {
        private static bool _pepperCryptoEnabled;

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public static void LoadConfig(LogicJSONObject jsonObject)
        {
            ServiceSecurityConfig._pepperCryptoEnabled = LogicJSONHelper.GetJSONBoolean(jsonObject, "pepper_enabled");
        }

        /// <summary>
        ///     Gets if the pepper crypto is enabled.
        /// </summary>
        public static bool IsPepperCryptoEnabled()
        {
            return ServiceSecurityConfig._pepperCryptoEnabled;
        }
    }
}