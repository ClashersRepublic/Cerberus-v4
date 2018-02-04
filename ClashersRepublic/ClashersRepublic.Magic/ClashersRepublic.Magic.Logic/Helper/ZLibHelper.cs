namespace ClashersRepublic.Magic.Logic.Helper
{
    using System;
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
            byte[] compressed = ZLibCompressor.Compress(input, CompressionLevel.Level1);

            output = new byte[compressed.Length + 4];
            output[0] = (byte) (input.Length);
            output[1] = (byte) (input.Length >> 8);
            output[2] = (byte) (input.Length >> 16);
            output[3] = (byte) (input.Length >> 24);

            Array.Copy(compressed, 0, output, 4, compressed.Length);
        }
    }
}