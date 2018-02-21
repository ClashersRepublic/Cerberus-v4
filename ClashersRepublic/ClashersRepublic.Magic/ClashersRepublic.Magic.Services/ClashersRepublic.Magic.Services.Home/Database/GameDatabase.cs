namespace ClashersRepublic.Magic.Services.Home.Database
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using ClashersRepublic.Magic.Services.Logic;
    using ClashersRepublic.Magic.Titan.Json;
    using Couchbase;
    using Couchbase.Core;

    internal static class GameDatabase
    {
        private static ICluster _cluster;
        private static IBucket _homeBucket;
        private static IBucket _accBucket;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            using (WebClient www = new WebClient())
            {
                GameDatabase.LoadConfig(www.DownloadString(Config.DatabaseFile));
            }
        }

        /// <summary>
        ///     Loads the config file.
        /// </summary>
        private static void LoadConfig(string file)
        {
            LogicJSONObject jsonObject = (LogicJSONObject) LogicJSONParser.Parse(file);
            LogicJSONArray serverArray = jsonObject.GetJSONArray("servers");

            List<Uri> servers = new List<Uri>(serverArray.Size());

            for (int i = 0; i < serverArray.Size(); i++)
            {
                servers.Add(new Uri(serverArray.GetJSONString(i).GetStringValue()));
            }

            GameDatabase._cluster = new Cluster
            {
                Configuration =
                {
                    Servers = servers
                }
            };

            GameDatabase._accBucket = GameDatabase._cluster.OpenBucket("magic-acc");
            GameDatabase._homeBucket = GameDatabase._cluster.OpenBucket("magic-home");
        }
    }
}