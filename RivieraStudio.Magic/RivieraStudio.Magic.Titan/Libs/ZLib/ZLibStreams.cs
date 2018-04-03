namespace RivieraStudio.Magic.Titan.Libs.ZLib
{
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    #region DeflateStream

    /// <summary>Provides methods and properties used to compress and decompress streams.</summary>
    public class DeflateStream : Stream
    {
        private bool _success;

        private readonly CompressionMode _compMode;
        private readonly ZStream _zstream;
        private GCHandle _zstreamPtr;
        private readonly bool _leaveOpen;

        // Dispose Pattern
        private bool _zstreamDisposed;
        private bool _zstreamPtrDisposed;

        private const int WORK_DATA_SIZE = 0x1000;
        private readonly byte[] _workData = new byte[DeflateStream.WORK_DATA_SIZE];
        private int _workDataPos;

        public DeflateStream(Stream stream, CompressionMode mode)
            : this(stream, mode, CompressionLevel.Default, false)
        {
        }

        public DeflateStream(Stream stream, CompressionMode mode, bool leaveOpen) :
            this(stream, mode, CompressionLevel.Default, leaveOpen)
        {
        }

        public DeflateStream(Stream stream, CompressionMode mode, CompressionLevel level) :
            this(stream, mode, level, false)
        {
        }

        public DeflateStream(Stream stream, CompressionMode mode, CompressionLevel level, bool leaveOpen)
        {
            if (ZLibNative.Loaded == false)
            {
                ZLibNative.AssemblyInit();
            }

            this._zstream = new ZStream();
            this._zstreamPtr = GCHandle.Alloc(this._zstream, GCHandleType.Pinned);

            this._leaveOpen = leaveOpen;
            this.BaseStream = stream;
            this._compMode = mode;
            this._workDataPos = 0;

            int ret;
            if (this._compMode == CompressionMode.Compress)
            {
                ret = ZLibNative.DeflateInit(this._zstream, level, this.WriteType);
            }
            else
            {
                ret = ZLibNative.InflateInit(this._zstream, this.OpenType);
            }

            if (ret != ZLibReturnCode.OK)
            {
                throw new ZLibException(ret, this._zstream.LastErrorMsg);
            }

            this._success = true;
        }

        #region Disposable Pattern

        ~DeflateStream()
        {
            this.Dispose(false);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (this.BaseStream != null)
                {
                    if (this._compMode == CompressionMode.Compress && this._success)
                    {
                        this.Flush();
                    }

                    if (!this._leaveOpen)
                    {
                        this.BaseStream.Close();
                    }

                    this.BaseStream = null;
                }

                if (this._zstreamDisposed == false)
                {
                    if (this._compMode == CompressionMode.Compress)
                    {
                        ZLibNative.DeflateEnd(this._zstream);
                    }
                    else
                    {
                        ZLibNative.InflateEnd(this._zstream);
                    }

                    this._zstreamDisposed = true;
                }

                if (this._zstreamPtrDisposed == false)
                {
                    this._zstreamPtr.Free();
                    this._zstreamPtrDisposed = true;
                }
            }
        }

        #endregion

        protected virtual ZLibOpenType OpenType
        {
            get
            {
                return ZLibOpenType.Deflate;
            }
        }

        protected virtual ZLibWriteType WriteType
        {
            get
            {
                return ZLibWriteType.Deflate;
            }
        }

        internal static void ValidateReadWriteArgs(byte[] buffer, int offset, int count)
        {
            if (buffer == null)
            {
                throw new ArgumentNullException("buffer");
            }

            if (offset < 0)
            {
                throw new ArgumentException("[offset] must be positive integer or 0");
            }

            if (count < 0)
            {
                throw new ArgumentException("[count] must be positive integer or 0");
            }

            if (buffer.Length - offset < count)
            {
                throw new ArgumentException("[count + offset] should be longer than [buffer.Length]");
            }
        }

        /// <summary>Reads a number of decompressed bytes into the specified byte array.</summary>
        /// <param name="array">The array used to store decompressed bytes.</param>
        /// <param name="offset">The location in the array to begin reading.</param>
        /// <param name="count">The number of bytes decompressed.</param>
        /// <returns>
        ///     The number of bytes that were decompressed into the byte array. If the end of the stream has been reached,
        ///     zero or the number of bytes read is returned.
        /// </returns>
        public override int Read(byte[] buffer, int offset, int count)
        {
            if (this._compMode != CompressionMode.Decompress)
            {
                throw new NotSupportedException("Can't read on a compress stream!");
            }

            DeflateStream.ValidateReadWriteArgs(buffer, offset, count);

            int readLen = 0;
            if (this._workDataPos != -1)
            {
                using (PinnedArray workDataPtr = new PinnedArray(this._workData)) // [In] Compressed
                using (PinnedArray bufferPtr = new PinnedArray(buffer)) // [Out] Will-be-decompressed
                {
                    this._zstream.next_in = workDataPtr[this._workDataPos];
                    this._zstream.next_out = bufferPtr[offset];
                    this._zstream.avail_out = (uint) count;

                    while (0 < this._zstream.avail_out)
                    {
                        if (this._zstream.avail_in == 0)
                        {
                            // Compressed Data is no longer available in array, so read more from _stream
                            this._workDataPos = 0;
                            this._zstream.next_in = workDataPtr;
                            this._zstream.avail_in = (uint) this.BaseStream.Read(this._workData, 0, this._workData.Length);
                            this.TotalIn += this._zstream.avail_in;
                        }

                        uint inCount = this._zstream.avail_in;
                        uint outCount = this._zstream.avail_out;

                        // flush method for inflate has no effect
                        int zlibError = ZLibNative.Inflate(this._zstream, ZLibFlush.Z_NO_FLUSH);

                        this._workDataPos += (int) (inCount - this._zstream.avail_in);
                        readLen += (int) (outCount - this._zstream.avail_out);

                        if (zlibError == ZLibReturnCode.StreamEnd)
                        {
                            this._workDataPos = -1; // magic for StreamEnd
                            break;
                        }

                        if (zlibError != ZLibReturnCode.OK)
                        {
                            this._success = false;
                            throw new ZLibException(zlibError, this._zstream.LastErrorMsg);
                        }
                    }

                    this.TotalOut += readLen;
                }
            }

            return readLen;
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            if (this._compMode != CompressionMode.Compress)
            {
                throw new NotSupportedException("Can't write on a decompression stream!");
            }

            this.TotalIn += count;

            using (PinnedArray writePtr = new PinnedArray(this._workData))
            using (PinnedArray bufferPtr = new PinnedArray(buffer))
            {
                this._zstream.next_in = bufferPtr[offset];
                this._zstream.avail_in = (uint) count;
                this._zstream.next_out = writePtr[this._workDataPos];
                this._zstream.avail_out = (uint) (DeflateStream.WORK_DATA_SIZE - this._workDataPos);

                while (this._zstream.avail_in != 0)
                {
                    if (this._zstream.avail_out == 0)
                    {
                        this.BaseStream.Write(this._workData, 0, DeflateStream.WORK_DATA_SIZE);
                        this.TotalOut += DeflateStream.WORK_DATA_SIZE;
                        this._workDataPos = 0;
                        this._zstream.next_out = writePtr;
                        this._zstream.avail_out = DeflateStream.WORK_DATA_SIZE;
                    }

                    uint outCount = this._zstream.avail_out;

                    int zlibError = ZLibNative.Deflate(this._zstream, ZLibFlush.Z_NO_FLUSH);

                    this._workDataPos += (int) (outCount - this._zstream.avail_out);

                    if (zlibError != ZLibReturnCode.OK)
                    {
                        this._success = false;
                        throw new ZLibException(zlibError, this._zstream.LastErrorMsg);
                    }
                }
            }
        }

        /// <summary>Flushes the contents of the internal buffer of the current GZipStream object to the underlying stream.</summary>
        public override void Flush()
        {
            if (this._compMode != CompressionMode.Compress)
            {
                throw new NotSupportedException("Can't flush a decompression stream.");
            }

            using (PinnedArray workDataPtr = new PinnedArray(this._workData))
            {
                this._zstream.next_in = IntPtr.Zero;
                this._zstream.avail_in = 0;
                this._zstream.next_out = workDataPtr[this._workDataPos];
                this._zstream.avail_out = (uint) (DeflateStream.WORK_DATA_SIZE - this._workDataPos);

                int zlibError = ZLibReturnCode.OK;
                while (zlibError != ZLibReturnCode.StreamEnd)
                {
                    if (this._zstream.avail_out != 0)
                    {
                        uint outCount = this._zstream.avail_out;
                        zlibError = ZLibNative.Deflate(this._zstream, ZLibFlush.Z_FINISH);

                        this._workDataPos += (int) (outCount - this._zstream.avail_out);
                        if (zlibError != ZLibReturnCode.StreamEnd && zlibError != ZLibReturnCode.OK)
                        {
                            this._success = false;
                            throw new ZLibException(zlibError, this._zstream.LastErrorMsg);
                        }
                    }

                    this.BaseStream.Write(this._workData, 0, this._workDataPos);
                    this.TotalOut += this._workDataPos;
                    this._workDataPos = 0;
                    this._zstream.next_out = workDataPtr;
                    this._zstream.avail_out = DeflateStream.WORK_DATA_SIZE;
                }
            }

            this.BaseStream.Flush();
        }

        public long TotalIn { get; private set; }

        public long TotalOut { get; private set; }

        // The compression ratio obtained (same for compression/decompression).
        public double CompressionRatio
        {
            get
            {
                if (this._compMode == CompressionMode.Compress)
                {
                    return this.TotalIn == 0 ? 0.0 : 100.0 - this.TotalOut * 100.0 / this.TotalIn;
                }

                return this.TotalOut == 0 ? 0.0 : 100.0 - this.TotalIn * 100.0 / this.TotalOut;
            }
        }

        /// <summary>Gets a value indicating whether the stream supports reading while decompressing a file.</summary>
        public override bool CanRead
        {
            get
            {
                return this._compMode == CompressionMode.Decompress && this.BaseStream.CanRead;
            }
        }

        /// <summary>Gets a value indicating whether the stream supports writing.</summary>
        public override bool CanWrite
        {
            get
            {
                return this._compMode == CompressionMode.Compress && this.BaseStream.CanWrite;
            }
        }

        /// <summary>Gets a value indicating whether the stream supports seeking.</summary>
        public override bool CanSeek
        {
            get
            {
                return false;
            }
        }

        /// <summary>Gets a reference to the underlying stream.</summary>
        public Stream BaseStream { get; private set; }

        /// <summary>This property is not supported and always throws a NotSupportedException.</summary>
        /// <param name="offset">The location in the stream.</param>
        /// <param name="origin">One of the SeekOrigin values.</param>
        /// <returns>A long value.</returns>
        public override long Seek(long offset, SeekOrigin origin)
        {
            throw new NotSupportedException("Seek not supported");
        }

        /// <summary>This property is not supported and always throws a NotSupportedException.</summary>
        /// <param name="value">The length of the stream.</param>
        public override void SetLength(long value)
        {
            throw new NotSupportedException("SetLength not supported");
        }

        /// <summary>This property is not supported and always throws a NotSupportedException.</summary>
        public override long Length
        {
            get
            {
                throw new NotSupportedException("Length not supported.");
            }
        }

        /// <summary>This property is not supported and always throws a NotSupportedException.</summary>
        public override long Position
        {
            get
            {
                throw new NotSupportedException("Position not supported.");
            }
            set
            {
                throw new NotSupportedException("Position not supported.");
            }
        }
    }

    #endregion

    #region ZLibStream

    /// <summary>
    ///     zlib header + adler32 et end.
    ///     wraps a deflate stream
    /// </summary>
    public class ZLibStream : DeflateStream
    {
        public ZLibStream(Stream stream, CompressionMode mode)
            : base(stream, mode)
        {
        }

        public ZLibStream(Stream stream, CompressionMode mode, bool leaveOpen) :
            base(stream, mode, leaveOpen)
        {
        }

        public ZLibStream(Stream stream, CompressionMode mode, CompressionLevel level) :
            base(stream, mode, level)
        {
        }

        public ZLibStream(Stream stream, CompressionMode mode, CompressionLevel level, bool leaveOpen) :
            base(stream, mode, level, leaveOpen)
        {
        }

        protected override ZLibOpenType OpenType
        {
            get
            {
                return ZLibOpenType.ZLib;
            }
        }

        protected override ZLibWriteType WriteType
        {
            get
            {
                return ZLibWriteType.ZLib;
            }
        }
    }

    #endregion

    #region GZipStream

    /// <summary>
    ///     Saved to file (.gz) can be opened with zip utils.
    ///     Have hdr + crc32 at end.
    ///     Wraps a deflate stream
    /// </summary>
    public class GZipStream : DeflateStream
    {
        public GZipStream(Stream stream, CompressionMode mode)
            : base(stream, mode)
        {
        }

        public GZipStream(Stream stream, CompressionMode mode, bool leaveOpen)
            : base(stream, mode, leaveOpen)
        {
        }

        public GZipStream(Stream stream, CompressionMode mode, CompressionLevel level)
            : base(stream, mode, level)
        {
        }

        public GZipStream(Stream stream, CompressionMode mode, CompressionLevel level, bool leaveOpen)
            : base(stream, mode, level, leaveOpen)
        {
        }

        protected override ZLibOpenType OpenType
        {
            get
            {
                return ZLibOpenType.GZip;
            }
        }

        protected override ZLibWriteType WriteType
        {
            get
            {
                return ZLibWriteType.GZip;
            }
        }
    }

    #endregion
}