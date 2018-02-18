namespace ClashersRepublic.Magic.Logic.Helper
{
    using System;
    using System.Linq;

    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.Utils;
    using ClashersRepublic.Magic.Titan.DataStream;

    public static class ByteStreamHelper
    {
        /// <summary>
        ///     Reads a compressable string or null.
        /// </summary>
        public static LogicCompressibleString ReadCompressableStringOrNull(this ByteStream stream)
        {
            if (stream.ReadBoolean())
            {
                return null;
            }

            LogicCompressibleString compressibleString = new LogicCompressibleString();
            compressibleString.Decode(stream);
            return compressibleString;
        }

        /// <summary>
        ///     Reads a data reference.
        /// </summary>
        public static LogicData ReadDataReference(this ByteStream stream)
        {
            return LogicDataTables.GetDataById(stream.ReadInt());
        }

        /// <summary>
        ///     Reads a data reference.
        /// </summary>
        public static LogicData ReadDataReference(this ByteStream stream, int tableIndex)
        {
            int globalId = stream.ReadInt();

            if (GlobalID.GetClassID(globalId) == tableIndex + 1)
            {
                return LogicDataTables.GetDataById(globalId);
            }

            return null;
        }

        /// <summary>
        ///     Writes a compressable string or null.
        /// </summary>
        public static void WriteCompressableStringOrNull(this ChecksumEncoder encoder, LogicCompressibleString compressibleString)
        {
            if (compressibleString == null)
            {
                encoder.WriteBoolean(false);
            }
            else
            {
                encoder.WriteBoolean(true);
                compressibleString.Encode(encoder);
            }
        }

        /// <summary>
        ///     Writes the specified data to stream.
        /// </summary>
        public static void WriteDataReference(this ChecksumEncoder encoder, LogicData data)
        {
            encoder.WriteInt(data != null ? data.GetGlobalID() : 0);
        }

        public static void WriteHexa(this ChecksumEncoder encoder, string hexa)
        {
            hexa = hexa.Replace("-", string.Empty);

            byte[] bytes = Enumerable.Range(0, hexa.Length)
                .Where(x => x % 2 == 0)
                .Select(x => Convert.ToByte(hexa.Substring(x, 2), 16))
                .ToArray();

            for (int i = 0; i < bytes.Length; i++)
            {
                encoder.WriteByte(bytes[i]);
            }
        }
    }
}