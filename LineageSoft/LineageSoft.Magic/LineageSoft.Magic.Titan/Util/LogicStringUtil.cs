namespace LineageSoft.Magic.Titan.Util
{
    using System.Text;

    public static class LogicStringUtil
    {
        /// <summary>
        ///     Gets the byte array of string.
        /// </summary>
        public static byte[] GetBytes(string str)
        {
            return Encoding.UTF8.GetBytes(str);
        }

        /// <summary>
        ///     Converts the byte array to string.
        /// </summary>
        public static string CreateString(byte[] value, int offset, int length)
        {
            return Encoding.UTF8.GetString(value, offset, length);
        }
    }
}