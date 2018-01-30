namespace ClashersRepublic.Magic.Logic.Utils
{
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Titan.DataStream;
    using ClashersRepublic.Magic.Titan.Json;

    public class LogicUnitSlot
    {
        private LogicData _data;

        private int _level;
        private int _count;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicUnitSlot"/> class.
        /// </summary>
        public LogicUnitSlot(LogicData data, int level, int count)
        {
            this._data = data;
            this._level = level;
            this._count = count;
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public void Decode(ByteStream stream)
        {
            this._data = stream.ReadDataReference();
            this._count = stream.ReadInt();
            this._level = stream.ReadInt();
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public void Encode(ChecksumEncoder encoder)
        {
            encoder.WriteDataReference(this._data);
            encoder.WriteInt(this._count);
            encoder.WriteInt(this._level);
        }

        /// <summary>
        ///     Gets the data instance.
        /// </summary>
        public LogicData GetData()
        {
            return this._data;
        }

        /// <summary>
        ///     Gets the count.
        /// </summary>
        public int GetCount()
        {
            return this._count;
        }

        /// <summary>
        ///     Gets the level.
        /// </summary>
        public int GetLevel()
        {
            return this._level;
        }

        /// <summary>
        ///     Gets the checksum of this instance.
        /// </summary>
        public void GetChecksum(ChecksumHelper checksumHelper)
        {
            checksumHelper.StartObject("LogicUnitSlot");

            if (this._data != null)
            {
                checksumHelper.WriteValue("globalID", this._data.GetGlobalID());
            }

            checksumHelper.WriteValue("m_level", this._level);
            checksumHelper.WriteValue("m_count", this._count);
            checksumHelper.EndObject();
        }

        /// <summary>
        ///     Sets the count.
        /// </summary>
        public void SetCount(int count)
        {
            this._count = count;
        }

        /// <summary>
        ///     Reads this instance from json.
        /// </summary>
        public void ReadFromJSON(LogicJSONObject jsonObject)
        {
            LogicJSONNumber id = jsonObject.GetJSONNumber("id");

            if (id != null && id.GetIntValue() != 0)
            {
                this._data = LogicDataTables.GetDataById(id.GetIntValue());
            }

            this._count = LogicJSONHelper.GetJSONNumber(jsonObject, "cnt");
            this._level = LogicJSONHelper.GetJSONNumber(jsonObject, "lvl");
        }

        /// <summary>
        ///     Writes this instance ti json.
        /// </summary>
        public void WriteToJSON(LogicJSONObject jsonObject)
        {
            jsonObject.Put("id", new LogicJSONNumber(this._data != null ? this._data.GetGlobalID() : 0));
            jsonObject.Put("cnt", new LogicJSONNumber(this._count));
            jsonObject.Put("lvl", new LogicJSONNumber(this._level));
        }
    }
}