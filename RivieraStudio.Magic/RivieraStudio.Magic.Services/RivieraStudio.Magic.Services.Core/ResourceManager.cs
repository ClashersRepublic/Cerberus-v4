﻿namespace RivieraStudio.Magic.Services.Core
{
    using System;
    using System.IO;
    using System.Net;
    using System.Security.Cryptography;
    using System.Threading.Tasks;
    using RivieraStudio.Magic.Logic.Helper;
    using RivieraStudio.Magic.Services.Core.Libs.Lzma.Compression.LZMA;
    using RivieraStudio.Magic.Services.Core.Web;
    using RivieraStudio.Magic.Titan.Debug;
    using RivieraStudio.Magic.Titan.Json;
    using RivieraStudio.Magic.Titan.Util;

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
                LogicJSONObject jsonObject = (LogicJSONObject) LogicJSONParser.Parse(json);
                LogicJSONString shaObject = jsonObject.GetJSONString("sha");

                string lastSha = WebManager.DownloadConfigString("/sha");

                if (shaObject.GetStringValue() != lastSha)
                {
                    ResourceManager.DownloadLastAssets(jsonObject);
                }
            }
            else
            {
                ResourceManager.DownloadLastAssets(null);
            }
        }

        /// <summary>
        ///     Downloads last assets.
        /// </summary>
        private static void DownloadLastAssets(LogicJSONObject currentFingerprint)
        {
            string lastSha = WebManager.DownloadConfigString("/sha");

            if (lastSha != null)
            {
                string json = WebManager.DownloadAssetString(lastSha, "fingerprint.json");

                if (json != null)
                {
                    ResourceManager.DownloadAssets(currentFingerprint, json);
                }
            }
        }

        /// <summary>
        ///     Downloads assets from config server.
        /// </summary>
        private static void DownloadAssets(LogicJSONObject currentFingerprint, string fingerprint)
        {
            LogicJSONArray currentFileArray = currentFingerprint?.GetJSONArray("files");
            LogicJSONObject jsonObject = (LogicJSONObject) LogicJSONParser.Parse(fingerprint);
            LogicJSONArray fileArray = jsonObject.GetJSONArray("files");

            if (currentFileArray != null)
            {
                if (fileArray.Size() != currentFileArray.Size())
                {
                    currentFileArray = null;
                }
            }

            string shaFingerprint = LogicJSONHelper.GetJSONString(jsonObject, "sha");

            Logging.Print("Download " + shaFingerprint + " assets...");

            Parallel.For(0, fileArray.Size(), new ParallelOptions {MaxDegreeOfParallelism = 4}, i =>
            {
                LogicJSONObject fileObject = fileArray.GetJSONObject(i);

                if (fileObject != null)
                {
                    string fileName = LogicJSONHelper.GetJSONString(fileObject, "file");
                    string sha = LogicJSONHelper.GetJSONString(fileObject, "sha");
                     
                    if (currentFileArray != null)
                    {
                        for (int j = 0; j < currentFileArray.Size(); j++)
                        {
                            LogicJSONObject currentFileObject = currentFileArray.GetJSONObject(j);
                            LogicJSONString currentFileNameObject = currentFileObject.GetJSONString("file");

                            if (fileName == currentFileNameObject.GetStringValue())
                            {
                                LogicJSONString currentShaObject = currentFileObject.GetJSONString("sha");

                                if (sha == currentShaObject.GetStringValue())
                                {
                                    return;
                                }
                            }
                        }
                    }

                    WebClient client = new WebClient();
                    byte[] data = client.DownloadData(ServiceCore.ConfigurationServer + "/assets/" + shaFingerprint + "/" + fileName);
                    client.Dispose();

                    if (data != null)
                    {
                        Logging.Print("file " + fileName + " downloaded");

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
                    else
                    {
                        Logging.Warning("file " + fileName + " not exist");
                    }
                }
            });

            File.WriteAllText("Assets/fingerprint.json", fingerprint);
            Logging.Print("Download completed");
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

            string resourceFile = WebManager.DownloadConfigString("/res/resources.json");

            if (resourceFile != null)
            {
                LogicJSONObject jsonObject = (LogicJSONObject) LogicJSONParser.Parse(resourceFile);
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