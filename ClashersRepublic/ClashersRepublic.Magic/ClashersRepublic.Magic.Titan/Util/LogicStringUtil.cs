namespace ClashersRepublic.Magic.Titan.Util
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
    }
}