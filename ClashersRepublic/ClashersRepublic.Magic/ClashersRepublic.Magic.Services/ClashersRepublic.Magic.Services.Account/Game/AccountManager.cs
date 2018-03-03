namespace ClashersRepublic.Magic.Services.Account.Game
{
    using System.Threading.Tasks;
    using System.Collections.Concurrent;

    using ClashersRepublic.Magic.Services.Account.Database;
    using ClashersRepublic.Magic.Services.Core;
    using ClashersRepublic.Magic.Services.Core.Database;
    using ClashersRepublic.Magic.Services.Core.Network;
    using ClashersRepublic.Magic.Titan.Json;
    using ClashersRepublic.Magic.Titan.Math;
    using ClashersRepublic.Magic.Titan.Util;

    internal static class AccountManager
    {
        private static int[] _accountCounters;
        private static string _passTokenChars = "abcdefghijklmnopqrstuvwxyz0123456789";

        private static LogicRandom _random;
        private static ConcurrentDictionary<long, Account> _accounts;

        /// <summary>
        ///     Gets the total accounts.
        /// </summary>
        internal static int TotalAccounts
        {
            get
            {
                return AccountManager._accounts.Count;
            }
        }

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            AccountManager._accountCounters = new int[DatabaseManager.GetDatabaseCount()];
            AccountManager._random = new LogicRandom(LogicTimeUtil.GetTimestamp());
            AccountManager._accounts = new ConcurrentDictionary<long, Account>();
        }

        /// <summary>
        ///     Loads all accounts from database.
        /// </summary>
        internal static void LoadAccounts()
        {
            for (int highId = 0; highId < DatabaseManager.GetDatabaseCount(); highId++)
            {
                IDatabase database = DatabaseManager.GetDatabase(highId);

                if (database != null)
                {
                    int maxLowId = database.GetHigherId();
                    
                    Parallel.For(1, maxLowId + 1, new ParallelOptions { MaxDegreeOfParallelism = 3 }, id =>
                    {
                        string json = database.GetDocument(new LogicLong(highId, id));

                        if (json != null)
                        {
                            Account account = new Account();
                            account.Load(json);
                            AccountManager.TryAdd(account);

                            AccountManager._accountCounters[highId] = id;
                        }
                    });
                }
                else
                {
                    Logging.Warning(typeof(AccountManager), "AccountManager::loadAccounts pDatabase->NULL");
                }
            }
        }

        /// <summary>
        ///     Tries to add the specified <see cref="Account"/> instance.
        /// </summary>
        private static bool TryAdd(Account account)
        {
            return AccountManager._accounts.TryAdd(account.Id, account);
        }

        /// <summary>
        ///     Tries to get the instance of the specified <see cref="Account"/> id.
        /// </summary>
        private static bool TryGet(LogicLong accountId, out Account account)
        {
            return AccountManager._accounts.TryGetValue(accountId, out account);
        }

        /// <summary>
        ///     Tries to remove the specified <see cref="Account"/> instance.
        /// </summary>
        private static bool TryRemove(LogicLong accountId, out Account account)
        {
            return AccountManager._accounts.TryRemove(accountId, out account);
        }

        /// <summary>
        ///     Generates a random passtoken.
        /// </summary>
        private static string GeneratePassToken()
        {
            string passToken = null;

            for (int i = 0; i < 40; i++)
            {
                passToken += AccountManager._passTokenChars[AccountManager._random.Rand(AccountManager._passTokenChars.Length)];
            }

            return passToken;
        }

        /// <summary>
        ///     Gets a random higher id for a new account.
        /// </summary>
        private static int GetRandomHigherId()
        {
            int idx = -1;
            int minId = 0x7fffffff;

            for (int i = 0; i < AccountManager._accountCounters.Length; i++)
            {
                if (AccountManager._accountCounters[i] < minId)
                {
                    idx = i;
                    minId = AccountManager._accountCounters[i];
                }
            }

            return idx;
        }

        /// <summary>
        ///     Tries to create a new <see cref="Account"/> instance.
        /// </summary>
        internal static bool TryCreateAccount(out LogicLong accountId, out Account account)
        {
            int highId = AccountManager.GetRandomHigherId();

            if (highId != -1)
            {
                IDatabase database = DatabaseManager.GetDatabase(highId);

                if (database != null)
                {
                    accountId = new LogicLong(highId, AccountManager._accountCounters[highId] += NetManager.GetServerCount(ServiceCore.ServiceNodeType));
                    account = new Account(accountId, AccountManager.GeneratePassToken());

                    bool success = AccountManager.TryAdd(account);

                    if (success)
                    {
                        database.InsertDocument(accountId, LogicJSONParser.CreateJSONString(account.Save()));
                    }
                    
                    return success;
                }
            }

            accountId = null;
            account = null;

            return false;
        }

        /// <summary>
        ///     Tries to get the <see cref="Account"/> instance with id.
        /// </summary>
        internal static bool TryGetAccount(LogicLong accountId, out Account account)
        {
            return AccountManager.TryGet(accountId, out account);
        }
    }
}