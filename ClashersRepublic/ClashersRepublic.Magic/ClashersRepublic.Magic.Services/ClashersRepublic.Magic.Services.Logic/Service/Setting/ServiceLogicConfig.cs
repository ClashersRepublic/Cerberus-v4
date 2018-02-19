namespace ClashersRepublic.Magic.Services.Logic.Service.Setting
{
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Services.Logic.Resource;
    using ClashersRepublic.Magic.Titan.Json;

    public static class ServiceLogicConfig
    {
        private static string _startingHomePath;
        private static string _calendarPath;
        private static string _globalsPath;

        private static string _startingHomeData;
        private static string _calendarData;
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
                ServiceLogicConfig._calendarPath = LogicJSONHelper.GetJSONString(pathObject, "calendar");
                ServiceLogicConfig._globalsPath = LogicJSONHelper.GetJSONString(pathObject, "globals");

                if (ServiceLogicConfig._startingHomePath != null)
                {
                    ServiceLogicConfig._startingHomeData = ResourceManager.LoadAssetFile(ServiceLogicConfig._startingHomePath);
                }

                if (ServiceLogicConfig._calendarPath != null)
                {
                    ServiceLogicConfig._calendarData = ResourceManager.LoadAssetFile(ServiceLogicConfig._calendarPath);
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
        ///     Gets the calendar path.
        /// </summary>
        public static string GetCalendarPath()
        {
            return ServiceLogicConfig._calendarPath;
        }

        /// <summary>
        ///     Gets the calendar json.
        /// </summary>
        public static string GetCalendarJson()
        {
            return ServiceLogicConfig._calendarData;
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