﻿namespace RivieraStudio.Magic.Titan.Message
{
    using RivieraStudio.Magic.Titan.DataStream;

    public class PiranhaMessage
    {
        protected ByteStream Stream;
        protected int Version;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PiranhaMessage" /> class.
        /// </summary>
        public PiranhaMessage(short messageVersion)
        {
            this.Stream = new ByteStream(10);
            this.Version = messageVersion;
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public virtual void Decode()
        {
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public virtual void Encode()
        {
        }

        /// <summary>
        ///     Gets the message type of this instance.
        /// </summary>
        public virtual short GetMessageType()
        {
            return 0;
        }

        /// <summary>
        ///     Destructs this message.
        /// </summary>
        public virtual void Destruct()
        {
            this.Stream.Destruct();
        }

        /// <summary>
        ///     Gets the service node type of this instance.
        /// </summary>
        public virtual int GetServiceNodeType()
        {
            return -1;
        }

        /// <summary>
        ///     Gets the message version of this instance.
        /// </summary>
        public int GetMessageVersion()
        {
            return this.Version;
        }

        /// <summary>
        ///     Sets the version of message.
        /// </summary>
        public void SetMessageVersion(int version)
        {
            this.Version = version;
        }

        /// <summary>
        ///     Gets a value indicating whether this message is a server to client message.
        /// </summary>
        public bool IsServerToClientMessage()
        {
            return this.GetMessageType() >= 20000;
        }

        /// <summary>
        ///     Gets the message bytes.
        /// </summary>
        public byte[] GetMessageBytes()
        {
            return this.Stream.GetByteArray();
        }

        /// <summary>
        ///     Gets the length of encoding.
        /// </summary>
        public int GetEncodingLength()
        {
            return this.Stream.GetLength();
        }

        /// <summary>
        ///     Gets the byte stream instance.
        /// </summary>
        public ByteStream GetByteStream()
        {
            return this.Stream;
        }
    }
}