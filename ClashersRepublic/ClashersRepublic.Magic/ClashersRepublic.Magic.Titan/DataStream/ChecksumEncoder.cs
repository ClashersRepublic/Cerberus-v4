namespace ClashersRepublic.Magic.Titan.DataStream
{
    using ClashersRepublic.Magic.Titan.Debug;
    using ClashersRepublic.Magic.Titan.Math;

    public class ChecksumEncoder
    {
        private int _checksum;
        private int _snapshotChecksum;

        private bool _enabled;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ChecksumEncoder" /> class.
        /// </summary>
        public ChecksumEncoder()
        {
            this._enabled = true;
        }

        /// <summary>
        ///     Enables the checksum.
        /// </summary>
        public void EnableCheckSum(bool enable)
        {
            if (!this._enabled || enable)
            {
                if (!this._enabled && enable)
                {
                    this._checksum = this._snapshotChecksum;
                }

                this._enabled = enable;
            }
            else
            {
                this._snapshotChecksum = this._checksum;
                this._enabled = false;
            }
        }

        /// <summary>
        ///     Reset this checksum.
        /// </summary>
        public void ResetCheckSum()
        {
            this._checksum = 0;
        }

        /// <summary>
        ///     Writes a boolean value.
        /// </summary>
        public virtual void WriteBoolean(bool value)
        {
            this._checksum = ((value ? 13 : 7) + (this._checksum >> 31)) | (this._checksum << (32 - 31));
        }

        /// <summary>
        ///     Writes a byte value.
        /// </summary>
        public virtual void WriteByte(byte value)
        {
            this._checksum = (value + (this._checksum >> 31)) | ((this._checksum << (32 - 31)) + 11);
        }

        /// <summary>
        ///     Writes a short value.
        /// </summary>
        public virtual void WriteShort(short value)
        {
            this._checksum = (value + (this._checksum >> 31)) | ((this._checksum << (32 - 31)) + 19);
        }

        /// <summary>
        ///     Writes a int value.
        /// </summary>
        public virtual void WriteInt(int value)
        {
            this._checksum = (value + (this._checksum >> 31)) | ((this._checksum << (32 - 31)) + 9);
        }

        /// <summary>
        ///     Writes a variable int value.
        /// </summary>
        public virtual void WriteVInt(int value)
        {
            this._checksum = (value + (this._checksum >> 31)) | ((this._checksum << (32 - 31)) + 33);
        }

        /// <summary>
        ///     Writes a long value.
        /// </summary>
        public virtual void WriteLong(LogicLong value)
        {
            value.Encode(this);
        }

        /// <summary>
        ///     Writes a byte array value.
        /// </summary>
        public virtual void WriteBytes(byte[] value, int length)
        {
            this._checksum = ((value != null ? length + 28 : 27) + (this._checksum >> 31)) | (this._checksum << (32 - 31));
        }

        /// <summary>
        ///     Writes a string value.
        /// </summary>
        public virtual void WriteString(string value)
        {
            this._checksum = ((value != null ? value.Length + 28 : 27) + (this._checksum >> 31)) | (this._checksum << (32 - 31));
        }

        /// <summary>
        ///     Writes a string reference value.
        /// </summary>
        public virtual void WriteStringReference(string value)
        {
            this._checksum = (value.Length + 38 + (this._checksum >> 31)) | (this._checksum << (32 - 31));
        }

        /// <summary>
        ///     Gets the hash code of this instance.
        /// </summary>
        public virtual int HashCode()
        {
            Debugger.Error("ChecksumEncoder hashCode not designed");
            return 42;
        }

        /// <summary>
        ///     Gets a value indicating whether the checksum mode is enabled.
        /// </summary>
        public bool IsCheckSumEnabled()
        {
            return this._enabled;
        }

        /// <summary>
        ///     Gets a value indicating whether this encoder is a only checksum mode.
        /// </summary>
        public virtual bool IsCheckSumOnlyMode()
        {
            return true;
        }

        /// <summary>
        ///     Gets a value indicating whether this instance is equal to the specified instance.
        /// </summary>
        public bool Equals(ChecksumEncoder encoder)
        {
            if (encoder != null)
            {
                int checksum = encoder._checksum;
                int checksum2 = this._checksum;

                if (!encoder._enabled)
                {
                    checksum = encoder._snapshotChecksum;
                }

                if (!this._enabled)
                {
                    checksum2 = this._snapshotChecksum;
                }

                return checksum == checksum2;
            }

            return false;
        }

        /// <summary>
        ///     Destructes this instance.
        /// </summary>
        ~ChecksumEncoder()
        {
            this._enabled = false;
            this._checksum = 0;
            this._snapshotChecksum = 0;
        }
    }
}