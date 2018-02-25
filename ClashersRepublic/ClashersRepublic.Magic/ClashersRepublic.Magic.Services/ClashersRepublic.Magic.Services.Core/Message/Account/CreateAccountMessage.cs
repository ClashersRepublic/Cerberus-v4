﻿namespace ClashersRepublic.Magic.Services.Core.Message.Account
{
    public class CreateAccountMessage : NetMessage
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="CreateAccountMessage"/> instance.
        /// </summary>
        public CreateAccountMessage() : base()
        {
            // CreateAccountMessage.
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
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
        }

        /// <summary>
        ///     Gets the message type of this instance.
        /// </summary>
        public override int GetMessageType()
        {
            return 10103;
        }
    }
}