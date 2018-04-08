namespace RivieraStudio.Magic.Services.Zone.Resource
{
    using RivieraStudio.Magic.Logic.Data;
    using RivieraStudio.Magic.Logic.Helper;
    using RivieraStudio.Magic.Services.Core;
    using RivieraStudio.Magic.Services.Core.Web;
    using RivieraStudio.Magic.Titan.Json;
    using RivieraStudio.Magic.Titan.Util;

    internal static class HomeResourceManager
    {
        private static string _startingHomeJSON;
        private static string _calendarJSON;
        private static string _globalJSON;

        private static LogicArrayList<string> _npcLevels;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            HomeResourceManager._npcLevels = new LogicArrayList<string>();
            LogicDataTable npcTable = LogicDataTables.GetTable(16);

            for (int i = 0; i < npcTable.GetItemCount(); i++)
            {
                LogicNpcData npcData = (LogicNpcData) npcTable.GetItemAt(i);

                if (npcData != null)
                {
                    string level = ResourceManager.LoadAssetFile(npcData.LevelFile);

                    if (level == null)
                    {
                        Logging.Error("HomeResourceManager::init pLevel is NULL");
                    }

                    HomeResourceManager._npcLevels.Add(level);
                }
            }

            HomeResourceManager.LoadConfig();
        }

        /// <summary>
        ///     Laods the config file.
        /// </summary>
        private static void LoadConfig()
        {
            string json = WebManager.DownloadConfigString("/core/conf/home.json");

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

        /// <summary>
        ///     Gets the npc level.
        /// </summary>
        internal static string GetNpcLevel(int idx)
        {
            return HomeResourceManager._npcLevels[idx];
        }

        /// <summary>
        ///     Gets the npc level by data.
        /// </summary>
        internal static string GetNpcLevelByData(LogicData data)
        {
            return HomeResourceManager._npcLevels[data.GetInstanceID()];
        }
    }
}