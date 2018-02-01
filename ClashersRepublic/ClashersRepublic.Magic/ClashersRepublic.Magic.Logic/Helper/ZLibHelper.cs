namespace ClashersRepublic.Magic.Logic.Helper
{
    using System.IO;

    using ClashersRepublic.Magic.Titan.Debug;
    using ClashersRepublic.Magic.Titan.Libs.ZLib;

    public class ZLibHelper
    {
        /// <summary>
        ///     Decompresses the specified input in my sql format.
        /// </summary>
        public static void DecompressInMySQLFormat(byte[] input, int length, out byte[] output)
        {
            int decompressedLength = input[0] | input[1] << 8 | input[2] << 16 | input[3] << 24;

            using (MemoryStream inputStream = new MemoryStream(input, 4, input.Length - 4))
            {
                using (MemoryStream outputStream = ZLibCompressor.Decompress(inputStream))
                {
                    byte[] decompressedByteArray = outputStream.ToArray();

                    if (decompressedByteArray.Length != decompressedLength)
                    {
                        Debugger.Error("ZLibHelper::decompressInMySQLFormat decompressed byte array is corrupted");
                    }

                    output = decompressedByteArray;
                }
            }
        }

        /// <summary>
        ///     Compresses the specified input in 
        /// </summary>
        public static void ConpressInZLibFormat(byte[] input, out byte[] output)
        {
            using (MemoryStream outputStream = new MemoryStream())
            {
                outputStream.WriteByte((byte) (input.Length));
                outputStream.WriteByte((byte) (input.Length >> 8));
                outputStream.WriteByte((byte) (input.Length >> 16));
                outputStream.WriteByte((byte) (input.Length >> 24));

                byte[] compressed = ZLibCompressor.Compress(input, CompressionLevel.Default);

                outputStream.Write(compressed, 0, compressed.Length);

                output = outputStream.GetBuffer();
            }
        }
    }
}