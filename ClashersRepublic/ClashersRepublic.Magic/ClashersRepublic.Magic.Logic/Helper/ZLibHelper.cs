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
        public static int DecompressInMySQLFormat(byte[] input, int length, out byte[] output)
        {
            int decompressedLength = input[0] | (input[1] << 8) | (input[2] << 16) | (input[3] << 24);

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

            return decompressedLength;
        }

        /// <summary>
        ///     Compresses the specified input in
        /// </summary>
        public static int CompressInZLibFormat(byte[] input, out byte[] output)
        {
            byte[] compressed = ZLibCompressor.Compress(input, CompressionLevel.Level1);
            int compressedLength = compressed.Length;
            int uncompressedLength = input.Length;

            output = new byte[compressedLength + 4];
            output[0] = (byte) uncompressedLength;
            output[1] = (byte) (uncompressedLength >> 8);
            output[2] = (byte) (uncompressedLength >> 16);
            output[3] = (byte) (uncompressedLength >> 24);

            Array.Copy(compressed, 0, output, 4, compressedLength);

            return output.Length;
        }
    }
}