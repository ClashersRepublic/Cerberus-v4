namespace ClashersRepublic.Magic.Proxy.Account
{
    using System.Collections.Concurrent;

    using ClashersRepublic.Magic.Proxy.Database;
    using ClashersRepublic.Magic.Proxy.Debug;
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

            GameDatabase.InsertAccount(account);

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
            for (int dbId = 0; dbId < Config.MongodServers.Length; dbId++)
            {
                int lastAccountId = GameDatabase.GetHigherAccountId(dbId);

                for (int lowId = 1; lowId <= lastAccountId; lowId++)
                {
                    GameAccountManager.LoadAccount(new LogicLong(dbId, lowId));
                }
            }
        }

        /// <summary>
        ///     Loads the specified account.
        /// </summary>
        internal static GameAccount LoadAccount(LogicLong accountId)
        {
            GameAccount account = GameDatabase.LoadAccount(accountId);

            if (account != null)
            {
                if (!GameAccountManager._accounts.TryAdd(accountId, account))
                {
                    Logging.Error(typeof(GameAccountManager), "GameAccountManager::loadAccount account already added");
                }
            }

            return account;
        }

        /// <summary>
        ///     Get sthe specified account.
        /// </summary>
        internal static GameAccount GetAccount(LogicLong accountId)
        {
            if (!GameAccountManager._accounts.TryGetValue(accountId, out GameAccount account))
            {
                account = GameAccountManager.LoadAccount(accountId);
            }

            return account;
        }
    }
}