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
            string json = WebManager.DownloadFileFromConfigServer("/core/conf/home.json");

            if (json != null)
            {
                LogicJSONObject jsonObject = (LogicJSONObject) LogicJSONParser.Parse(json);

                LogicJSONObject resObject = jsonObject.GetJSONObject("res");

                if (resObject != null)
                {
                    HomeResourceManager._startingHomeJSON = ResourceManager.LoadAssetFile(LogicJSONHelper.GetJSONString(resObject, "starting_home"));
                    HomeResourceManager._calendarJSON = ResourceManager.LoadAssetFile(LogicJSONHelper.GetJSONString(resObject, "calendar"));
                    HomeResourceManager._globalJSON = ResourceManager.LoadAssetFile(LogicJSONHelper.GetJSONString(resObject, "globals"));
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