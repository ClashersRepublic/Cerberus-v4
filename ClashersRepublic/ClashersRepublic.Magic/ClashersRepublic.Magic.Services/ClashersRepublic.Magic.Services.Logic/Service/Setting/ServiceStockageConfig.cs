namespace ClashersRepublic.Magic.Services.Logic.Service.Setting
{
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Titan.Json;

    public static class ServiceStockageConfig
    {
        private static bool _preloadData;

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public static void LoadConfig(LogicJSONObject jsonObject)
        {
            ServiceStockageConfig._preloadData = LogicJSONHelper.GetJSONBoolean(jsonObject, "preload");
        }

        /// <summary>
        ///     Gets if preload data is enabled.
        /// </summary>
        public static bool PreloadEnabled()
        {
            return ServiceStockageConfig._preloadData;
        }
    }
}