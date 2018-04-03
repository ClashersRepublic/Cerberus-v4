namespace LineageSoft.Magic.Logic.Message.Avatar.Stream
{
    using LineageSoft.Magic.Titan.Debug;
    using LineageSoft.Magic.Titan.Message;
    using LineageSoft.Magic.Titan.Util;

    public class AvatarStreamMessage : PiranhaMessage
    {
        private LogicArrayList<AvatarStreamEntry> _entries;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AvatarStreamMessage" /> class.
        /// </summary>
        public AvatarStreamMessage() : this(0)
        {
            // AvatarStreamMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AvatarStreamMessage" /> class.
        /// </summary>
        public AvatarStreamMessage(short messageVersion) : base(messageVersion)
        {
            // AvatarStreamMessage.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();

            int cnt = this.Stream.ReadInt();

            if (cnt != -1)
            {
                this._entries = new LogicArrayList<AvatarStreamEntry>(cnt);

                for (int i = 0; i < cnt; i++)
                {
                    AvatarStreamEntry entry = AvatarStreamEntryFactory.CreateStreamEntryByType(this.Stream.ReadInt());

                    if (entry == null)
                    {
                        Debugger.Warning("Corrupted AvatarStreamMessage");
                        break;
                    }

                    entry.Decode(this.Stream);
                }
            }
            else
            {
                this._entries = null;
            }
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();

            if (this._entries != null)
            {
                this.Stream.WriteInt(this._entries.Count);

                for (int i = 0; i < this._entries.Count; i++)
                {
                    this.Stream.WriteInt(this._entries[i].GetAvatarStreamEntryType());
                    this._entries[i].Encode(this.Stream);
                }
            }
            else
            {
                this.Stream.WriteInt(-1);
            }
        }

        /// <summary>
        ///     Gets the message type of this message.
        /// </summary>
        public override short GetMessageType()
        {
            return 24411;
        }

        /// <summary>
        ///     Gets the service node type of this message.
        /// </summary>
        public override int GetServiceNodeType()
        {
            return 11;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();

            if (this._entries != null)
            {
                if (this._entries.Count != 0)
                {
                    do
                    {
                        this._entries[0].Destruct();
                        this._entries.Remove(0);
                    } while (this._entries.Count != 0);
                }

                this._entries = null;
            }
        }

        /// <summary>
        ///     Removes the stream entry array.
        /// </summary>
        /// <returns></returns>
        public LogicArrayList<AvatarStreamEntry> RemoveStreamEntries()
        {
            LogicArrayList<AvatarStreamEntry> tmp = this._entries;
            this._entries = null;
            return tmp;
        }

        /// <summary>
        ///     Sets the stream entry array.
        /// </summary>
        public void SetStreamEntries(LogicArrayList<AvatarStreamEntry> entry)
        {
            this._entries = entry;
        }
    }
}