namespace LineageSoft.Magic.Services.Avatar.Game
{
    using System.Collections.Generic;
    using System.Threading.Tasks;

    using LineageSoft.Magic.Services.Core;
    using LineageSoft.Magic.Services.Core.Database;
    using LineageSoft.Magic.Services.Core.Network;

    using LineageSoft.Magic.Titan.Json;
    using LineageSoft.Magic.Titan.Math;

    internal static class AvatarAccountManager
    {
        private static Dictionary<long, AvatarAccount> _accounts;

        /// <summary>
        ///     Gets the total accounts.
        /// </summary>
        internal static int TotalAccounts
        {
            get
            {
                return AvatarAccountManager._accounts.Count;
            }
        }

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            AvatarAccountManager._accounts = new Dictionary<long, AvatarAccount>();
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
                                AvatarAccount avatarAccount = new AvatarAccount();
                                avatarAccount.Load(json);

                                lock (locker)
                                {
                                    AvatarAccountManager._accounts.Add(avatarAccount.Id, avatarAccount);
                                }
                            }
                        }
                    });
                }
                else
                {
                    Logging.Warning("AvatarAccountManager::loadAccounts pDatabase->NULL");
                }
            }
        }

        /// <summary>
        ///     Tries to get the instance of the specified <see cref="AvatarAccount"/> id.
        /// </summary>
        internal static bool TryGet(LogicLong accountId, out AvatarAccount home)
        {
            return AvatarAccountManager._accounts.TryGetValue(accountId, out home);
        }

        /// <summary>
        ///     Create a new <see cref="AvatarAccount"/> instance.
        /// </summary>
        internal static AvatarAccount CreatePartyAccount(LogicLong homeId)
        {
            AvatarAccount avatarAccount = new AvatarAccount(homeId);
            AvatarAccountManager._accounts.Add(homeId, avatarAccount);
            DatabaseManager.Insert(0, homeId, LogicJSONParser.CreateJSONString(avatarAccount.Save()));

            return avatarAccount;
        }
    }
}