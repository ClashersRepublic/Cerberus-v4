namespace ClashersRepublic.Magic.Services.Account.Logic.Account
{
    using System.Collections.Concurrent;

    using ClashersRepublic.Magic.Services.Account.Database;
    using ClashersRepublic.Magic.Services.Logic.Account;
    using ClashersRepublic.Magic.Titan.Math;

    internal static class GameAccountManager
    {
        private static bool _initialized;
        private static int _lastLowId;

        private static ConcurrentDictionary<long, GameAccount> _accounts;
        private static ConcurrentDictionary<string, GameAccount> _sessions;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            if (GameAccountManager._initialized)
            {
                return;
            }

            GameAccountManager._initialized = true;

            GameAccountManager._lastLowId = GameDatabase.GetHigherLowId();
            GameAccountManager._accounts = new ConcurrentDictionary<long, GameAccount>();
            GameAccountManager._sessions = new ConcurrentDictionary<string, GameAccount>();
        }

        /// <summary>
        ///     Loads all accounts from database.
        /// </summary>
        internal static void LoadAccountsFromDB()
        {
            for (int i = 1; i <= GameAccountManager._lastLowId; i++)
            {
                GameAccountManager.GetAccount(new LogicLong(Config.ServerId, i));
            }
        }

        /// <summary>
        ///     Gets the account instance by id.
        /// </summary>
        internal static GameAccount GetAccount(LogicLong accountId)
        {
            if (!GameAccountManager._accounts.TryGetValue(accountId, out GameAccount account))
            {
                account = GameAccountManager.GetAccount(accountId);

                if (account != null)
                {
                    if (!GameAccountManager._accounts.TryAdd(accountId, account))
                    {
                        Logging.Error(typeof(GameAccountManager), "GetAccount account id " + accountId + " already added");
                    }
                }
            }

            return account;
        }

        /// <summary>
        ///     Gets the account instance by session id.
        /// </summary>
        internal static GameAccount GetAccount(string sessionId)
        {
            return GameAccountManager._sessions.TryGetValue(sessionId, out GameAccount account) ? account : null;
        }

        /// <summary>
        ///     Loads the account from database.
        /// </summary>
        internal static GameAccount LoadAccountFromDB(LogicLong accountId)
        {
            GameAccount account = GameDatabase.LoadAccount(accountId);

            if (account == null)
            {
                Logging.Warning(typeof(GameAccountManager), "LoadAccountFromDB account id " + accountId + " not exist");
            }

            return account;
        }
    }
}