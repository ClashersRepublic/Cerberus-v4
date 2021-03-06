﻿namespace RivieraStudio.Magic.Services.Zone.Game
{
    using System.Threading.Tasks;
    using System.Collections.Generic;
    
    using RivieraStudio.Magic.Services.Core;
    using RivieraStudio.Magic.Services.Core.Database;
    using RivieraStudio.Magic.Services.Core.Network;
    using RivieraStudio.Magic.Services.Core.Utils;
    using RivieraStudio.Magic.Titan.Json;
    using RivieraStudio.Magic.Titan.Math;
    using RivieraStudio.Magic.Titan.Util;

    internal static class ZoneAccountManager
    {
        private static Dictionary<long, ZoneAccount> _zones;

        /// <summary>
        ///     Gets the total accounts.
        /// </summary>
        internal static int TotalAccounts
        {
            get
            {
                return ZoneAccountManager._zones.Count;
            }
        }

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            ZoneAccountManager._zones = new Dictionary<long, ZoneAccount>();
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
                                ZoneAccount zoneAccount = new ZoneAccount();
                                zoneAccount.Load(json);

                                lock (locker)
                                {
                                    ZoneAccountManager._zones.Add(zoneAccount.Id, zoneAccount);
                                }
                            }
                        }
                    });
                }
                else
                {
                    Logging.Warning("ZoneAccountManager::loadAccounts pDatabase->NULL");
                }
            }
        }

        /// <summary>
        ///     Tries to get the instance of the specified <see cref="ZoneAccount"/> id.
        /// </summary>
        internal static bool TryGet(LogicLong avatarId, out ZoneAccount account)
        {
            return ZoneAccountManager._zones.TryGetValue(avatarId, out account);
        }

        /// <summary>
        ///     Tries to update the instance in database.
        /// </summary>
        internal static void UpdateZoneAccount(LogicLong homeId, ZoneAccount update)
        {
            if (ZoneAccountManager._zones.ContainsKey(homeId))
            {
                ZoneAccountManager._zones[homeId] = update;
            }
            else
            {
                ZoneAccountManager._zones.Add(homeId, update);
            }

            DatabaseManagerNew.Update(0, homeId, LogicJSONParser.CreateJSONString(update.Save()));
        }

        /// <summary>
        ///     Create a new <see cref="ZoneAccount"/> instance.
        /// </summary>
        internal static ZoneAccount CreateZoneAccount(LogicLong homeId)
        {
            ZoneAccount home = new ZoneAccount(homeId);
            ZoneAccountManager._zones.Add(homeId, home);
            DatabaseManagerNew.Insert(0, homeId, LogicJSONParser.CreateJSONString(home.Save()));
            
            return home;
        }

        /// <summary>
        ///     Called when the service is closed.
        /// </summary>
        internal static void OnQuit()
        {
            int timeStamp = LogicTimeUtil.GetTimestamp();

            for (int i = 0; i < DatabaseManager.GetDatabaseCount(0); i++)
            {
                DatabaseManager.GetDatabaseAt(0, i).ExecuteQueryCommand(string.Format("UPDATE `%BUCKET_NAME%` SET unload_time={0} WHERE (id_lo - 1) % {1} == {2}", timeStamp,
                    NetManager.GetServerCount(NetUtils.SERVICE_NODE_TYPE_ZONE_CONTAINER), ServiceCore.ServiceNodeId));
            }
        }
    }
}