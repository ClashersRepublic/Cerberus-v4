namespace ClashersRepublic.Magic.Titan.ZLib
{
    using System;

    public static class ZLibHelper
    {
        /// <summary>
        ///     Compresses the specified byte array.
        /// </summary>
        public static byte[] CompressByteArray(byte[] input)
        {
            int inputLength = input.Length;

            if (inputLength > 0)
            {
                byte[] compressedBuffer = ZlibStream.CompressBuffer(input);
                byte[] output = new byte[4 + compressedBuffer.Length];

                output[0] = (byte) inputLength;
                output[1] = (byte) (inputLength >> 8);
                output[2] = (byte) (inputLength >> 16);
                output[3] = (byte) (inputLength >> 24);

                Array.Copy(compressedBuffer, 0, output, 4, compressedBuffer.Length);

                return output;
            }

            return input;
        }

        /// <summary>
        ///     Compresses the specified string.
        /// </summary>
        public static byte[] CompressString(string input)
        {
            int inputLength = input.Length;

            if (inputLength > 0)
            {
                byte[] compressedBuffer = ZlibStream.CompressString(input);
                byte[] output = new byte[4 + compressedBuffer.Length];

                output[0] = (byte) inputLength;
                output[1] = (byte) (inputLength >> 8);
                output[2] = (byte) (inputLength >> 16);
                output[3] = (byte) (inputLength >> 24);

                Array.Copy(compressedBuffer, 0, output, 4, compressedBuffer.Length);

                return output;
            }

            return new byte[0];
        }

        /// <summary>
        ///     Decompresses the specified byte array.
        /// </summary>
        public static byte[] DecompressByteArray(byte[] input)
        {
            int inputLength = input.Length;

            if (inputLength > 4)
            {
                byte[] compressedBuffer = new byte[inputLength - 4];
                Array.Copy(input, 4, compressedBuffer, 0, compressedBuffer.Length);
                byte[] output = ZlibStream.UncompressBuffer(compressedBuffer);

                int decompressedSize = input[0] | (input[1] << 8) | (input[2] << 16) | (input[3] << 24);

                if (decompressedSize != output.Length)
                {
                    return null;
                }

                return output;
            }

            return input;
        }

        /// <summary>
        ///     Decompresses the specified byte array to string.
        /// </summary>
        public static string DecompressString(byte[] input)
        {
            int inputLength = input.Length;

            if (inputLength > 4)
            {
                byte[] compressedBuffer = new byte[inputLength - 4];
                Array.Copy(input, 4, compressedBuffer, 0, compressedBuffer.Length);
                string output = ZlibStream.UncompressString(compressedBuffer);

                int decompressedSize = input[0] | (input[1] << 8) | (input[2] << 16) | (input[3] << 24);

                if (decompressedSize != output.Length)
                {
                    return null;
                }

                return output;
            }

            return string.Empty;
        }
    }
}