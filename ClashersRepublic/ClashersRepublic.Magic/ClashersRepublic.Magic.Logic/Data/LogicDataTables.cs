namespace ClashersRepublic.Magic.Logic.Data
{
    using System.IO;
    using ClashersRepublic.Magic.Titan.CSV;
    using ClashersRepublic.Magic.Titan.Debug;

    public class LogicDataTables
    {
        private static LogicDataTable[] _dataTables;
        private static LogicGlobals _globals;
        private static LogicClientGlobals _clientGlobals;

        private static LogicResourceData _diamondsData;
        private static LogicResourceData _goldData;
        private static LogicResourceData _elixirData;
        private static LogicResourceData _darkElixirData;
        private static LogicResourceData _gold2Data;
        private static LogicResourceData _elixir2Data;

        private static LogicBuildingData _townHallData;
        private static LogicBuildingData _townHallVillage2Data;
        private static LogicBuildingData _allianceCastleData;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        public static void Initialize()
        {
            LogicDataTables._dataTables = new LogicDataTable[44];

            LogicDataTables.Load("logic/buildings.csv", 0);
            LogicDataTables.Load("logic/locales.csv", 1);
            LogicDataTables.Load("logic/resources.csv", 2);
            LogicDataTables.Load("logic/characters.csv", 3);
            // LogicDataTables.Load("csv/animations.csv", 4);
            LogicDataTables.Load("logic/projectiles.csv", 5);
            LogicDataTables.Load("logic/building_classes.csv", 6);
            LogicDataTables.Load("logic/obstacles.csv", 7);
            LogicDataTables.Load("logic/effects.csv", 8);
            LogicDataTables.Load("csv/particle_emitters.csv", 9);
            LogicDataTables.Load("logic/experience_levels.csv", 10);
            LogicDataTables.Load("logic/traps.csv", 11);
            LogicDataTables.Load("logic/alliance_badges.csv", 12);
            LogicDataTables.Load("logic/globals.csv", 13);
            LogicDataTables.Load("logic/townhall_levels.csv", 14);
            LogicDataTables.Load("logic/alliance_portal.csv", 15);
            LogicDataTables.Load("logic/npcs.csv", 16);
            LogicDataTables.Load("logic/decos.csv", 17);
            LogicDataTables.Load("csv/resource_packs.csv", 18);
            LogicDataTables.Load("logic/shields.csv", 19);
            LogicDataTables.Load("logic/missions.csv", 20);
            LogicDataTables.Load("csv/billing_packages.csv", 21);
            LogicDataTables.Load("logic/achievements.csv", 22);
            // LogicDataTables.Load("csv/credits.csv", 23);
            // LogicDataTables.Load("csv/faq.csv", 24);
            LogicDataTables.Load("logic/spells.csv", 25);
            LogicDataTables.Load("csv/hints.csv", 26);
            LogicDataTables.Load("logic/heroes.csv", 27);
            LogicDataTables.Load("logic/leagues.csv", 28);
            LogicDataTables.Load("csv/news.csv", 29);
            LogicDataTables.Load("logic/war.csv", 30);
            LogicDataTables.Load("logic/regions.csv", 31);
            LogicDataTables.Load("csv/client_globals.csv", 32);
            LogicDataTables.Load("logic/alliance_badge_layers.csv", 33);
            LogicDataTables.Load("logic/alliance_levels.csv", 34);
            // LogicDataTables.Load("csv/helpshift.csv", 35);
            LogicDataTables.Load("logic/variables.csv", 36);
            LogicDataTables.Load("logic/gem_bundles.csv", 37);
            LogicDataTables.Load("logic/village_objects.csv", 38);
            LogicDataTables.Load("logic/calendar_event_functions.csv", 39);
            LogicDataTables.Load("csv/boombox.csv", 40);
            LogicDataTables.Load("csv/event_entries.csv", 41);
            LogicDataTables.Load("csv/deeplinks.csv", 42);
            LogicDataTables.Load("logic/leagues2.csv", 43);

            LogicDataTables._globals = new LogicGlobals();
            LogicDataTables._clientGlobals = new LogicClientGlobals();

            for (int i = 0; i < LogicDataTables._dataTables.Length; i++)
            {
                if (LogicDataTables._dataTables[i] != null)
                {
                    LogicDataTables._dataTables[i].CreateReferences();
                }
            }

            LogicDataTables._diamondsData = LogicDataTables.GetResourceByName("Diamonds");
            LogicDataTables._goldData = LogicDataTables.GetResourceByName("Gold");
            LogicDataTables._elixirData = LogicDataTables.GetResourceByName("Elixir");
            LogicDataTables._darkElixirData = LogicDataTables.GetResourceByName("DarkElixir");
            LogicDataTables._gold2Data = LogicDataTables.GetResourceByName("Gold2");
            LogicDataTables._elixir2Data = LogicDataTables.GetResourceByName("Elixir2");

            LogicDataTable buildingDataTable = LogicDataTables._dataTables[0];

            for (int i = 0; i < buildingDataTable.GetItemCount(); i++)
            {
                LogicBuildingData buildingData = (LogicBuildingData) buildingDataTable.GetItemAt(i);

                if (buildingData.IsAllianceCastle())
                {
                    LogicDataTables._allianceCastleData = buildingData;
                }

                if (buildingData.IsTownHall())
                {
                    LogicDataTables._townHallData = buildingData;
                }
            }

            LogicDataTables._globals.CreateReferences();
            LogicDataTables._clientGlobals.CreateReferences();
        }

        /// <summary>
        ///     Gets a value indicating whether the specified data table can be reloaded.
        /// </summary>
        public static bool CanReloadTable(LogicDataTable table)
        {
            return true;
        }

        /// <summary>
        ///     Gets the specified table index
        /// </summary>
        public static LogicDataTable GetTable(int tableIndex)
        {
            return LogicDataTables._dataTables[tableIndex];
        }

        /// <summary>
        ///     Gets a data reference by id.
        /// </summary>
        public static LogicData GetDataById(int globalId)
        {
            int tableIndex = GlobalID.GetClassID(globalId) - 1;

            if (tableIndex >= 0 && tableIndex <= 43 && LogicDataTables._dataTables[tableIndex] != null)
            {
                return LogicDataTables._dataTables[tableIndex].GetItemById(globalId);
            }

            return null;
        }

        /// <summary>
        ///     Gets the data by instance.
        /// </summary>
        public static LogicData GetDataByName(string name)
        {
            for (int i = 0; i < LogicDataTables._dataTables.Length; i++)
            {
                if (LogicDataTables._dataTables[i] != null)
                {
                    LogicData data = LogicDataTables._dataTables[i].GetDataByName(name);

                    if (data != null)
                    {
                        return data;
                    }
                }
            }

            return null;
        }

        /// <summary>
        ///     Gets the town hall level data by index.
        /// </summary>
        public static LogicTownhallLevelData GetTownHallLevel(int levelIndex)
        {
            if (levelIndex > -1)
            {
                if (levelIndex < LogicDataTables._dataTables[14].GetItemCount())
                {
                    return (LogicTownhallLevelData) LogicDataTables._dataTables[14].GetItemAt(levelIndex);
                }
            }

            Debugger.Error("LogicDataTables::getTownHallLevel parameter out of bounds");

            return null;
        }

        /// <summary>
        ///     Gets the resource data by name.
        /// </summary>
        public static LogicResourceData GetResourceByName(string name)
        {
            return (LogicResourceData) LogicDataTables._dataTables[2].GetDataByName(name);
        }

        /// <summary>
        ///     Gets the client global data by name.
        /// </summary>
        public static LogicGlobalData GetClientGlobalByName(string name)
        {
            return (LogicGlobalData) LogicDataTables._dataTables[32].GetDataByName(name);
        }

        /// <summary>
        ///     Gets the global data by name.
        /// </summary>
        public static LogicGlobalData GetGlobalByName(string name)
        {
            return (LogicGlobalData)LogicDataTables._dataTables[13].GetDataByName(name);
        }

        /// <summary>
        ///     Gets the building class data by name.
        /// </summary>
        public static LogicBuildingClassData GetBuildingClassData(string name)
        {
            return (LogicBuildingClassData) LogicDataTables._dataTables[6].GetDataByName(name);
        }

        /// <summary>
        ///     Gets the globals instance.
        /// </summary>
        public static LogicClientGlobals GetClientGlobals()
        {
            return LogicDataTables._clientGlobals;
        }

        /// <summary>
        ///     Gets the globals instance.
        /// </summary>
        public static LogicGlobals GetGlobals()
        {
            return LogicDataTables._globals;
        }

        /// <summary>
        ///     Gets the alliance castle data.
        /// </summary>
        public static LogicBuildingData GetAllianceCastleData()
        {
            return LogicDataTables._allianceCastleData;
        }

        /// <summary>
        ///     Gets the diamonds data instance.
        /// </summary>
        public static LogicResourceData GetDiamondsData()
        {
            return LogicDataTables._diamondsData;
        }

        /// <summary>
        ///     Gets the gold data instance.
        /// </summary>
        public static LogicResourceData GetGoldData()
        {
            return LogicDataTables._goldData;
        }

        /// <summary>
        ///     Gets the elixir data instance.
        /// </summary>
        public static LogicResourceData GetElixirData()
        {
            return LogicDataTables._elixirData;
        }

        /// <summary>
        ///     Gets the dark elixir data instance.
        /// </summary>
        public static LogicResourceData GetDarkElixirData()
        {
            return LogicDataTables._darkElixirData;
        }

        /// <summary>
        ///     Gets the gold2 data instance.
        /// </summary>
        public static LogicResourceData GetGold2Data()
        {
            return LogicDataTables._gold2Data;
        }

        /// <summary>
        ///     Gets the elixir2 data instance.
        /// </summary>
        public static LogicResourceData GetElixir2Data()
        {
            return LogicDataTables._elixir2Data;
        }

        /// <summary>
        ///     Loads the specified csv file.
        /// </summary>
        public static void Load(string path, int tableIndex)
        {
            if (File.Exists("Assets/" + path))
            {
                string[] lines = File.ReadAllLines("Assets/" + path);

                if (lines.Length > 1)
                {
                    LogicDataTables.LoadTable(new CSVNode(lines, path).GetTable(), tableIndex);
                }
            }
            else
            {
                Debugger.Warning("LogicDataTables::load file " + path + " not exist");
            }
        }

        public static void LoadTable(CSVTable table, int tableIndex)
        {
            LogicDataTables._dataTables[tableIndex] = new LogicDataTable(table, tableIndex);
        }
    }
}