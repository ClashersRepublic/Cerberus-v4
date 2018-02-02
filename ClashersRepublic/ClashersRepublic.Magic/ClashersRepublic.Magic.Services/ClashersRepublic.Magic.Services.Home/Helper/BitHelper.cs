namespace ClashersRepublic.Magic.Services.Home.Helper
{
    using System;
    using System.Linq;

    internal static class BitHelper
    {
        internal static byte[] ToByteArray(this string hex)
        {
            return Enumerable.Range(0, hex.Length)
                             .Where(x => x % 2 == 0)
                             .Select(x => Convert.ToByte(hex.Substring(x, 2), 16))
                             .ToArray();
        }
    }
}