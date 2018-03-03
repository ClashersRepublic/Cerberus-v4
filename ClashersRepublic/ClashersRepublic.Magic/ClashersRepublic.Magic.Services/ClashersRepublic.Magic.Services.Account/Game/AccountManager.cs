namespace ClashersRepublic.Magic.Services.Account.Game
{
    using System;
    using System.Collections.Concurrent;
    using System.Diagnostics;
    using System.Threading.Tasks;
    using ClashersRepublic.Magic.Services.Account.Database;
    using ClashersRepublic.Magic.Services.Core;
    using ClashersRepublic.Magic.Titan.Json;
    using ClashersRepublic.Magic.Titan.Math;
    using ClashersRepublic.Magic.Titan.Util;

    internal static class AccountManager
    {
        private static int _accountCounter;
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
            AccountManager._accountCounter = 0;

            AccountManager._random = new LogicRandom(LogicTimeUtil.GetTimestamp());
            AccountManager._accounts = new ConcurrentDictionary<long, Account>();
        }

        /// <summary>
        ///     Loads all accounts from database.
        /// </summary>
        internal static void LoadAccounts()
        {
            IDatabase database = DatabaseManager.GetDatabase();

            if (database != null)
            {
                AccountManager._accountCounter = database.GetHigherId();
                
                Parallel.For(1, AccountManager._accountCounter + 1, new ParallelOptions {MaxDegreeOfParallelism = 3}, id =>
                {
                    string json = database.GetDocument(new LogicLong(ServiceCore.ServiceNodeId, id));

                    if (json != null)
                    {
                        Account account = new Account();
                        account.Load(json);
                        AccountManager.TryAdd(account);
                    }
                });
            }
            else
            {
                Logging.Warning(typeof(AccountManager), "AccountManager::loadAccounts pDatabase->NULL");
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
        ///     Tries to create a new <see cref="Account"/> instance.
        /// </summary>
        internal static bool TryCreateAccount(out LogicLong accountId, out Account account)
        {
            accountId = new LogicLong(ServiceCore.ServiceNodeId, ++AccountManager._accountCounter);
            account = new Account(accountId, AccountManager.GeneratePassToken());

            bool success = AccountManager.TryAdd(account);

            if (success)
            {
                DatabaseManager.GetDatabase().InsertDocument(accountId, LogicJSONParser.CreateJSONString(account.Save()));
            }

            return success;
        }

        /// <summary>
        ///     Tries to get the <see cref="Account"/> instance with id.
        /// </summary>
        internal static bool TryGetAccount(LogicLong accountId, out Account account)
        {
            if (accountId.GetHigherInt() == ServiceCore.ServiceNodeId)
            {
                return AccountManager.TryGet(accountId, out account);
            }

            account = null;
            return false;
        }
    }
}