namespace LineageSoft.Magic.Titan.Libs.ZLib
{
    using System.IO;

    /// <summary>
    ///     Classes that simplify a common use of compression streams
    /// </summary>
    internal delegate DeflateStream CreateStreamDelegate(Stream s, CompressionMode mode, CompressionLevel level, bool leaveOpen);

    #region DeflateCompressor

    public static class DeflateCompressor
    {
        public static MemoryStream Compress(Stream source, CompressionLevel level = CompressionLevel.Default)
        {
            return CommonCompressor.Compress(DeflateCompressor.CreateStream, source, level);
        }

        public static MemoryStream Decompress(Stream source)
        {
            return CommonCompressor.Decompress(DeflateCompressor.CreateStream, source);
        }

        public static byte[] Compress(byte[] source, CompressionLevel level = CompressionLevel.Default)
        {
            return CommonCompressor.Compress(DeflateCompressor.CreateStream, source, level);
        }

        public static byte[] Decompress(byte[] source)
        {
            return CommonCompressor.Decompress(DeflateCompressor.CreateStream, source);
        }

        private static DeflateStream CreateStream(Stream s, CompressionMode mode, CompressionLevel level, bool leaveOpen)
        {
            return new DeflateStream(s, mode, level, leaveOpen);
        }
    }

    #endregion

    #region ZLibCompressor

    public static class ZLibCompressor
    {
        public static MemoryStream Compress(Stream source, CompressionLevel level = CompressionLevel.Default)
        {
            return CommonCompressor.Compress(ZLibCompressor.CreateStream, source, level);
        }

        public static MemoryStream Decompress(Stream source)
        {
            return CommonCompressor.Decompress(ZLibCompressor.CreateStream, source);
        }

        public static byte[] Compress(byte[] source, CompressionLevel level = CompressionLevel.Default)
        {
            return CommonCompressor.Compress(ZLibCompressor.CreateStream, source, level);
        }

        public static byte[] Decompress(byte[] source)
        {
            return CommonCompressor.Decompress(ZLibCompressor.CreateStream, source);
        }

        private static DeflateStream CreateStream(Stream s, CompressionMode mode, CompressionLevel level, bool leaveOpen)
        {
            return new ZLibStream(s, mode, level, leaveOpen);
        }
    }

    #endregion

    #region GZipCompressor

    public static class GZipCompressor
    {
        public static MemoryStream Compress(Stream source, CompressionLevel level = CompressionLevel.Default)
        {
            return CommonCompressor.Compress(GZipCompressor.CreateStream, source, level);
        }

        public static MemoryStream Decompress(Stream source)
        {
            return CommonCompressor.Decompress(GZipCompressor.CreateStream, source);
        }

        public static byte[] Compress(byte[] source, CompressionLevel level = CompressionLevel.Default)
        {
            return CommonCompressor.Compress(GZipCompressor.CreateStream, source, level);
        }

        public static byte[] Decompress(byte[] source)
        {
            return CommonCompressor.Decompress(GZipCompressor.CreateStream, source);
        }

        private static DeflateStream CreateStream(Stream s, CompressionMode mode, CompressionLevel level, bool leaveOpen)
        {
            return new GZipStream(s, mode, level, leaveOpen);
        }
    }

    #endregion

    #region CommonCompressor

    internal class CommonCompressor
    {
        private static void Compress(CreateStreamDelegate sc, Stream source, Stream dest, CompressionLevel level)
        {
            using (DeflateStream zsDest = sc(dest, CompressionMode.Compress, level, true))
            {
                source.CopyTo(zsDest);
            }
        }

        private static void Decompress(CreateStreamDelegate sc, Stream source, Stream dest)
        {
            // CompressionLevel.Default in CompressionMode.Decompress does not affect performance or efficiency
            using (DeflateStream zsSource = sc(source, CompressionMode.Decompress, CompressionLevel.Default, true))
            {
                zsSource.CopyTo(dest);
            }
        }

        public static MemoryStream Compress(CreateStreamDelegate sc, Stream source, CompressionLevel level = CompressionLevel.Default)
        {
            MemoryStream result = new MemoryStream();
            CommonCompressor.Compress(sc, source, result, level);
            result.Position = 0;
            return result;
        }

        public static MemoryStream Decompress(CreateStreamDelegate sc, Stream source)
        {
            MemoryStream result = new MemoryStream();
            CommonCompressor.Decompress(sc, source, result);
            result.Position = 0;
            return result;
        }

        public static byte[] Compress(CreateStreamDelegate sc, byte[] source, CompressionLevel level = CompressionLevel.Default)
        {
            using (MemoryStream srcStream = new MemoryStream(source))
            using (MemoryStream dstStream = CommonCompressor.Compress(sc, srcStream, level))
            {
                return dstStream.ToArray();
            }
        }

        public static byte[] Decompress(CreateStreamDelegate sc, byte[] source)
        {
            using (MemoryStream srcStream = new MemoryStream(source))
            using (MemoryStream dstStream = CommonCompressor.Decompress(sc, srcStream))
            {
                return dstStream.ToArray();
            }
        }
    }

    #endregion
}