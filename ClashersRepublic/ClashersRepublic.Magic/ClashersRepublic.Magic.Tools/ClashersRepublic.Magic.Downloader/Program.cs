namespace ClashersRepublic.Magic.Downloader
{
    using System;
    using System.Collections.Generic;
    using System.IO;
    using System.Net;
    using System.Threading.Tasks;

    using ClashersRepublic.Magic.Titan.Json;
    using SevenZip.Compression.LZMA;

    internal class Program
    {
        internal static WebClient Client;

        private static void Main(string[] args)
        {
            Program.Client = new WebClient();

            if (Directory.Exists("Assets"))
            {
                if (File.Exists("Assets/fingerprint.json"))
                {
                    Program.DownloadAssets(File.ReadAllText("Assets/fingerprint.json"));
                }
                else if (File.Exists("fingerprint.json"))
                {
                    Program.DownloadAssets(File.ReadAllText("fingerprint.json"));
                }
            }
            else if (File.Exists("fingerprint.json"))
            {
                Program.DownloadAssets(File.ReadAllText("fingerprint.json"));
            }
        }

        /// <summary>
        ///     Downloads all assets
        /// </summary>
        private static void DownloadAssets(string fingerprint)
        {
            Directory.CreateDirectory("dl");

            LogicJSONObject jsonObject = (LogicJSONObject) LogicJSONParser.Parse(fingerprint);
            LogicJSONString fingerprintSha = jsonObject.GetJSONString("sha");
            LogicJSONArray fileArray = jsonObject.GetJSONArray("files");

            List<Task> tasks = new List<Task>(fileArray.Size());

            for (int i = 0; i < fileArray.Size(); i++)
            {
                string path = fileArray.GetJSONObject(i).GetJSONString("file").GetStringValue();

                if (File.Exists("dl/" + path))
                {
                    tasks.Add(Task.Run(() => Program.DownloadContent(fingerprintSha.GetStringValue(), path)));
                }
            }

            File.WriteAllText("dl/fingerprint.json", fingerprint);
            Task.WaitAll(tasks.ToArray());
        }

        /// <summary>
        ///     Downloads the content.
        /// </summary>
        private static void DownloadContent(string fingerprintSha, string filePath)
        {
            byte[] content = new WebClient().DownloadData("http://b46f744d64acd2191eda-3720c0374d47e9a0dd52be4d281c260f.r11.cf2.rackcdn.com/" + fingerprintSha + "/" + filePath);

            if (content.Length > 0)
            {
                if (filePath.EndsWith(".csv"))
                {
                    using (MemoryStream inputStream = new MemoryStream(content))
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

                            content = outputStream.ToArray();
                        }
                    }
                }
            }

            Directory.CreateDirectory(Path.GetDirectoryName("dl/" + filePath));
            File.WriteAllBytes("dl/" + filePath, content);
            Console.WriteLine("File " + filePath + " downloaded");
        }
    }
}