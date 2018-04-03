namespace RivieraStudio.Magic.Logic.Util
{
    using RivieraStudio.Magic.Logic.Data;
    using RivieraStudio.Magic.Logic.Helper;
    using RivieraStudio.Magic.Titan.DataStream;
    using RivieraStudio.Magic.Titan.Json;

    public class LogicDataSlot
    {
        private LogicData _data;
        private int _count;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicDataSlot" /> class.
        /// </summary>
        public LogicDataSlot(LogicData data, int count)
        {
            this._data = data;
            this._count = count;
        }

        /// <summary>
        ///     Clones this instance.
        /// </summary>
        public LogicDataSlot Clone()
        {
            return new LogicDataSlot(this._data, this._count);
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public void Destruct()
        {
            this._data = null;
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public void Decode(ByteStream stream)
        {
            this._data = stream.ReadDataReference();
            this._count = stream.ReadInt();
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public void Encode(ChecksumEncoder encoder)
        {
            encoder.WriteDataReference(this._data);
            encoder.WriteInt(this._count);
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
        ///     Gets the checksum of this instance.
        /// </summary>
        public void GetChecksum(ChecksumHelper checksumHelper)
        {
            checksumHelper.StartObject("LogicDataSlot");

            if (this._data != null)
            {
                checksumHelper.WriteValue("globalID", this._data.GetGlobalID());
            }

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
        }

        /// <summary>
        ///     Writes this instance ti json.
        /// </summary>
        public void WriteToJSON(LogicJSONObject jsonObject)
        {
            jsonObject.Put("id", new LogicJSONNumber(this._data != null ? this._data.GetGlobalID() : 0));
            jsonObject.Put("cnt", new LogicJSONNumber(this._count));
        }
    }
}