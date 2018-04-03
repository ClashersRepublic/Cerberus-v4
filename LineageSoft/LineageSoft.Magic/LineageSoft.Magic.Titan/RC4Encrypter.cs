namespace LineageSoft.Magic.Titan
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
            this.InitState(baseKey, nonce);
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();

            this._key = null;
            this._x = 0;
            this._y = 0;
        }

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        public void InitState(string baseKey, string nonce)
        {
            string key = baseKey + nonce;

            this._key = new byte[256];
            this._x = 0;
            this._y = 0;

            for (int i = 0; i < 256; i++)
            {
                this._key[i] = (byte) i;
            }

            for (int i = 0, j = 0; i < 256; i++)
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
        public override int Decrypt(byte[] input, byte[] output, int length)
        {
            return this.Encrypt(input, output, length);
        }

        /// <summary>
        ///     Encrypts this instance.
        /// </summary>
        public override int Encrypt(byte[] input, byte[] output, int length)
        {
            if (length >= 1)
            {
                for (int i = 0; i < length; i++)
                {
                    this._x = (this._x + 1) % 256;
                    this._y = (this._y + this._key[this._x]) % 256;

                    byte tmp = this._key[this._y];
                    this._key[this._y] = this._key[this._x];
                    this._key[this._x] = tmp;

                    output[i] = (byte) (input[i] ^ this._key[(this._key[this._x] + this._key[this._y]) % 256]);
                }
            }

            return 0;
        }
    }
}