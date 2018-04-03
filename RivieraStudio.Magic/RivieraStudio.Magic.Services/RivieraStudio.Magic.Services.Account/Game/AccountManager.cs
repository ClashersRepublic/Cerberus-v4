namespace RivieraStudio.Magic.Services.Account.Game
{
    using System.Threading.Tasks;
    using System.Collections.Generic;
    
    using RivieraStudio.Magic.Services.Core;
    using RivieraStudio.Magic.Services.Core.Database;
    using RivieraStudio.Magic.Services.Core.Network;

    using RivieraStudio.Magic.Titan.Json;
    using RivieraStudio.Magic.Titan.Math;
    using RivieraStudio.Magic.Titan.Util;

    internal static class AccountManager
    {
        private static int[] _accountCounters;
        private static string _passTokenChars = "abcdefghijklmnopqrstuvwxyz0123456789";

        private static LogicRandom _random;
        private static Dictionary<long, Account> _accounts;

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
            AccountManager._accountCounters = new int[DatabaseManager.GetDatabaseCount(0)];
            AccountManager._random = new LogicRandom(LogicTimeUtil.GetTimestamp());
            AccountManager._accounts = new Dictionary<long, Account>();
        }

        /// <summary>
        ///     Loads all accounts from database.
        /// </summary>
        internal static void LoadAccounts()
        {
            int dbCount = DatabaseManager.GetDatabaseCount(0);

            for (int i = 0; i < dbCount; i++)
            {
                CouchbaseDatabase database = DatabaseManager.GetDatabaseAt(0, i);

                if (database != null)
                {
                    int highId = i;
                    int maxLowId = database.GetHigherId();

                    object locker = new object();

                    Parallel.For(1, maxLowId + 1, new ParallelOptions { MaxDegreeOfParallelism = 4 }, id =>
                    {
                        LogicLong accId = new LogicLong(highId, id);

                        if (NetManager.GetDocumentOwnerId(ServiceCore.ServiceNodeType, id) == ServiceCore.ServiceNodeId)
                        {
                            string json = database.GetDocument(accId);

                            if (json != null)
                            {
                                Account account = new Account();
                                account.Load(json);

                                lock (locker)
                                {
                                    AccountManager._accounts.Add(account.Id, account);
                                    AccountManager._accountCounters[highId] = LogicMath.Max(id, AccountManager._accountCounters[highId]);
                                }
                            }
                        }
                    });
                }
                else
                {
                    Logging.Warning("AccountManager::loadAccounts pDatabase->NULL");
                }
            }
        }
        
        /// <summary>
        ///     Tries to get the instance of the specified <see cref="Account"/> id.
        /// </summary>
        internal static bool TryGet(LogicLong accountId, out Account account)
        {
            return AccountManager._accounts.TryGetValue(accountId, out account);
        }
        
        /// <summary>
        ///     Generates a random passtoken.
        /// </summary>
        private static string GeneratePassToken()
        {
            string passToken = null;

            for (int i = 0, rnd = 0; i < 40; i++, rnd >>= 8)
            {
                if (i % 4 == 0)
                {
                    rnd = AccountManager._random.Rand(0x7fffffff);
                }

                passToken += AccountManager._passTokenChars[rnd % AccountManager._passTokenChars.Length];
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
        internal static Account CreateAccount()
        {
            int highId = AccountManager.GetRandomHigherId();

            if (highId != -1)
            {
                CouchbaseDatabase database = DatabaseManager.GetDatabaseAt(0, highId);

                if (database != null)
                {
                    LogicLong accountId = new LogicLong(highId, AccountManager._accountCounters[highId] += NetManager.GetServerCount(ServiceCore.ServiceNodeType));
                    Account account = new Account(accountId, AccountManager.GeneratePassToken());                  
                    AccountManager._accounts.Add(account.Id, account);

                    database.InsertDocument(accountId, LogicJSONParser.CreateJSONString(account.Save()));

                    return account;
                }
            }

            return null;
        }
    }
}