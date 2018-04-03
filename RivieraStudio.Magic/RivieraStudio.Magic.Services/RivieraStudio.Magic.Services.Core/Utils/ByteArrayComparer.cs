namespace RivieraStudio.Magic.Services.Core.Utils
{
    using System.Collections.Generic;

    public class ByteArrayComparer : IEqualityComparer<byte[]>
    {
        public unsafe bool Equals(byte[] x, byte[] y)
        {
            int length = x.Length;

            fixed (byte* ap = x)
            fixed (byte* bp = y)
            {
                byte* a = ap;
                byte* b = bp;

                while (length >= 12)
                {
                    if (*(long*)a     != *(long*)b) return false;
                    if (*(long*)(a+4) != *(long*)(b+4)) return false;
                    if (*(long*)(a+8) != *(long*)(b+8)) return false;
                    a += 12; b += 12; length -= 12;
                }

                while (length > 0)
                {
                    if (*(int*)a != *(int*)b) break;
                    a += 2; b += 2; length -= 2;
                }

                return (length <= 0);
            }
        }

        public unsafe int GetHashCode(byte[] obj)
        {
            fixed (byte* src = obj)
            {
                int hash1 = 5381;
                int hash2 = hash1;
                byte* s = src;

                int c;

                while ((c = s[0]) != 0)
                {
                    hash1 = ((hash1 << 5) + hash1) ^ c;
                    c = s[1];
                    if (c == 0)
                        break;
                    hash2 = ((hash2 << 5) + hash2) ^ c;
                    s += 2;
                }

                return hash1 + (hash2 * 1566083941);
            }
        }
    }
}