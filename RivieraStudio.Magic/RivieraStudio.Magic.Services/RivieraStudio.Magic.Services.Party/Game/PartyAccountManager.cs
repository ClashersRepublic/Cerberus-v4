namespace RivieraStudio.Magic.Services.Party.Game
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using RivieraStudio.Magic.Services.Core;
    using RivieraStudio.Magic.Services.Core.Database;
    using RivieraStudio.Magic.Services.Core.Network;

    using RivieraStudio.Magic.Titan.Json;
    using RivieraStudio.Magic.Titan.Math;

    internal static class PartyAccountManager
    {
        private static Dictionary<long, PartyAccount> _accounts;

        /// <summary>
        ///     Gets the total accounts.
        /// </summary>
        internal static int TotalAccounts
        {
            get
            {
                return PartyAccountManager._accounts.Count;
            }
        }

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            PartyAccountManager._accounts = new Dictionary<long, PartyAccount>();
        }

        /// <summary>
        ///     Loads all accounts from database.
        /// </summary>
        internal static void LoadAccounts()
        {
            for (int i = 0; i < DatabaseManager.GetDatabaseCount(0); i++)
            {
                CouchbaseDatabase database = DatabaseManager.GetDatabaseAt(0, i);

                if (database != null)
                {
                    int highId = i;
                    int maxLowId = database.GetHigherId();

                    object locker = new object();

                    Parallel.For(1, maxLowId + 1, new ParallelOptions { MaxDegreeOfParallelism = 4 }, id =>
                    {
                        if (NetManager.GetDocumentOwnerId(ServiceCore.ServiceNodeType, id) == ServiceCore.ServiceNodeId)
                        {
                            string json = database.GetDocument(new LogicLong(highId, id));

                            if (json != null)
                            {
                                PartyAccount partyAccount = new PartyAccount();
                                partyAccount.Load(json);

                                lock (locker)
                                {
                                    PartyAccountManager._accounts.Add(partyAccount.Id, partyAccount);
                                }
                            }
                        }
                    });
                }
                else
                {
                    Logging.Warning("PartyAccountManager::loadHomes pDatabase->NULL");
                }
            }
        }

        /// <summary>
        ///     Tries to get the instance of the specified <see cref="PartyAccount"/> id.
        /// </summary>
        internal static bool TryGet(LogicLong accountId, out PartyAccount home)
        {
            return PartyAccountManager._accounts.TryGetValue(accountId, out home);
        }

        /// <summary>
        ///     Create a new <see cref="PartyAccount"/> instance.
        /// </summary>
        internal static PartyAccount CreatePartyAccount(LogicLong homeId)
        {
            PartyAccount partyAccount = new PartyAccount(homeId);
            PartyAccountManager._accounts.Add(homeId, partyAccount);
            DatabaseManager.Insert(0, homeId, LogicJSONParser.CreateJSONString(partyAccount.Save()));

            return partyAccount;
        }
    }
}