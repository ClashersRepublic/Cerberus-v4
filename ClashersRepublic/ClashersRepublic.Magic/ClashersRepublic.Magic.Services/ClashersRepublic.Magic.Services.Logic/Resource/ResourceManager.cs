namespace ClashersRepublic.Magic.Services.Logic.Resource
{
    using System.IO;
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Titan.Debug;
    using ClashersRepublic.Magic.Titan.Json;
    using ClashersRepublic.Magic.Titan.Util;

    public static class ResourceManager
    {
        private static bool _initialized;

        public static string FingerprintVersion;
        public static string FingerprintJson;
        public static string FingerprintSha;
        
        public static LogicArrayList<string> ContentUrlList;
        public static LogicArrayList<string> ChronosContentUrlList;
        public static LogicArrayList<string> AppStoreUrlList;

        private static int[] _version;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        public static void Initialize()
        {
            if (ResourceManager._initialized)
            {
                return;
            }

            ResourceManager._initialized = true;
            ResourceManager.Load();
        }

        /// <summary>
        ///     Loads this instance.
        /// </summary>
        public static void Load()
        {
            ResourceManager.LoadFingeprint();
            ResourceManager.LoadResources();
        }

        /// <summary>
        ///     Loads the fingerprint file.
        /// </summary>
        public static void LoadFingeprint()
        {
            ResourceManager.FingerprintJson = ResourceManager.LoadAssetFile("fingerprint.json");

            if (ResourceManager.FingerprintJson != null)
            {
                LogicJSONObject jsonObject = (LogicJSONObject) LogicJSONParser.Parse(ResourceManager.FingerprintJson);

                ResourceManager.FingerprintSha = LogicJSONHelper.GetJSONString(jsonObject, "sha");
                ResourceManager.FingerprintVersion = LogicJSONHelper.GetJSONString(jsonObject, "version");

                string[] version = ResourceManager.FingerprintVersion.Split('.');

                if (version.Length == 3)
                {
                    ResourceManager._version = new int[3];

                    for (int i = 0; i < 3; i++)
                    {
                        ResourceManager._version[i] = int.Parse(version[i]);
                    }
                }
                else
                {
                    Debugger.Error("ResourceManager::loadFingeprint invalid fingerprint version: " + ResourceManager.FingerprintVersion);
                }
            }
            else
            {
                Debugger.Error("ResourceManager::loadFingeprint fingerprint.json not exist");
            }
        }

        /// <summary>
        ///     Loads resources file.
        /// </summary>
        public static void LoadResources()
        {
            ResourceManager.ContentUrlList = new LogicArrayList<string>();
            ResourceManager.ChronosContentUrlList = new LogicArrayList<string>();
            ResourceManager.AppStoreUrlList = new LogicArrayList<string>();

            if (File.Exists("resources.json"))
            {
                LogicJSONObject jsonObject = (LogicJSONObject) LogicJSONParser.Parse(File.ReadAllText("resources.json"));
                LogicJSONArray contentArray = jsonObject.GetJSONArray("content");
                LogicJSONArray chronosContentArray = jsonObject.GetJSONArray("chronosContent");
                LogicJSONArray appStoreArray = jsonObject.GetJSONArray("appstore");

                for (int i = 0; i < contentArray.Size(); i++)
                {
                    ResourceManager.ContentUrlList.Add(contentArray.GetJSONString(i).GetStringValue());
                }

                for (int i = 0; i < chronosContentArray.Size(); i++)
                {
                    ResourceManager.ChronosContentUrlList.Add(chronosContentArray.GetJSONString(i).GetStringValue());
                }

                for (int i = 0; i < appStoreArray.Size(); i++)
                {
                    ResourceManager.AppStoreUrlList.Add(appStoreArray.GetJSONString(i).GetStringValue());
                }
            }
            else
            {
                Debugger.Error("ResourceManager::loadResources resources.json not exist");
            }
        }

        /// <summary>
        ///     Loads the specified asset file.
        /// </summary>
        public static string LoadAssetFile(string file)
        {
            if (File.Exists("Assets/" + file))
            {
                return File.ReadAllText("Assets/" + file);
            }

            return null;
        }

        /// <summary>
        ///     Gets the content version.
        /// </summary>
        public static int GetContentVersion()
        {
            return ResourceManager._version[2];
        }

        /// <summary>
        ///     Gets the content url.
        /// </summary>
        public static string GetContentUrl()
        {
            if (ResourceManager.ContentUrlList.Count > 1)
            {
                return ResourceManager.ContentUrlList[1];
            }

            return "about:blank";
        }

        /// <summary>
        ///     Gets the content url.
        /// </summary>
        public static string GetAppStoreUrl(int deviceType)
        {
            if (deviceType != 0)
            {
                if (ResourceManager.AppStoreUrlList.Count > deviceType - 1)
                {
                    return ResourceManager.AppStoreUrlList[deviceType - 1];
                }
            }

            return string.Empty;
        }
    }
}