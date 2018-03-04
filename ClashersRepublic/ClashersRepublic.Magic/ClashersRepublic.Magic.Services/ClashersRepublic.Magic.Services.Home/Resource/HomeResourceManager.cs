namespace ClashersRepublic.Magic.Services.Home.Resource
{
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Services.Core;
    using ClashersRepublic.Magic.Services.Core.Web;
    using ClashersRepublic.Magic.Titan.Json;

    internal static class HomeResourceManager
    {
        private static string _startingHomeJSON;
        private static string _calendarJSON;
        private static string _globalJSON;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            HomeResourceManager.LoadConfig();
        }

        /// <summary>
        ///     Laods the config file.
        /// </summary>
        private static void LoadConfig()
        {
            string json = WebManager.DownloadString("https://raw.githubusercontent.com/Mimi8298/services/master/s/home.json");

            if (json != null)
            {
                LogicJSONObject jsonObject = (LogicJSONObject) LogicJSONParser.Parse(json);

                LogicJSONObject logicObject = jsonObject.GetJSONObject("logic");

                if (logicObject != null)
                {
                    LogicJSONObject pathObject = logicObject.GetJSONObject("paths");

                    if (pathObject != null)
                    {
                        HomeResourceManager._startingHomeJSON = ResourceManager.LoadAssetFile(LogicJSONHelper.GetJSONString(pathObject, "starting_home"));
                        HomeResourceManager._calendarJSON = ResourceManager.LoadAssetFile(LogicJSONHelper.GetJSONString(pathObject, "calendar"));
                        HomeResourceManager._globalJSON = ResourceManager.LoadAssetFile(LogicJSONHelper.GetJSONString(pathObject, "globals"));
                    }
                }
            }
        }

        /// <summary>
        ///     Gets the starting home json.
        /// </summary>
        internal static string GetStartingHomeJSON()
        {
            return HomeResourceManager._startingHomeJSON;
        }

        /// <summary>
        ///     Gets the calendar json.
        /// </summary>
        internal static string GetCalendarJSON()
        {
            return HomeResourceManager._calendarJSON;
        }

        /// <summary>
        ///     Gets the global json.
        /// </summary>
        internal static string GetGlobalJSON()
        {
            return HomeResourceManager._globalJSON;
        }
    }
}