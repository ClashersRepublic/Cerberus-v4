namespace RivieraStudio.Magic.Titan
{
    using System;

    public class StreamEncrypter
    {
        /// <summary>
        ///     Decryptes this instance.
        /// </summary>
        public virtual int Decrypt(byte[] input, byte[] output, int length)
        {
            byte[] decrypted = new byte[length];
            Array.Copy(input, decrypted, length);
            return 0;
        }

        /// <summary>
        ///     Encryptes this instance.
        /// </summary>
        public virtual int Encrypt(byte[] input, byte[] output, int length)
        {
            byte[] encrypted = new byte[length];
            Array.Copy(input, encrypted, length);
            return 0;
        }

        /// <summary>
        ///     Gets the size of encryption overhead.
        /// </summary>
        public virtual int GetOverheadEncryption()
        {
            return 0;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public virtual void Destruct()
        {
            // Destruct.
        }
    }
}