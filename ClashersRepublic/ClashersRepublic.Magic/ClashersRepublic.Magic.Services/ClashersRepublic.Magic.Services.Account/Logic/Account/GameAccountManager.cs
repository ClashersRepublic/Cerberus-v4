namespace ClashersRepublic.Magic.Services.Account.Logic.Account
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using ClashersRepublic.Magic.Services.Account.Database;
    using ClashersRepublic.Magic.Services.Logic;
    using ClashersRepublic.Magic.Services.Logic.Account;
    using ClashersRepublic.Magic.Titan.Math;
    using ClashersRepublic.Magic.Titan.Util;

    internal static class GameAccountManager
    {
        private static bool _initialized;
        private static int _lastLowId;

        private static LogicMersenneTwister _passTokenGenerator;

        private static ConcurrentDictionary<long, GameAccount> _accounts;
        private static ConcurrentDictionary<string, GameAccount> _sessions;

        private static readonly string PassTokenChars = "abcdefghijklmnopqrstuvwxyz0123456789";

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
            GameAccountManager._passTokenGenerator = new LogicMersenneTwister(LogicTimeUtil.GetTimestamp());
            GameAccountManager._accounts = new ConcurrentDictionary<long, GameAccount>();
            GameAccountManager._sessions = new ConcurrentDictionary<string, GameAccount>();

            GameAccountManager.LoadAccountsFromDB();
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

            Console.WriteLine("In-Memory Accounts : " + GameAccountManager._accounts.Count);
        }

        /// <summary>
        ///     Generates a pass token.
        /// </summary>
        internal static string GeneratePassToken()
        {
            string passToken = null;

            for (int i = 0; i < 40; i++)
            {
                passToken += GameAccountManager.PassTokenChars[GameAccountManager._passTokenGenerator.NextInt() % GameAccountManager.PassTokenChars.Length];
            }

            return passToken;
        }

        /// <summary>
        ///     Creates a new account.
        /// </summary>
        internal static GameAccount CreateAccount()
        {
            GameAccount account = new GameAccount(Config.ServerId, Interlocked.Increment(ref GameAccountManager._lastLowId), GameAccountManager.GeneratePassToken());
            GameDatabase.InsertAccount(account);
            return account;
        }

        /// <summary>
        ///     Gets the account instance by id.
        /// </summary>
        internal static GameAccount GetAccount(LogicLong accountId)
        {
            if (!GameAccountManager._accounts.TryGetValue(accountId, out GameAccount account))
            {
                account = GameAccountManager.LoadAccountFromDB(accountId);

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