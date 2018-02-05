namespace ClashersRepublic.Magic.Services.Home.Player
{
    using System;
    using System.Collections.Concurrent;
    using ClashersRepublic.Magic.Logic.Avatar;
    using ClashersRepublic.Magic.Logic.Home;
    using ClashersRepublic.Magic.Logic.Level;
    using ClashersRepublic.Magic.Logic.Message.Home;
    using ClashersRepublic.Magic.Logic.Mode;
    using ClashersRepublic.Magic.Services.Home.Database;
    using ClashersRepublic.Magic.Services.Home.Debug;
    using ClashersRepublic.Magic.Services.Home.Service;
    using ClashersRepublic.Magic.Services.Home.Session;
    using ClashersRepublic.Magic.Services.Logic;
    using ClashersRepublic.Magic.Services.Logic.Message.Session;

    using ClashersRepublic.Magic.Titan.Math;
    using ClashersRepublic.Magic.Titan.Util;

    internal static class GamePlayerManager
    {
        private static ConcurrentDictionary<long, GamePlayer> _players;
        private static ConcurrentDictionary<string, GameSession> _sessions;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            GamePlayerManager._players = new ConcurrentDictionary<long, GamePlayer>();
            GamePlayerManager._sessions = new ConcurrentDictionary<string, GameSession>();

            GamePlayerManager.LoadAllPlayers();
        }

        /// <summary>
        ///     Loads all players from database.
        /// </summary>
        internal static void LoadAllPlayers()
        {
            int lastPlayerId = GameDatabase.GetHigherAccountId(Config.ServerId);

            for (int lowId = 1, highId = Config.ServerId; lowId <= lastPlayerId; lowId++)
            {
                GamePlayer player = GamePlayerManager.LoadPlayer(new LogicLong(highId, lowId));

                if (player == null)
                {
                    GamePlayerManager.CreatePlayer(new LogicLong(highId, lowId));
                }
            }
        }

        /// <summary>
        ///     Loads the specified player from database.
        /// </summary>
        internal static GamePlayer LoadPlayer(LogicLong accountId)
        {
            GamePlayer player = GameDatabase.LoadPlayer(accountId);

            if (player != null)
            {
                player.LoadFromDocuments();

                if (!GamePlayerManager._players.TryAdd(accountId, player))
                {
                    Logging.Error(typeof(GamePlayerManager), "GamePlayerManager::loadPlayer account already added");
                }
            }

            return player;
        }

        /// <summary>
        ///     Called when a client connects to the server
        /// </summary>
        internal static void OnClientConnected(int serverId, string sessionId, LogicLong accountId, bool isNewClient)
        {
            if (!GamePlayerManager._players.TryGetValue(accountId, out GamePlayer player))
            {
                if (isNewClient)
                {
                    player = GamePlayerManager.CreatePlayer(accountId);

                    if (player == null)
                    {
                        Logging.Error(typeof(GamePlayerManager), "GamePlayerManager::onClientConnected account creation error");
                    }
                }
                else
                {
                    Logging.Error(typeof(GamePlayerManager), "GamePlayerManager::onClientConnected player id " + accountId + " not exist");
                }
            }

            if (player != null)
            {
                if (GamePlayerManager._sessions.TryAdd(sessionId, new GameSession(serverId, sessionId, player)))
                {
                    GamePlayerManager.SendGameData(sessionId);
                }
            }
        }

        /// <summary>
        ///     Called when the client is disconnected from server.
        /// </summary>
        internal static void OnClientDisconnected(string sessionId)
        {
            if (GamePlayerManager._sessions.TryRemove(sessionId, out GameSession session))
            {
                GamePlayerManager.OnSessionRemoved(session);
            }
        }

        /// <summary>
        ///     Called when a session has been removed.
        /// </summary>
        internal static void OnSessionRemoved(GameSession session)
        {
            GameDatabase.SaveAccount(session.Player);
        }

        /// <summary>
        ///     Forces the closure of the session.
        /// </summary>
        internal static void CloseSession(string sessionId)
        {
            if (GamePlayerManager._sessions.TryRemove(sessionId, out GameSession session))
            {
                GamePlayerManager.OnSessionRemoved(session);
                ServiceMessageManager.SendMessage(new SessionClosedMessage(), ServiceExchangeName.SERVICE_PROXY_NAME, session.ServerId, session.SessionId);
            }
        }

        /// <summary>
        ///     Gets a session by id.
        /// </summary>
        internal static GameSession GetSession(string sessionId)
        {
            return GamePlayerManager._sessions.TryGetValue(sessionId, out GameSession session) ? session : null;
        }

        /// <summary>
        ///     Gets a session by id.
        /// </summary>
        internal static bool GetSession(string sessionId, out GameSession session)
        {
            return GamePlayerManager._sessions.TryGetValue(sessionId, out session);
        }

        /// <summary>
        ///     Creates a new player.
        /// </summary>
        internal static GamePlayer CreatePlayer(LogicLong accountId)
        {
            if (!accountId.IsZero())
            {
                if (accountId.GetHigherInt() == Config.ServerId)
                {
                    GamePlayer player = new GamePlayer(accountId);
                    
                    if (GamePlayerManager._players.TryAdd(accountId, player))
                    {
                        GameDatabase.InsertPlayer(player);
                        return player;
                    }

                    Logging.Error(typeof(GamePlayerManager), "GamePlayerManager::createPlayer player already exist");
                }
                else
                {
                    Logging.Error(typeof(GamePlayerManager), "GamePlayerManager::createPlayer invalid high id");
                }
            }
            else
            {
                Logging.Error(typeof(GamePlayerManager), "GamePlayerManager::createPlayer account id is empty");
            }

            return null;
        }

        /// <summary>
        ///     Sends the game data to client.
        /// </summary>
        internal static void SendGameData(string sessionId)
        {
            if (GamePlayerManager._sessions.TryGetValue(sessionId, out GameSession session))
            {
                GamePlayerManager.LoadHomeState(session);

                if (session.Player.LogicClientAvatar.IsInAlliance())
                {
                    //  Send request to Alliance node
                }
            }
        }

        /// <summary>
        ///     Loads the home state.
        /// </summary>
        internal static void LoadHomeState(GameSession session)
        {
            GamePlayer player = session.Player;
            LogicGameMode gameMode = session.LogicGameMode;

            int timestamp = LogicTimeUtil.GetTimestamp();
            int secondsSinceLastSave = timestamp - player.LastSaveTime;

            LogicClientHome clientHome = player.LogicClientHome;
            LogicClientAvatar clientAvatar = player.LogicClientAvatar;

            clientHome.SetGlobalJSON("{\"Globals\":{\"DuelBonusMaxDiamondCostPercent\":100,\"GiftPackExtension\":\" \",\"DuelBonusPercentDraw\":0,\"DuelBonusLimitWinsPerDay\":3,\"DuelLootLimitCooldownInMinutes\":1320,\"DuelBonusPercentLose\":0,\"DuelBonusPercentWin\":100},\"Village2\":{\"StrengthRangeForScore\":[{\"Percentage\":80,\"Milestone\":0},{\"Percentage\":80,\"Milestone\":200},{\"Percentage\":100,\"Milestone\":400},{\"Percentage\":120,\"Milestone\":600},{\"Percentage\":140,\"Milestone\":800},{\"Percentage\":160,\"Milestone\":1000},{\"Percentage\":180,\"Milestone\":1200},{\"Percentage\":200,\"Milestone\":1400},{\"Percentage\":400,\"Milestone\":1600},{\"Percentage\":600,\"Milestone\":1800},{\"Percentage\":1000,\"Milestone\":2000},{\"Percentage\":10000,\"Milestone\":10000}],\"TownHallMaxLevel\":7,\"ScoreChangeForLosing\":[{\"Percentage\":0,\"Milestone\":0},{\"Percentage\":30,\"Milestone\":400},{\"Percentage\":55,\"Milestone\":800},{\"Percentage\":70,\"Milestone\":1200},{\"Percentage\":85,\"Milestone\":1600},{\"Percentage\":95,\"Milestone\":2000},{\"Percentage\":100,\"Milestone\":2400}]},\"Village1\":{\"SpecialObstacle\":\" \"}}");
            clientHome.SetEventJSON("{\"events\":[{\"type\":4,\"id\":238,\"version\":6,\"visibleTime\":\"20171018T130000.000Z\",\"startTime\":\"20171018T130000.000Z\",\"endTime\":\"20171218T070000.000Z\",\"clashBoxEntryName\":\"\",\"eventEntryName\":\"CustomSCLoading\",\"inboxEntryId\":-1,\"notificationTid\":\"TID_LOCAL_NOTIFICATION_EVENT_BuilderBase_RewardsTimer\",\"image\":\"\",\"sc\":\"/b28191dc-8e64-497e-9221-90a14e968a18_BB_BattleFest.sc\",\"localization\":\"/9ff354b6-a48e-4363-8dcc-db0503e4ca99_builder_base_lootfest_mega_marathon.csv\",\"functions\":[],\"lootLimitCooldownInMinutes\":1320,\"duelBonusPercentWin\":100,\"duelBonusPercentLose\":0,\"duelBonusPercentDraw\":0,\"duelBonusMaxDiamondCostPercent\":33},{\"type\":0,\"id\":320,\"version\":5,\"visibleTime\":\"20171201T080000.000Z\",\"startTime\":\"20171208T080000.000Z\",\"endTime\":\"20171211T080000.000Z\",\"clashBoxEntryName\":\"\",\"eventEntryName\":\"CustomSCLoading\",\"inboxEntryId\":-1,\"notificationTid\":\"TID_LOCAL_NOTIFICATION_EVENT_Golem/Jump\",\"image\":\"/c4345350-765d-4783-b37c-1f5a8506ef5e_golem_jump.png\",\"sc\":\"/b6931982-811e-4630-a1e1-1930f1f74b29_purple_background.sc\",\"localization\":\"/1359dac9-cdee-4854-8530-f9f43217026b_Golem_Jump.csv\",\"functions\":[{\"name\":\"SpellDiscount\",\"parameters\":[\"26000003\",\"10\"]},{\"name\":\"UseTroop\",\"parameters\":[\"4000013\",\"10\",\"20\",\"100\",\"300\"]},{\"name\":\"TroopDiscount\",\"parameters\":[\"4000013\",\"10\"]}]},{\"type\":0,\"id\":327,\"version\":1,\"visibleTime\":\"20171208T080000.000Z\",\"startTime\":\"20171215T080000.000Z\",\"endTime\":\"20171218T080000.000Z\",\"clashBoxEntryName\":\"\",\"eventEntryName\":\"CustomSCLoading\",\"inboxEntryId\":-1,\"notificationTid\":\"TID_LOCAL_NOTIFICATION_EVENT_SPECIAL_TROOP_LavaLoonion\",\"image\":\"/daed1127-151d-4546-b868-59246de3d4ce_LavaLooNion_image.png\",\"sc\":\"/8754b802-00d1-4e67-bf77-faee25eb5619_purple_background.sc\",\"localization\":\"/e5d6dced-814d-4366-b3d6-665a008946e4_LavaLoonion.csv\",\"functions\":[{\"name\":\"UseTroop\",\"parameters\":[\"4000017\",\"10\",\"20\",\"30\",\"150\"]},{\"name\":\"UseTroop\",\"parameters\":[\"4000005\",\"10\",\"20\",\"30\",\"150\"]},{\"name\":\"UseTroop\",\"parameters\":[\"4000010\",\"10\",\"20\",\"30\",\"150\"]},{\"name\":\"TroopDiscount\",\"parameters\":[\"4000017\",\"50\"]},{\"name\":\"TroopDiscount\",\"parameters\":[\"4000005\",\"50\"]},{\"name\":\"TroopDiscount\",\"parameters\":[\"4000010\",\"50\"]}]}]}");

            gameMode.LoadHomeState(clientHome, clientAvatar, secondsSinceLastSave, timestamp);

            session.SendToProxy(new OwnHomeDataMessage
            {
                LogicClientAvatar = clientAvatar,
                LogicClientHome = clientHome,
                SecondsSinceLastSave = secondsSinceLastSave,
                Timestamp = timestamp
            });
        }
    }
}