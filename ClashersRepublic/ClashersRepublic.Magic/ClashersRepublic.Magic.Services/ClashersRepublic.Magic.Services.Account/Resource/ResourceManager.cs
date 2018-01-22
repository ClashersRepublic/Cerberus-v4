namespace ClashersRepublic.Magic.Services.Account.Resource
{
    using System;
    using System.IO;
    using ClashersRepublic.Magic.Titan.Json;
    using ClashersRepublic.Magic.Titan.Util;

    internal static class ResourceManager
    {
        private static bool _initialized;
        private static GameAssets _gameAssetsSettings;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            if (ResourceManager._initialized)
            {
                return;
            }

            ResourceManager._initialized = true;
            ResourceManager._gameAssetsSettings = new GameAssets();

            if (!File.Exists("resources.json"))
            {
                throw new Exception("resources.json file not exist.");
            }

            ResourceManager.LoadFromJson((LogicJSONObject)LogicJSONParser.Parse(File.ReadAllText("resources.json")));
        }

        /// <summary>
        ///     Loads this instance from json.
        /// </summary>
        private static void LoadFromJson(LogicJSONObject jsonObject)
        {
            ResourceManager._gameAssetsSettings.LoadFromJson(jsonObject);
        }

        private class GameAssets
        {
            internal string Fingerprint;
            internal string FingerprintContent;

            internal readonly LogicArrayList<string> ContentUrlList;
            internal readonly LogicArrayList<string> AppStoreUrlList;

            /// <summary>
            ///     Initializes a new instance of the <see cref="GameAssets"/> class.
            /// </summary>
            internal GameAssets()
            {
                this.ContentUrlList = new LogicArrayList<string>();
                this.AppStoreUrlList = new LogicArrayList<string>();
            }

            /// <summary>
            ///     Loads from json.
            /// </summary>
            internal void LoadFromJson(LogicJSONObject jsonObject)
            {
                LogicJSONArray contentUrlArray = jsonObject.GetJSONArray("content");
                LogicJSONArray appstoreUrlArray = jsonObject.GetJSONArray("appstore");
                LogicJSONString fingeprintString = jsonObject.GetJSONString("fingerprint");

                this.Fingerprint = fingeprintString.GetStringValue();

                if (!File.Exists(this.Fingerprint))
                {
                    throw new Exception(this.Fingerprint + " not exist.");
                }

                this.FingerprintContent = File.ReadAllText(this.Fingerprint);
                
                for (int i = 0; i < contentUrlArray.Size(); i++)
                {
                    this.ContentUrlList[i] = ((LogicJSONString) contentUrlArray[i]).GetStringValue();
                }

                for (int i = 0; i < appstoreUrlArray.Size(); i++)
                {
                    this.AppStoreUrlList[i] = ((LogicJSONString) appstoreUrlArray[i]).GetStringValue();
                }
            }
        }
    }
}