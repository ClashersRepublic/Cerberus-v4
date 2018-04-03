// LzInWindow.cs

namespace RivieraStudio.Magic.Services.Core.Libs.Lzma.Compression.LZ
{
    using System.IO;

    public class InWindow
    {
        public byte[] _bufferBase; // pointer to buffer with data
        private Stream _stream;
        private uint _posLimit; // offset (from _buffer) of first byte when new block reading must be done
        private bool _streamEndWasReached; // if (true) then _streamPos shows real end of stream

        private uint _pointerToLastSafePosition;

        public uint _bufferOffset;

        public uint _blockSize; // Size of Allocated memory block
        public uint _pos; // offset (from _buffer) of curent byte
        private uint _keepSizeBefore; // how many BYTEs must be kept in buffer before _pos
        private uint _keepSizeAfter; // how many BYTEs must be kept buffer after _pos
        public uint _streamPos; // offset (from _buffer) of first not read byte from Stream

        public void MoveBlock()
        {
            uint offset = this._bufferOffset + this._pos - this._keepSizeBefore;
            // we need one additional byte, since MovePos moves on 1 byte.
            if (offset > 0)
            {
                offset--;
            }

            uint numBytes = this._bufferOffset + this._streamPos - offset;

            // check negative offset ????
            for (uint i = 0; i < numBytes; i++)
            {
                this._bufferBase[i] = this._bufferBase[offset + i];
            }

            this._bufferOffset -= offset;
        }

        public virtual void ReadBlock()
        {
            if (this._streamEndWasReached)
            {
                return;
            }

            while (true)
            {
                int size = (int) (0 - this._bufferOffset + this._blockSize - this._streamPos);
                if (size == 0)
                {
                    return;
                }

                int numReadBytes = this._stream.Read(this._bufferBase, (int) (this._bufferOffset + this._streamPos), size);
                if (numReadBytes == 0)
                {
                    this._posLimit = this._streamPos;
                    uint pointerToPostion = this._bufferOffset + this._posLimit;
                    if (pointerToPostion > this._pointerToLastSafePosition)
                    {
                        this._posLimit = this._pointerToLastSafePosition - this._bufferOffset;
                    }

                    this._streamEndWasReached = true;
                    return;
                }

                this._streamPos += (uint) numReadBytes;
                if (this._streamPos >= this._pos + this._keepSizeAfter)
                {
                    this._posLimit = this._streamPos - this._keepSizeAfter;
                }
            }
        }

        private void Free()
        {
            this._bufferBase = null;
        }

        public void Create(uint keepSizeBefore, uint keepSizeAfter, uint keepSizeReserv)
        {
            this._keepSizeBefore = keepSizeBefore;
            this._keepSizeAfter = keepSizeAfter;
            uint blockSize = keepSizeBefore + keepSizeAfter + keepSizeReserv;
            if (this._bufferBase == null || this._blockSize != blockSize)
            {
                this.Free();
                this._blockSize = blockSize;
                this._bufferBase = new byte[this._blockSize];
            }

            this._pointerToLastSafePosition = this._blockSize - keepSizeAfter;
        }

        public void SetStream(Stream stream)
        {
            this._stream = stream;
        }

        public void ReleaseStream()
        {
            this._stream = null;
        }

        public void Init()
        {
            this._bufferOffset = 0;
            this._pos = 0;
            this._streamPos = 0;
            this._streamEndWasReached = false;
            this.ReadBlock();
        }

        public void MovePos()
        {
            this._pos++;
            if (this._pos > this._posLimit)
            {
                uint pointerToPostion = this._bufferOffset + this._pos;
                if (pointerToPostion > this._pointerToLastSafePosition)
                {
                    this.MoveBlock();
                }

                this.ReadBlock();
            }
        }

        public byte GetIndexByte(int index)
        {
            return this._bufferBase[this._bufferOffset + this._pos + index];
        }

        // index + limit have not to exceed _keepSizeAfter;
        public uint GetMatchLen(int index, uint distance, uint limit)
        {
            if (this._streamEndWasReached)
            {
                if (this._pos + index + limit > this._streamPos)
                {
                    limit = this._streamPos - (uint) (this._pos + index);
                }
            }

            distance++;
            // Byte *pby = _buffer + (size_t)_pos + index;
            uint pby = this._bufferOffset + this._pos + (uint) index;

            uint i;
            for (i = 0; i < limit && this._bufferBase[pby + i] == this._bufferBase[pby + i - distance]; i++)
            {
                ;
            }

            return i;
        }

        public uint GetNumAvailableBytes()
        {
            return this._streamPos - this._pos;
        }

        public void ReduceOffsets(int subValue)
        {
            this._bufferOffset += (uint) subValue;
            this._posLimit -= (uint) subValue;
            this._pos -= (uint) subValue;
            this._streamPos -= (uint) subValue;
        }
    }
}