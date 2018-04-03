namespace RivieraStudio.Magic.Tools.Patcher
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Security.Cryptography;

    using RivieraStudio.Magic.Patcher;
    using RivieraStudio.Magic.Titan.Json;

    internal class Program
    {
        private static List<FileAsset> _files;

        private static string _sha;
        private static string _currentSha;
        private static string _fingerprint;

        private static int[] _currentVersion;

        private static void Main(string[] args)
        {
            Program._files = new List<FileAsset>(2048);

            if (Directory.Exists("Assets"))
            {
                foreach (string filePath in Directory.GetFiles("Assets", "*.*", SearchOption.AllDirectories))
                {
                    if (filePath.EndsWith(".json"))
                    {
                        if (filePath.StartsWith("Assets\\level\\") || filePath.StartsWith("Assets\\offline\\") || filePath.StartsWith("Assets\\backup\\") || Path.GetFileName(filePath) == "fingerprint.json")
                        {
                            continue;
                        }
                    }

                    Program.LoadFile(filePath);
                }

                Program.LoadCurrentFingerprint();
                Program.Compress();
                Program.GenerateSha();
                Program.GenerateFingerprint();
                Program.WriteFiles();
            }
        }

        /// <summary>
        ///     Loads the specified file.
        /// </summary>
        private static void LoadFile(string path)
        {
            Program._files.Add(new FileAsset(path.Replace("Assets\\", string.Empty), File.ReadAllBytes(path)));
        }

        /// <summary>
        ///     Loads the current fingerprint version.
        /// </summary>
        private static void LoadCurrentFingerprint()
        {
            LogicJSONObject jsonObject = (LogicJSONObject) LogicJSONParser.Parse(File.ReadAllText("Assets/fingerprint.json"));
            LogicJSONString versionString = jsonObject.GetJSONString("version");

            if (!string.IsNullOrEmpty(versionString.GetStringValue()))
            {
                string[] version = versionString.GetStringValue().Split('.');

                if (version.Length == 3)
                {
                    Program._currentVersion = new int[3];

                    for (int i = 0; i < 3; i++)
                    {
                        Program._currentVersion[i] = int.Parse(version[i]);
                    }
                }
            }

            Program._currentSha = jsonObject.GetJSONString("sha").GetStringValue();
        }

        /// <summary>
        ///     Compresses all files.
        /// </summary>
        private static void Compress()
        {
            for (int i = 0; i < Program._files.Count; i++)
            {
                Program._files[i].Compress();
            }
        }

        /// <summary>
        ///     Generates the patch sha.
        /// </summary>
        private static void GenerateSha()
        {
            List<byte> contents = new List<byte>(4096 * Program._files.Count);

            for (int i = 0; i < Program._files.Count; i++)
            {
                contents.AddRange(Program._files[i].Content);
            }

            using (SHA1 sha = new SHA1Managed())
            {
                Program._sha = BitConverter.ToString(sha.ComputeHash(contents.ToArray())).Replace("-", string.Empty).ToLower();
            }
        }

        /// <summary>
        ///     Generates the fingerprint file.
        /// </summary>
        private static void GenerateFingerprint()
        {
            string version = Program._currentVersion[0] + "." + Program._currentVersion[1] + "." + (Program._currentVersion[2] + 1);

            LogicJSONObject jsonObject = new LogicJSONObject();
            LogicJSONArray fileArray = new LogicJSONArray(Program._files.Count);

            for (int i = 0; i < Program._files.Count; i++)
            {
                fileArray.Add(Program._files[i].SaveToJson());
            }

            jsonObject.Put("files", fileArray);
            jsonObject.Put("sha", new LogicJSONString(Program._sha));
            jsonObject.Put("version", new LogicJSONString(version));

            Program._fingerprint = LogicJSONParser.CreateJSONString(jsonObject);
        }

        /// <summary>
        ///     Writes all files to patch folder.
        /// </summary>
        private static void WriteFiles()
        {
            Directory.CreateDirectory("Patchs");
            Directory.CreateDirectory("Patchs/" + Program._sha);
            Directory.CreateDirectory("Assets/backup");

            for (int i = 0; i < Program._files.Count; i++)
            {
                Program._files[i].WriteTo("Patchs/" + Program._sha);
            }

            if (!File.Exists($"Assets/backup/fingerprint-{Program._currentSha}.json"))
            {
                File.Copy("Assets/fingerprint.json", $"Assets/backup/fingerprint-{Program._currentSha}.json");
            }

            File.WriteAllText("Patchs/" + Program._sha + "/fingerprint.json", Program._fingerprint);
            File.WriteAllText("Assets/fingerprint.json", Program._fingerprint);
        }
    }
}