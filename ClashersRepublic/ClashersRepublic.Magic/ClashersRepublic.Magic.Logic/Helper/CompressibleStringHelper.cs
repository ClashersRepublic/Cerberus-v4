namespace ClashersRepublic.Magic.Logic.Helper
{
    using ClashersRepublic.Magic.Logic.Utils;
    using ClashersRepublic.Magic.Titan.Util;

    public static class CompressibleStringHelper
    {
        /// <summary>
        ///     Uncompresses the <see cref="LogicCompressibleString" /> instance.
        /// </summary>
        public static void Uncompress(LogicCompressibleString str)
        {
            if (str.IsCompressed())
            {
                int compressedLength = str.GetCompressedLength();
                byte[] compressedData = str.RemoveCompressed();
                int uncompressedLength = ZLibHelper.DecompressInMySQLFormat(compressedData, compressedLength, out byte[] uncompressedData);

                if (uncompressedLength > 0)
                {
                    str.Set(LogicStringUtil.CreateString(uncompressedData, 0, uncompressedLength));
                }
            }
        }

        /// <summary>
        ///     Compresses the <see cref="LogicCompressibleString" /> instance.
        /// </summary>
        public static void Compress(LogicCompressibleString str)
        {
            string strInstance = str.Get();

            if (strInstance != null)
            {
                byte[] uncompressedData = LogicStringUtil.GetBytes(strInstance);
                int compressedLength = ZLibHelper.CompressInZLibFormat(uncompressedData, out byte[] compressedData);

                if (compressedLength > 0)
                {
                    str.Set(compressedData, compressedLength);
                }
            }
        }
    }
}