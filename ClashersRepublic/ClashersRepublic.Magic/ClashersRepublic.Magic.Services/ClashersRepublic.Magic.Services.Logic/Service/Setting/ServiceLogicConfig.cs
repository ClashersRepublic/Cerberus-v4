namespace ClashersRepublic.Magic.Services.Logic.Service.Setting
{
    using System.IO;
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Services.Logic.Resource;
    using ClashersRepublic.Magic.Titan.Json;

    public static class ServiceLogicConfig
    {
        private static string _startingHomePath;
        private static string _calandarPath;
        private static string _globalsPath;

        private static string _startingHomeData;
        private static string _calandarData;
        private static string _globalsData;

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public static void LoadConfig(LogicJSONObject jsonObject)
        {
            LogicJSONObject pathObject = jsonObject.GetJSONObject("paths");

            if (pathObject != null)
            {
                ServiceLogicConfig._startingHomePath = LogicJSONHelper.GetJSONString(pathObject, "starting_home");
                ServiceLogicConfig._calandarPath = LogicJSONHelper.GetJSONString(pathObject, "calandar");
                ServiceLogicConfig._globalsPath = LogicJSONHelper.GetJSONString(pathObject, "globals");

                if (ServiceLogicConfig._startingHomePath != null)
                {
                    ServiceLogicConfig._startingHomeData = ResourceManager.LoadAssetFile(ServiceLogicConfig._startingHomePath);
                }

                if (ServiceLogicConfig._calandarPath != null)
                {
                    ServiceLogicConfig._calandarData = ResourceManager.LoadAssetFile(ServiceLogicConfig._calandarPath);
                }

                if (ServiceLogicConfig._globalsPath != null)
                {
                    ServiceLogicConfig._globalsData = ResourceManager.LoadAssetFile(ServiceLogicConfig._globalsPath);
                }
            }
        }

        /// <summary>
        ///     Gets the starting home path.
        /// </summary>
        public static string GetStartingHomePath()
        {
            return ServiceLogicConfig._startingHomePath;
        }

        /// <summary>
        ///     Gets the starting home json.
        /// </summary>
        public static string GetStartingHomeJson()
        {
            return ServiceLogicConfig._startingHomeData;
        }

        /// <summary>
        ///     Gets the calandar path.
        /// </summary>
        public static string GetCalandarPath()
        {
            return ServiceLogicConfig._calandarPath;
        }

        /// <summary>
        ///     Gets the calandar json.
        /// </summary>
        public static string GetCalandarJson()
        {
            return ServiceLogicConfig._calandarData;
        }

        /// <summary>
        ///     Gets the globals path.
        /// </summary>
        public static string GetGlobalsPath()
        {
            return ServiceLogicConfig._globalsPath;
        }

        /// <summary>
        ///     Gets the globals json.
        /// </summary>
        public static string GetGlobalsJson()
        {
            return ServiceLogicConfig._globalsData;
        }
    }
}