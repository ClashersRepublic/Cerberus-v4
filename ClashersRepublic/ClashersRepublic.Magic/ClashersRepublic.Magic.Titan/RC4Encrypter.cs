namespace ClashersRepublic.Magic.Titan
{
    public class RC4Encrypter : StreamEncrypter
    {
        private byte[] _key;

        private int _x;
        private int _y;

        /// <summary>
        ///     Initializes a new instance of the <see cref="RC4Encrypter" /> class.
        /// </summary>
        public RC4Encrypter(string baseKey, string nonce)
        {
            this.Init(baseKey, nonce);
        }

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        public void Init(string baseKey, string nonce)
        {
            string key = baseKey + nonce;

            this._key = new byte[256];
            this._x = 0;
            this._y = 0;

            for (int i = 0; i < 256; i++)
            {
                this._key[i] = (byte) i;
            }

            int j = 0;

            for (int i = 0; i < 256; i++)
            {
                j = (j + this._key[i] + key[i % key.Length]) % 256;

                byte tmpSwap = this._key[i];
                this._key[i] = this._key[j];
                this._key[j] = tmpSwap;
            }

            for (int i = 0; i < key.Length; i++)
            {
                this._x = (this._x + 1) % 256;
                this._y = (this._y + this._key[this._x]) % 256;

                byte tmpSwap = this._key[this._y];
                this._key[this._y] = this._key[this._x];
                this._key[this._x] = tmpSwap;
            }
        }

        /// <summary>
        ///     Decrypts this instance.
        /// </summary>
        public override byte[] Decrypt(byte[] stream)
        {
            byte[] decrypted = new byte[stream.Length];

            for (int i = 0; i < decrypted.Length; i++)
            {
                this._x = (this._x + 1) % 256;
                this._y = (this._y + this._key[this._x]) % 256;

                byte tmpSwap = this._key[this._y];
                this._key[this._y] = this._key[this._x];
                this._key[this._x] = tmpSwap;

                decrypted[i] = (byte) (stream[i] ^ this._key[(this._key[this._x] + this._key[this._y]) % 256]);
            }

            return decrypted;
        }

        /// <summary>
        ///     Encrypts this instance.
        /// </summary>
        public override byte[] Encrypt(byte[] stream)
        {
            byte[] encrypted = new byte[stream.Length];

            for (int i = 0; i < encrypted.Length; i++)
            {
                this._x = (this._x + 1) % 256;
                this._y = (this._y + this._key[this._x]) % 256;

                byte tmpSwap = this._key[this._y];
                this._key[this._y] = this._key[this._x];
                this._key[this._x] = tmpSwap;

                encrypted[i] = (byte) (stream[i] ^ this._key[(this._key[this._x] + this._key[this._y]) % 256]);
            }

            return encrypted;
        }
    }
}