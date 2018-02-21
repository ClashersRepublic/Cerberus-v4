﻿namespace ClashersRepublic.Magic.Logic.Command.Server
{
    using ClashersRepublic.Magic.Titan.DataStream;

    public class LogicServerCommand : LogicCommand
    {
        private int _id;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicServerCommand" /> class.
        /// </summary>
        public LogicServerCommand()
        {
            this._id = -1;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public virtual void Destruct()
        {
            // Destruct.
        }

        /// <summary>
        ///     Gets the server command id.
        /// </summary>
        public int GetId()
        {
            return this._id;
        }

        /// <summary>
        ///     Sets the server command id.
        /// </summary>
        public void SetId(int id)
        {
            this._id = id;
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode(ByteStream stream)
        {
            this._id = stream.ReadInt();
            base.Decode(stream);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode(ChecksumEncoder encoder)
        {
            encoder.WriteInt(this._id);
            base.Encode(encoder);
        }
    }
}