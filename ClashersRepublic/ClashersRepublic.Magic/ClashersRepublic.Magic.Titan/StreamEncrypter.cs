namespace ClashersRepublic.Magic.Titan
{
    using System;

    public class StreamEncrypter
    {
        /// <summary>
        ///     Decryptes this instance.
        /// </summary>
        public virtual byte[] Decrypt(byte[] stream)
        {
            byte[] decrypted = new byte[stream.Length];
            Array.Copy(stream, decrypted, stream.Length);
            return decrypted;
        }

        /// <summary>
        ///     Encryptes this instance.
        /// </summary>
        public virtual byte[] Encrypt(byte[] stream)
        {
            byte[] encrypted = new byte[stream.Length];
            Array.Copy(stream, encrypted, stream.Length);
            return encrypted;
        }

        /// <summary>
        ///     Gets the size of encryption overhead.
        /// </summary>
        public virtual int GetOverheadEncryption()
        {
            return 0;
        }
    }
}