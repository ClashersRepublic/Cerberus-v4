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
    using ClashersRepublic.Magic.Services.Home.Session;

    using ClashersRepublic.Magic.Services.Logic;
    using ClashersRepublic.Magic.Titan.Json;
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
        ///     Gets the player by id.
        /// </summary>
        internal static GamePlayer GetPlayer(LogicLong accountId, bool isNewClient)
        {
            if (!GamePlayerManager._players.TryGetValue(accountId, out GamePlayer player))
            {
                if (isNewClient)
                {
                    player = GamePlayerManager.CreatePlayer(accountId);

                    if (player == null)
                    {
                        Logging.Error(typeof(GamePlayerManager), "GamePlayerManager::getPlayer account creation error");
                    }
                }
                else
                {
                    Logging.Warning(typeof(GamePlayerManager), "GamePlayerManager::getPlayer player id " + accountId + " not exist");
                }
            }

            return player;
        }

        /// <summary>
        ///     Saves the player to database.
        /// </summary>
        internal static void SavePlayer(GamePlayer player, LogicGameMode gameMode)
        {
            if (gameMode.GetState() == 1)
            {
                LogicLevel level = gameMode.GetLevel();
                LogicJSONObject jsonRoot = new LogicJSONObject();

                level.SaveToJSON(jsonRoot);
                level.GetHome().SetHomeJSON(LogicJSONParser.CreateJSONString(jsonRoot));
            }

            player.UpdateDocuments();

            GameDatabase.SaveAccount(player);
        }
    }
}