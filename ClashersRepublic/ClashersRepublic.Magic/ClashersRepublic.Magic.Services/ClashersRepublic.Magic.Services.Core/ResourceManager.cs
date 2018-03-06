namespace ClashersRepublic.Magic.Services.Core
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using System.Threading.Tasks;
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Services.Core.Libs.Lzma.Compression.LZMA;
    using ClashersRepublic.Magic.Services.Core.Web;
    using ClashersRepublic.Magic.Titan.Debug;
    using ClashersRepublic.Magic.Titan.Json;
    using ClashersRepublic.Magic.Titan.Util;

    public static class ResourceManager
    {
        private static bool _initialized;

        public static byte[] CompressedFingeprintJson;
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
        private static void Load()
        {
            ResourceManager.VerifyAssets();
            ResourceManager.LoadFingeprint();
            ResourceManager.LoadResources();
        }

        /// <summary>
        ///     Verifies the version of assets.
        /// </summary>
        private static void VerifyAssets()
        {
            string json = ResourceManager.LoadAssetFile("fingerprint.json");

            if (json != null)
            {
                LogicJSONObject jsonObject = (LogicJSONObject)LogicJSONParser.Parse(json);
                LogicJSONArray fileArray = jsonObject.GetJSONArray("files");
                LogicJSONString shaObject = jsonObject.GetJSONString("sha");

                for (int i = 0; i < fileArray.Size(); i++)
                {
                    LogicJSONObject fileObject = fileArray.GetJSONObject(i);

                    if (fileObject != null)
                    {
                        string fileName = LogicJSONHelper.GetJSONString(fileObject, "file");

                        if (File.Exists("Assets/" + fileName))
                        {
                            continue;
                        }

                        Logging.Debug(typeof(ResourceManager), "ResourceManager::verifyAssets missing file " + fileName);
                    }
                    else
                    {
                        Logging.Debug(typeof(ResourceManager), "ResourceManager::verifyAssets pFileObject->NULL");
                    }

                    ResourceManager.DownloadAssets(json);
                }

                if (shaObject != null)
                {
                    string lastSha = WebManager.DownloadFileFromConfigServer("/assets/sha");

                    if (lastSha != null)
                    {
                        if (shaObject.GetStringValue() != lastSha)
                        {
                            json = WebManager.DownloadFileFromConfigServer("/assets/generated/" + lastSha + "/fingerprint.json");

                            if (json != null)
                            {
                                ResourceManager.DownloadAssets(json);
                            }
                        }
                    }
                    else
                    {
                        Logging.Warning(typeof(ResourceManager), "ResourceManager::verifyAssets /assets/sha doesn't exist");
                    }
                }
                else
                {
                    Logging.Warning(typeof(ResourceManager), "ResourceManager::verifyAssets pShaObject->NULL");

                    string lastSha = WebManager.DownloadFileFromConfigServer("/assets/sha");

                    if (lastSha != null)
                    {
                        json = WebManager.DownloadFileFromConfigServer("/assets/generated/" + lastSha + "/fingerprint.json");

                        if (json != null)
                        {
                            ResourceManager.DownloadAssets(json);
                        }
                    }
                }
            }
            else
            {
                string lastSha = WebManager.DownloadFileFromConfigServer("/assets/sha");
                
                if (lastSha != null)
                {
                    json = WebManager.DownloadFileFromConfigServer("/assets/generated/" + lastSha + "/fingerprint.json");

                    if (json != null)
                    {
                        ResourceManager.DownloadAssets(json);
                    }
                }
            }
        }

        /// <summary>
        ///     Downloads assets from config server.
        /// </summary>
        private static void DownloadAssets(string fingerprint)
        {
            LogicJSONObject jsonObject = (LogicJSONObject) LogicJSONParser.Parse(fingerprint);
            LogicJSONArray fileArray = jsonObject.GetJSONArray("files");

            string shaFingerprint = LogicJSONHelper.GetJSONString(jsonObject, "sha");

            Logging.Debug(typeof(ResourceManager), "Download " + shaFingerprint + " assets...");

            for(int i = 0; i < fileArray.Size(); i++)
            {
                LogicJSONObject fileObject = fileArray.GetJSONObject(i);

                if (fileObject != null)
                {
                    string fileName = LogicJSONHelper.GetJSONString(fileObject, "file");
                    string sha = LogicJSONHelper.GetJSONString(fileObject, "sha");

                    byte[] data = WebManager.DownloadDataFromConfigServer(string.Format("/assets/generated/{0}/{1}", shaFingerprint, fileName));

                    if (data != null)
                    {
                        string hash;

                        using (SHA1 shaService = new SHA1CryptoServiceProvider())
                        {
                            hash = BitConverter.ToString(shaService.ComputeHash(data)).Replace("-", string.Empty).ToLower();
                        }

                        if (sha != hash)
                        {
                            Logging.Warning(typeof(ResourceManager), "ResourceManager::downloadAssets sha mismatch");
                        }

                        switch (Path.GetExtension(fileName))
                        {
                            case ".csv":
                                using (MemoryStream inputStream = new MemoryStream(data))
                                {
                                    using (MemoryStream outputStream = new MemoryStream())
                                    {
                                        Decoder decompresser = new Decoder();

                                        byte[] properties = new byte[5];
                                        byte[] fileLengthBytes = new byte[4];

                                        inputStream.Read(properties, 0, 5);
                                        inputStream.Read(fileLengthBytes, 0, 4);

                                        int fileLength = fileLengthBytes[0] | fileLengthBytes[1] << 8 | fileLengthBytes[2] << 16 | fileLengthBytes[3] << 24;

                                        decompresser.SetDecoderProperties(properties);
                                        decompresser.Code(inputStream, outputStream, inputStream.Length, fileLength, null);

                                        data = outputStream.ToArray();
                                    }
                                }

                                break;
                        }

                        Directory.CreateDirectory("Assets/" + fileName.Replace(Path.GetFileName(fileName), string.Empty));
                        File.WriteAllBytes("Assets/" + fileName, data);
                    }
                }
            }

            File.WriteAllText("Assets/fingerprint.json", fingerprint);
            Logging.Debug(typeof(ResourceManager), "Download completed");
        }

        /// <summary>
        ///     Loads the fingerprint file.
        /// </summary>
        private static void LoadFingeprint()
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

                ZLibHelper.CompressInZLibFormat(LogicStringUtil.GetBytes(ResourceManager.FingerprintJson), out ResourceManager.CompressedFingeprintJson);
            }
            else
            {
                Debugger.Error("ResourceManager::loadFingeprint fingerprint.json not exist");
            }
        }

        /// <summary>
        ///     Loads resources file.
        /// </summary>
        private static void LoadResources()
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