namespace ClashersRepublic.Magic.Proxy.Account
{
    using System;
    using System.Collections.Concurrent;

    using ClashersRepublic.Magic.Proxy.Database;
    using ClashersRepublic.Magic.Proxy.Log;
    using ClashersRepublic.Magic.Services.Logic;
    using ClashersRepublic.Magic.Titan.Math;
    using ClashersRepublic.Magic.Titan.Util;

    internal class GameAccountManager
    {
        private static ConcurrentDictionary<long, GameAccount> _accounts;
        private static readonly string PASSTOKEN_CHARS = "abcdefghijklmnopqrstuvwxyz0123456789";

        /// <summary>
        ///     Gets the total accounts in memory.
        /// </summary>
        internal static int TotalAccounts
        {
            get
            {
                return GameAccountManager._accounts.Count;
            }
        }

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            GameAccountManager._accounts = new ConcurrentDictionary<long, GameAccount>();
            GameAccountManager.LoadAllAccounts();
        }

        /// <summary>
        ///     Generates a pass token.
        /// </summary>
        internal static string GeneratePassToken()
        {
            string passToken = null;

            for (int i = 0; i < 40; i++)
            {
                passToken += GameAccountManager.PASSTOKEN_CHARS[Resources.Random.NextInt() % GameAccountManager.PASSTOKEN_CHARS.Length];
            }

            return passToken;
        }

        /// <summary>
        ///     Creates a new account.
        /// </summary>
        internal static GameAccount CreateAccount()
        {
            GameAccount account = new GameAccount();

            account.PassToken = GameAccountManager.GeneratePassToken();
            account.AccountCreationDate = LogicTimeUtil.GetTimestampMS();

            GameDatabaseManager.GetRandomDatabase().InsertAccount(account);

            if (GameAccountManager._accounts.TryAdd((long) (account.HighId << 32) | (uint) account.LowId, account))
            {
                return account;
            }

            return null;
        }

        /// <summary>
        ///     Loads all accounts from database.
        /// </summary>
        internal static void LoadAllAccounts()
        {
            // LoadAllAccounts.
        }

        /// <summary>
        ///     Loads the specified account.
        /// </summary>
        internal static int LoadAccount(LogicLong accountId, out GameAccount account)
        {
            GameDatabase database = GameDatabaseManager.GetDatabase(accountId.GetHigherInt());

            if (database != null)
            {
                account = database.GetAccount(accountId.GetLowerInt());

                if (account != null)
                {
                    if (!GameAccountManager._accounts.TryAdd(accountId, account))
                    {
                        Logging.Error(typeof(GameAccountManager), "GameAccountManager::loadAccount account already added");
                    }

                    return 0;
                }

                return 2;
            }
            else
            {
                account = null;
            }

            return 1;
        }

        /// <summary>
        ///     Get sthe specified account.
        /// </summary>
        internal static int GetAccount(LogicLong accountId, out GameAccount account)
        {
            if (!GameAccountManager._accounts.TryGetValue(accountId, out account))
            {
                return GameAccountManager.LoadAccount(accountId, out account);
            }

            return 0;
        }
    }
}