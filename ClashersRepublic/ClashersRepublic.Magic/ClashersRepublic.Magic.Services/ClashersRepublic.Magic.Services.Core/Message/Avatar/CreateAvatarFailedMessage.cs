﻿namespace ClashersRepublic.Magic.Services.Core.Message.Avatar
{
    public class CreateAvatarFailedMessage : NetMessage
    {
        private int _errorCode;

        /// <summary>
        ///     Initializes a new instance of the <see cref="CreateAvatarFailedMessage"/> instance.
        /// </summary>
        public CreateAvatarFailedMessage() : base()
        {
            // CreateAvatarFailedMessage.
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
            this.Stream.WriteVInt(this._errorCode);
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
            this._errorCode = this.Stream.ReadVInt();
        }

        /// <summary>
        ///     Gets the message type of this instance.
        /// </summary>
        public override int GetMessageType()
        {
            return 20202;
        }

        /// <summary>
        ///     Gets the error code.
        /// </summary>
        public int GetErrorCode()
        {
            return this._errorCode;
        }

        /// <summary>
        ///     Sets the error code.
        /// </summary>
        public void SetErrorCode(int value)
        {
            this._errorCode = value;
        }
    }
}