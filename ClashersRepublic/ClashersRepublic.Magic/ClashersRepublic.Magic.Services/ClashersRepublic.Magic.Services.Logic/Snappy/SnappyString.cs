namespace ClashersRepublic.Magic.Services.Logic.Snappy
{
    using System.Text;
    using global::Snappy;

    public class SnappyString
    {
        private byte[] _compressedByteArray;

        /// <summary>
        ///     Initializes a new instance of the <see cref="SnappyString"/> class.
        /// </summary>
        public SnappyString() : this(string.Empty)
        {
            // SnappyString.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SnappyString"/> class.
        /// </summary>
        public SnappyString(string content)
        {
            this.Compress(content);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SnappyString"/> class.
        /// </summary>
        public SnappyString(byte[] compressed)
        {
            this._compressedByteArray = compressed;
        }

        /// <summary>
        ///     Compresses the string content.
        /// </summary>
        private void Compress(string content)
        {
            this._compressedByteArray = SnappyCodec.Compress(Encoding.UTF8.GetBytes(content));
        }

        /// <summary>
        ///     Uncompresses the compressed byte array.
        /// </summary>
        private string Uncompress()
        {
            return Encoding.UTF8.GetString(SnappyCodec.Uncompress(this._compressedByteArray));
        }

        /// <summary>
        ///     Updates the string content.
        /// </summary>
        public void Update(string content)
        {
            this.Compress(content);
        }

        /// <summary>
        ///     Gets the compressed array.
        /// </summary>
        public byte[] GetArray()
        {
            return this._compressedByteArray;
        }

        public static implicit operator SnappyString(string content)
        {
            return new SnappyString(content);
        }

        public static implicit operator string(SnappyString snappyString)
        {
            return snappyString.Uncompress();
        }
    }
}