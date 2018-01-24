namespace ClashersRepublic.Magic.Titan.DataStream
{
    using System;
    using System.Text;
    using ClashersRepublic.Magic.Titan.Debug;
    using ClashersRepublic.Magic.Titan.Math;

    public class ByteStream : ChecksumEncoder
    {
        private int _bitIdx;

        private byte[] _buffer;
        private int _length;
        private int _offset;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ByteStream" /> class.
        /// </summary>
        public ByteStream(int capacity)
        {
            this._buffer = new byte[capacity];
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ByteStream" /> class.
        /// </summary>
        public ByteStream(byte[] buffer)
        {
            this._length = buffer.Length;
            this._buffer = new byte[this._length];
            Array.Copy(buffer, this._buffer, this._length);
        }

        /// <summary>
        ///     Gets the length of this stream.
        /// </summary>
        public int Length
        {
            get
            {
                if (this._offset < this._length)
                {
                    return this._length;
                }

                return this._offset;
            }
        }

        /// <summary>
        ///     Gets the length of this stream.
        /// </summary>
        public bool IsAtEnd
        {
            get
            {
                return this._offset >= this._length;
            }
        }

        /// <summary>
        ///     Clear this instance.
        /// </summary>
        public void Clear(int capacity)
        {
            this._buffer = new byte[capacity];
        }

        /// <summary>
        ///     Gets the byte array.
        /// </summary>
        public byte[] GetBytes()
        {
            byte[] array = new byte[this._offset];
            Array.Copy(this._buffer, array, this._offset);
            return array;
        }

        /// <summary>
        ///     Reads a boolean value.
        /// </summary>
        public bool ReadBoolean()
        {
            if (this._bitIdx == 0)
            {
                ++this._offset;
            }

            bool value = (this._buffer[this._offset - 1] & (1 << this._bitIdx)) != 0;
            this._bitIdx = (this._bitIdx + 1) & 7;
            return value;
        }

        /// <summary>
        ///     Reads a byte value.
        /// </summary>
        public byte ReadByte()
        {
            this._bitIdx = 0;
            this._offset += 1;

            return this._buffer[this._offset - 1];
        }

        /// <summary>
        ///     Reads a short value.
        /// </summary>
        public short ReadShort()
        {
            this._bitIdx = 0;
            this._offset += 2;

            return (short) (this._buffer[this._offset - 1] | (this._buffer[this._offset - 2] << 8));
        }

        /// <summary>
        ///     Reads a int value.
        /// </summary>
        public int ReadInt()
        {
            this._bitIdx = 0;
            this._offset += 4;

            return this._buffer[this._offset - 1] | (this._buffer[this._offset - 2] << 8) | (this._buffer[this._offset - 3] << 16) | (this._buffer[this._offset - 4] << 24);
        }

        /// <summary>
        ///     Reads a vint.
        /// </summary>
        public int ReadVInt()
        {
            this._bitIdx = 0;
            byte byteValue = this._buffer[this._offset++];
            int value = 0;

            if ((byteValue & 0x40) != 0)
            {
                value |= byteValue & 0x3F;

                if ((byteValue & 0x80) != 0)
                {
                    value |= ((byteValue = this._buffer[this._offset++]) & 0x7F) << 6;

                    if ((byteValue & 0x80) != 0)
                    {
                        value |= ((byteValue = this._buffer[this._offset++]) & 0x7F) << 13;

                        if ((byteValue & 0x80) != 0)
                        {
                            value |= ((byteValue = this._buffer[this._offset++]) & 0x7F) << 20;

                            if ((byteValue & 0x80) != 0)
                            {
                                value |= ((byteValue = this._buffer[this._offset++]) & 0x7F) << 27;
                                return (int) (value | 0x80000000);
                            }

                            return (int) (value | 0xF8000000);
                        }

                        return (int) (value | 0xFFF00000);
                    }

                    return (int) (value | 0xFFFFE000);
                }

                return (int) (value | 0xFFFFFFC0);
            }

            value |= byteValue & 0x3F;

            if ((byteValue & 0x80) != 0)
            {
                value |= ((byteValue = this._buffer[this._offset++]) & 0x7F) << 6;

                if ((byteValue & 0x80) != 0)
                {
                    value |= ((byteValue = this._buffer[this._offset++]) & 0x7F) << 13;

                    if ((byteValue & 0x80) != 0)
                    {
                        value |= ((byteValue = this._buffer[this._offset++]) & 0x7F) << 20;

                        if ((byteValue & 0x80) != 0)
                        {
                            value |= ((byteValue = this._buffer[this._offset++]) & 0x7F) << 27;
                        }
                    }
                }
            }

            return value;
        }

        /// <summary>
        ///     Reads a long value.
        /// </summary>
        public LogicLong ReadLong()
        {
            LogicLong logicLong = new LogicLong();
            logicLong.Decode(this);
            return logicLong;
        }

        /// <summary>
        ///     Reads a long value.
        /// </summary>
        public LogicLong ReadLong(LogicLong longValue)
        {
            longValue.Decode(this);
            return longValue;
        }

        /// <summary>
        ///     Reads the byte array length.
        /// </summary>
        public int ReadBytesLength()
        {
            this._bitIdx = 0;
            this._offset += 4;

            return this._buffer[this._offset - 1] | (this._buffer[this._offset - 2] << 8) | (this._buffer[this._offset - 3] << 16) | (this._buffer[this._offset - 4] << 24);
        }

        /// <summary>
        ///     Reads a byte array.
        /// </summary>
        public byte[] ReadBytes(int length, int maxCapacity)
        {
            this._bitIdx = 0;

            if (length <= -1)
            {
                if (length != -1)
                {
                    Debugger.Warning("Negative readBytes length encountered.");
                }

                return null;
            }

            if (length <= maxCapacity)
            {
                byte[] array = new byte[length];
                Array.Copy(this._buffer, this._offset, array, 0, length);
                this._offset += length;
                return array;
            }

            Debugger.Warning("readBytes too long array, max " + maxCapacity);

            return null;
        }

        /// <summary>
        ///     Reads a string value.
        /// </summary>
        public string ReadString(int maxCapacity)
        {
            this._bitIdx = 0;
            this._offset += 4;

            int length = this._buffer[this._offset - 1] | (this._buffer[this._offset - 2] << 8) | (this._buffer[this._offset - 3] << 16) | (this._buffer[this._offset - 4] << 24);

            if (length <= -1)
            {
                if (length != -1)
                {
                    Debugger.Warning("Negative String length encountered.");
                }

                return null;
            }

            if (length <= maxCapacity)
            {
                string value = Encoding.UTF8.GetString(this._buffer, this._offset, length);
                this._offset += length;
                return value;
            }

            Debugger.Warning("Too long String encountered, max " + maxCapacity);

            return null;
        }

        /// <summary>
        ///     Reads a string value.
        /// </summary>
        public string ReadStringReference(int maxCapacity)
        {
            this._bitIdx = 0;
            this._offset += 4;

            int length = this._buffer[this._offset - 1] | (this._buffer[this._offset - 2] << 8) | (this._buffer[this._offset - 3] << 16) | (this._buffer[this._offset - 4] << 24);

            if (length <= -1)
            {
                Debugger.Warning("Negative String length encountered.");
                return string.Empty;
            }

            if (length <= maxCapacity)
            {
                string value = Encoding.UTF8.GetString(this._buffer, this._offset, length);
                this._offset += length;
                return value;
            }

            Debugger.Warning("Too long String encountered, max " + maxCapacity);

            return null;
        }

        /// <summary>
        ///     Writes a boolean value.
        /// </summary>
        public override void WriteBoolean(bool value)
        {
            base.WriteBoolean(value);

            if (this._bitIdx == 0)
            {
                this.EnsureCapacity(1);
                ++this._offset;
            }

            if (value)
            {
                this._buffer[this._offset - 1] |= (byte) (1 << this._bitIdx);
            }

            this._bitIdx = (this._bitIdx + 1) & 7;
        }

        /// <summary>
        ///     Writes a byte value.
        /// </summary>
        public override void WriteByte(byte value)
        {
            base.WriteByte(value);

            this.EnsureCapacity(1);

            this._bitIdx = 0;
            this._offset += 1;

            this._buffer[this._offset - 1] = value;
        }

        /// <summary>
        ///     Writes a short value.
        /// </summary>
        public override void WriteShort(short value)
        {
            base.WriteShort(value);

            this.EnsureCapacity(2);

            this._bitIdx = 0;
            this._offset += 2;

            this._buffer[this._offset - 1] = (byte) value;
            this._buffer[this._offset - 2] = (byte) (value >> 8);
        }

        /// <summary>
        ///     Writes a int value.
        /// </summary>
        public override void WriteInt(int value)
        {
            base.WriteInt(value);

            this.EnsureCapacity(4);

            this._bitIdx = 0;
            this._offset += 4;

            this._buffer[this._offset - 1] = (byte) value;
            this._buffer[this._offset - 2] = (byte) (value >> 8);
            this._buffer[this._offset - 3] = (byte) (value >> 16);
            this._buffer[this._offset - 4] = (byte) (value >> 24);
        }

        /// <summary>
        ///     Writes a variable int value.
        /// </summary>
        public override void WriteVInt(int value)
        {
            base.WriteVInt(value);

            this.EnsureCapacity(5);

            this._bitIdx = 0;

            if (value >= 0)
            {
                if (value >= 64)
                {
                    if (value >= 0x2000)
                    {
                        if (value >= 0x100000)
                        {
                            if (value >= 0x8000000)
                            {
                                this._offset += 5;

                                this._buffer[this._offset - 5] = (byte) ((value & 0x3F) | 0x80);
                                this._buffer[this._offset - 4] = (byte) (((value >> 6) & 0x7F) | 0x80);
                                this._buffer[this._offset - 3] = (byte) (((value >> 13) & 0x7F) | 0x80);
                                this._buffer[this._offset - 2] = (byte) (((value >> 20) & 0x7F) | 0x80);
                                this._buffer[this._offset - 1] = (byte) ((value >> 27) & 0xF);

                                return;
                            }

                            this._offset += 4;

                            this._buffer[this._offset - 4] = (byte) ((value & 0x3F) | 0x80);
                            this._buffer[this._offset - 3] = (byte) (((value >> 6) & 0x7F) | 0x80);
                            this._buffer[this._offset - 2] = (byte) (((value >> 13) & 0x7F) | 0x80);
                            this._buffer[this._offset - 1] = (byte) ((value >> 20) & 0x7F);

                            return;
                        }

                        this._offset += 3;

                        this._buffer[this._offset - 3] = (byte) ((value & 0x3F) | 0x80);
                        this._buffer[this._offset - 2] = (byte) (((value >> 6) & 0x7F) | 0x80);
                        this._buffer[this._offset - 1] = (byte) ((value >> 13) & 0x7F);

                        return;
                    }

                    this._offset += 2;

                    this._buffer[this._offset - 2] = (byte) ((value & 0x3F) | 0x80);
                    this._buffer[this._offset - 1] = (byte) ((value >> 6) & 0x7F);

                    return;
                }

                this._offset += 1;

                this._buffer[this._offset - 1] = (byte) (value & 0x3F);
            }
            else
            {
                if (value <= -0x40)
                {
                    if (value <= -0x2000)
                    {
                        if (value <= -0x100000)
                        {
                            if (value <= -0x8000000)
                            {
                                this._offset += 5;

                                this._buffer[this._offset - 5] = (byte) ((value & 0x3F) | 0xC0);
                                this._buffer[this._offset - 4] = (byte) (((value >> 6) & 0x7F) | 0x80);
                                this._buffer[this._offset - 3] = (byte) (((value >> 13) & 0x7F) | 0x80);
                                this._buffer[this._offset - 2] = (byte) (((value >> 20) & 0x7F) | 0x80);
                                this._buffer[this._offset - 1] = (byte) ((value >> 27) & 0xF);

                                return;
                            }

                            this._offset += 4;

                            this._buffer[this._offset - 4] = (byte) ((value & 0x3F) | 0xC0);
                            this._buffer[this._offset - 3] = (byte) (((value >> 6) & 0x7F) | 0x80);
                            this._buffer[this._offset - 2] = (byte) (((value >> 13) & 0x7F) | 0x80);
                            this._buffer[this._offset - 1] = (byte) (((value >> 20) & 0x7F) | 0x80);

                            return;
                        }

                        this._offset += 3;

                        this._buffer[this._offset - 3] = (byte) ((value & 0x3F) | 0xC0);
                        this._buffer[this._offset - 2] = (byte) (((value >> 6) & 0x7F) | 0x80);
                        this._buffer[this._offset - 1] = (byte) (((value >> 13) & 0x7F) | 0x80);

                        return;
                    }

                    this._offset += 2;

                    this._buffer[this._offset - 2] = (byte) ((value & 0x3F) | 0xC0);
                    this._buffer[this._offset - 1] = (byte) (((value >> 6) & 0x7F) | 0x80);

                    return;
                }

                this._offset += 1;

                this._buffer[this._offset - 1] = (byte) ((value & 0x3F) | 0x40);
            }
        }

        /// <summary>
        ///     Writes a int value to byte array.
        /// </summary>
        public void WriteIntToByteArray(int value)
        {
            this._bitIdx = 0;
            this._offset += 4;

            this.EnsureCapacity(4);

            this._buffer[this._offset - 1] = (byte) value;
            this._buffer[this._offset - 2] = (byte) (value >> 8);
            this._buffer[this._offset - 3] = (byte) (value >> 16);
            this._buffer[this._offset - 4] = (byte) (value >> 24);
        }

        /// <summary>
        ///     Writes a byte array value.
        /// </summary>
        public override void WriteBytes(byte[] value, int length)
        {
            base.WriteBytes(value, length);

            this._bitIdx = 0;

            if (value == null)
            {
                this.WriteIntToByteArray(-1);
            }
            else
            {
                this.EnsureCapacity(length + 4);
                this.WriteIntToByteArray(length);

                Array.Copy(value, 0, this._buffer, this._offset, length);

                this._offset += length;
            }
        }

        /// <summary>
        ///     Writes a string value.
        /// </summary>
        public override void WriteString(string value)
        {
            base.WriteString(value);

            if (value == null)
            {
                this.WriteIntToByteArray(-1);
            }
            else
            {
                int length = value.Length;

                if (length <= 900000)
                {
                    this.EnsureCapacity(length + 4);
                    this.WriteIntToByteArray(length);

                    Encoding.UTF8.GetBytes(value, 0, length, this._buffer, this._offset);

                    this._offset += length;
                }
                else
                {
                    Debugger.Warning("ByteStream::writeString invalid string length " + length);
                    this.WriteIntToByteArray(-1);
                }
            }
        }

        /// <summary>
        ///     Writes a string reference value.
        /// </summary>
        public override void WriteStringReference(string value)
        {
            base.WriteStringReference(value);

            int length = value.Length;

            if (length <= 900000)
            {
                this.EnsureCapacity(length + 4);
                this.WriteIntToByteArray(value.Length);

                Encoding.UTF8.GetBytes(value, 0, length, this._buffer, this._offset);

                this._offset += length;
            }
            else
            {
                Debugger.Warning("ByteStream::writeString invalid string length " + length);
                this.WriteIntToByteArray(-1);
            }
        }

        /// <summary>
        ///     Sets the buffer.
        /// </summary>
        public void SetByteArray(byte[] buffer, int length)
        {
            this._offset = 0;
            this._bitIdx = 0;
            this._buffer = buffer;
            this._length = length;
        }

        /// <summary>
        ///     Sets the offset.
        /// </summary>
        public void SetOffset(int offset)
        {
            this._offset = offset;
            this._bitIdx = 0;
        }

        /// <summary>
        ///     Removes the byte array.
        /// </summary>
        public byte[] RemoveByteArray()
        {
            byte[] byteArray = this._buffer;
            this._buffer = null;
            return byteArray;
        }

        /// <summary>
        ///     Ensures the capacity of byte array.
        /// </summary>
        public void EnsureCapacity(int capacity)
        {
            int bufferLength = this._buffer.Length;

            if (this._offset + capacity > bufferLength)
            {
                byte[] tmpBuffer = new byte[this._buffer.Length + capacity + 100];
                Array.Copy(this._buffer, tmpBuffer, bufferLength);
                this._buffer = tmpBuffer;
            }
        }

        /// <summary>
        ///     Destructes this instance.
        /// </summary>
        ~ByteStream()
        {
            this._buffer = null;
            this._bitIdx = 0;
            this._length = 0;
            this._offset = 0;
        }
    }
}