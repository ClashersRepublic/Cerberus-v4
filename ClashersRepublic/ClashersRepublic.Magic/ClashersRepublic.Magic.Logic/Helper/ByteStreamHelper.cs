namespace ClashersRepublic.Magic.Logic.Helper
{
    using System.Linq;

    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Titan.DataStream;

    public static class ByteStreamHelper
    {
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

            if (GlobalID.GetClassID(tableIndex) == tableIndex)
            {
                return LogicDataTables.GetDataById(globalId);
            }

            return null;
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

            byte[] bytes = System.Linq.Enumerable.Range(0, hexa.Length)
                .Where(x => x % 2 == 0)
                .Select(x => System.Convert.ToByte(hexa.Substring(x, 2), 16))
                .ToArray();

            for (int i = 0; i < bytes.Length; i++)
            {
                encoder.WriteByte(bytes[i]);
            }
        }
    }
}