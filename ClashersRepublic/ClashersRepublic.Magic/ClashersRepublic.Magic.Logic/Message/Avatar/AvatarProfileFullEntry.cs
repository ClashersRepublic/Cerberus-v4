namespace ClashersRepublic.Magic.Logic.Message.Avatar
{
    using ClashersRepublic.Magic.Logic.Avatar;
    using ClashersRepublic.Magic.Logic.Util;
    using ClashersRepublic.Magic.Titan.DataStream;

    public class AvatarProfileFullEntry
    {
        private LogicClientAvatar _clientAvatar;

        private byte[] _compressedHomeJSON;

        private int _donations;
        private int _donationsReceived;
        private int _remainingSecsForWar;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AvatarProfileFullEntry"/> class.
        /// </summary>
        public AvatarProfileFullEntry()
        {
            // AvatarProfileFullEntry.
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public void Destruct()
        {
            this._clientAvatar = null;
            this._compressedHomeJSON = null;
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public void Encode(ChecksumEncoder encoder)
        {
            this._clientAvatar.Encode(encoder);

            encoder.WriteBytes(this._compressedHomeJSON, this._compressedHomeJSON.Length);
            encoder.WriteInt(this._donations);
            encoder.WriteInt(this._donationsReceived);
            encoder.WriteInt(this._remainingSecsForWar);
            encoder.WriteBoolean(true);
            encoder.WriteInt(0);
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public void Decode(ByteStream stream)
        {
            this._clientAvatar = new LogicClientAvatar();
            this._clientAvatar.Decode(stream);

            this._compressedHomeJSON = stream.ReadBytes(stream.ReadBytesLength(), 900000);
            this._donations = stream.ReadInt();
            this._donationsReceived = stream.ReadInt();
            this._remainingSecsForWar = stream.ReadInt();

            stream.ReadBoolean();
            stream.ReadInt();
        }

        /// <summary>
        ///     Gets the <see cref="LogicClientAvatar"/> instance.
        /// </summary>
        public LogicClientAvatar GetLogicClientAvatar()
        {
            return this._clientAvatar;
        }

        /// <summary>
        ///     Sets the <see cref="LogicClientAvatar"/> instance.
        /// </summary>
        public void SetLogicClientAvatar(LogicClientAvatar avatar)
        {
            this._clientAvatar = avatar;
        }

        /// <summary>
        ///     Gets the home json.
        /// </summary>
        public byte[] GetCompressdHomeJSON()
        {
            return this._compressedHomeJSON;
        }

        /// <summary>
        ///     Sets the home json.
        /// </summary>
        public void SetCompressedHomeJSON(byte[] compressibleString)
        {
            this._compressedHomeJSON = compressibleString;
        }

        /// <summary>
        ///     Gets the total donations.
        /// </summary>
        public int GetDonations()
        {
            return this._donations;
        }

        /// <summary>
        ///     Sets the total donations.
        /// </summary>
        public void SetDonations(int value)
        {
            this._donations = value;
        }

        /// <summary>
        ///     Gets the total donations received.
        /// </summary>
        public int GetDonationsReceived()
        {
            return this._donationsReceived;
        }

        /// <summary>
        ///     Sets the total donations received.
        /// </summary>
        public void SetDonationsReceived(int value)
        {
            this._donationsReceived = value;
        }

        /// <summary>
        ///     Gets the remaining secs for war.
        /// </summary>
        public int GetRemainingSecondsForWar()
        {
            return this._remainingSecsForWar;
        }

        /// <summary>
        ///     Sets the remaining secs for war.
        /// </summary>
        public void SetRemainingSecondsForWar(int value)
        {
            this._remainingSecsForWar = value;
        }
    }
}