namespace ClashersRepublic.Magic.Patcher
{
    using System;
    using System.IO;
    using System.Security.Cryptography;
    using ClashersRepublic.Magic.Titan.Json;

    using SevenZip;
    using SevenZip.Compression.LZMA;

    internal class FileAsset
    {
        private readonly string _filePath;

        private bool _isCompressed;

        /// <summary>
        ///     Gets the content of file.
        /// </summary>
        internal byte[] Content { get; private set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="FileAsset" /> class.
        /// </summary>
        internal FileAsset(string filePath, byte[] content)
        {
            this._filePath = filePath;
            this.Content = content;
        }

        /// <summary>
        ///     Compresses the file asset.
        /// </summary>
        internal void Compress()
        {
            if (this._filePath.EndsWith(".csv"))
            {
                Encoder compressor = new Encoder();

                using (MemoryStream iStream = new MemoryStream(this.Content))
                {
                    using (MemoryStream oStream = new MemoryStream())
                    {
                        CoderPropID[] coderPropIDs =
                        {
                            CoderPropID.DictionarySize, CoderPropID.PosStateBits, CoderPropID.LitContextBits, CoderPropID.LitPosBits,
                            CoderPropID.Algorithm, CoderPropID.NumFastBytes, CoderPropID.MatchFinder, CoderPropID.EndMarker
                        };

                        object[] properties =
                        {
                            262144, 2, 3, 0, 2, 32, "bt4", false
                        };

                        compressor.SetCoderProperties(coderPropIDs, properties);
                        compressor.WriteCoderProperties(oStream);

                        oStream.Write(BitConverter.GetBytes(iStream.Length), 0, 4);

                        compressor.Code(iStream, oStream, iStream.Length, -1L, null);

                        this.Content = oStream.ToArray();
                    }
                }
            }
            else if (this._filePath.EndsWith(".sc"))
            {
            }

            this._isCompressed = true;
        }

        /// <summary>
        ///     Gets the hash of content.
        /// </summary>
        internal string GetSha()
        {
            if (!this._isCompressed)
            {
                throw new InvalidOperationException();
            }

            using (SHA1 sha = new SHA1Managed())
            {
                return BitConverter.ToString(sha.ComputeHash(this.Content)).Replace("-", string.Empty).ToLower();
            }
        }

        /// <summary>
        ///     Writes to the specifed directory.
        /// </summary>
        internal void WriteTo(string output)
        {
            string filePath = output + "/" + this._filePath;

            Directory.CreateDirectory(Path.GetDirectoryName(filePath));
            File.WriteAllBytes(filePath, this.Content);
        }

        /// <summary>
        ///     Saves the file to json.
        /// </summary>
        internal LogicJSONObject SaveToJson()
        {
            LogicJSONObject jsonRoot = new LogicJSONObject();

            if (this._filePath.Contains("highres_tex"))
            {
                jsonRoot.Put("defer", new LogicJSONBoolean(true));
            }

            jsonRoot.Put("file", new LogicJSONString(this._filePath.Replace("\\", "/")));
            jsonRoot.Put("sha", new LogicJSONString(this.GetSha()));

            return jsonRoot;
        }
    }
}