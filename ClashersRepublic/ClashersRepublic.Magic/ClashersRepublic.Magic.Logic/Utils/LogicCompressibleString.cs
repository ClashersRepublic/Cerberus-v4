namespace ClashersRepublic.Magic.Logic.Utils
{
    using System;
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Titan.DataStream;
    using ClashersRepublic.Magic.Titan.Json;

    public class LogicCompressibleString
    {
        private string _stringValue;
        private byte[] _compressedData;
        private int _compressedLength;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicCompressibleString"/> class.
        /// </summary>
        public LogicCompressibleString()
        {
            // LogicCompressibleString.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public void Decode(ByteStream stream)
        {
            if (stream.ReadBoolean())
            {
                this._compressedLength = stream.ReadBytesLength();
                this._compressedData = stream.ReadBytes(this._compressedLength, 900000);
            }
            else
            {
                this._stringValue = stream.ReadString(900000);
            }
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public void Encode(ChecksumEncoder encoder)
        {
            if (this._compressedData != null)
            {
                encoder.WriteBoolean(true);
                encoder.WriteBytes(this._compressedData, this._compressedData.Length);
            }
            else
            {
                encoder.WriteBoolean(false);
                encoder.WriteString(this._stringValue);
            }
        }

        /// <summary>
        ///     Gets the string value.
        /// </summary>
        public string Get()
        {
            return this._stringValue;
        }

        /// <summary>
        ///     Sets the string value.
        /// </summary>
        public void Set(string value)
        {
            this.Set(value, null, 0);
        }

        /// <summary>
        ///     Sets the compressed bytes.
        /// </summary>
        public void Set(byte[] compressedBytes, int length)
        {
            this.Set(null, compressedBytes, length);
        }

        /// <summary>
        ///     Sets the string and compressed bytes.
        /// </summary>
        public void Set(string value, byte[] compressedBytes, int length)
        {
            this._stringValue = value;
            this._compressedData = null;
            this._compressedLength = length;

            if (compressedBytes != null)
            {
                this._compressedData = new byte[length <= 0 ? 0 : length];

                if (length > 0)
                {
                    Array.Copy(compressedBytes, this._compressedData, length);
                }
            }
        }

        /// <summary>
        ///     Gets the compressed length.
        /// </summary>
        public int GetCompressedLength()
        {
            return this._compressedLength;
        }

        /// <summary>
        ///     Gets if this instance is compressed.
        /// </summary>
        public bool IsCompressed()
        {
            return this._stringValue == null && this._compressedLength != 0;
        }

        /// <summary>
        ///     Removes the compressed bytes.
        /// </summary>
        public byte[] RemoveCompressed()
        {
            byte[] tmp = this._compressedData;
            this._compressedData = null;
            this._compressedLength = 0;
            return tmp;
        }

        /// <summary>
        ///     Saves this instance to json.
        /// </summary>
        public void Save(LogicJSONObject jsonObject)
        {
            if (this._stringValue != null)
            {
                jsonObject.Put("s", new LogicJSONString(this._stringValue));
            }

            if (this._compressedData != null)
            {
                jsonObject.Put("c", new LogicJSONString(Convert.ToBase64String(this._compressedData, 0, this._compressedLength)));
            }
        }

        /// <summary>
        ///     Loads this instance from json.
        /// </summary>
        public void Load(LogicJSONObject jsonObject)
        {
            LogicJSONString sString = jsonObject.GetJSONString("s");

            if (sString != null)
            {
                this._stringValue = sString.GetStringValue();
            }

            LogicJSONString cString = jsonObject.GetJSONString("c");

            if (cString != null)
            {
                this._compressedData = Convert.FromBase64String(cString.GetStringValue());
            }
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public void Destruct()
        {
            this._stringValue = null;
            this._compressedData = null;
        }
    }
}