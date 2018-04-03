// Common/CRC.cs

namespace RivieraStudio.Magic.Services.Core.Libs.Lzma
{
    internal class CRC
    {
        public static readonly uint[] Table;

        static CRC()
        {
            CRC.Table = new uint[256];
            const uint kPoly = 0xEDB88320;
            for (uint i = 0; i < 256; i++)
            {
                uint r = i;
                for (int j = 0; j < 8; j++)
                {
                    if ((r & 1) != 0)
                    {
                        r = (r >> 1) ^ kPoly;
                    }
                    else
                    {
                        r >>= 1;
                    }
                }

                CRC.Table[i] = r;
            }
        }

        private uint _value = 0xFFFFFFFF;

        public void Init()
        {
            this._value = 0xFFFFFFFF;
        }

        public void UpdateByte(byte b)
        {
            this._value = CRC.Table[(byte) this._value ^ b] ^ (this._value >> 8);
        }

        public void Update(byte[] data, uint offset, uint size)
        {
            for (uint i = 0; i < size; i++)
            {
                this._value = CRC.Table[(byte) this._value ^ data[offset + i]] ^ (this._value >> 8);
            }
        }

        public uint GetDigest()
        {
            return this._value ^ 0xFFFFFFFF;
        }

        private static uint CalculateDigest(byte[] data, uint offset, uint size)
        {
            CRC crc = new CRC();
            // crc.Init();
            crc.Update(data, offset, size);
            return crc.GetDigest();
        }

        private static bool VerifyDigest(uint digest, byte[] data, uint offset, uint size)
        {
            return CRC.CalculateDigest(data, offset, size) == digest;
        }
    }
}