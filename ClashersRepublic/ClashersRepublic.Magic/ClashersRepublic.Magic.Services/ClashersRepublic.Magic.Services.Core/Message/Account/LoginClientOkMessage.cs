namespace ClashersRepublic.Magic.Services.Core.Message.Account
{
    using ClashersRepublic.Magic.Titan.Math;

    public class LoginClientOkMessage : NetMessage
    {
        private LogicLong _accountId;
        private LogicLong _homeId;
        private string _passToken;
        private string _accountCreatedDate;
        private string _facebookId;
        private string _gamecenterId;
        private string _googleServiceId;
        private int _sessionCount;
        private int _playTimeSeconds;
        private int _daysSinceStartedPlaying;

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();

            this._accountId = null;
            this._homeId = null;
            this._passToken = null;
            this._accountCreatedDate = null;
            this._facebookId = null;
            this._gamecenterId = null;
            this._googleServiceId = null;
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();

            this.Stream.WriteLong(this._accountId);
            this.Stream.WriteLong(this._homeId);
            this.Stream.WriteString(this._passToken);
            this.Stream.WriteString(this._accountCreatedDate);
            this.Stream.WriteString(this._facebookId);
            this.Stream.WriteString(this._gamecenterId);
            this.Stream.WriteString(this._googleServiceId);
            this.Stream.WriteVInt(this._sessionCount);
            this.Stream.WriteVInt(this._playTimeSeconds);
            this.Stream.WriteVInt(this._daysSinceStartedPlaying);
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();

            this._accountId = this.Stream.ReadLong();
            this._homeId = this.Stream.ReadLong();
            this._passToken = this.Stream.ReadString(900000);
            this._accountCreatedDate = this.Stream.ReadString(900000);
            this._facebookId = this.Stream.ReadString(900000);
            this._gamecenterId = this.Stream.ReadString(900000);
            this._googleServiceId = this.Stream.ReadString(900000);
            this._sessionCount = this.Stream.ReadVInt();
            this._playTimeSeconds = this.Stream.ReadVInt();
            this._daysSinceStartedPlaying = this.Stream.ReadVInt();
        }

        /// <summary>
        ///     Gets the message type of this instance.
        /// </summary>
        public override int GetMessageType()
        {
            return 20104;
        }

        /// <summary>
        ///     Removes the account id.
        /// </summary>
        public LogicLong RemoveAccountId()
        {
            LogicLong tmp = this._accountId;
            this._accountId = null;
            return tmp;
        }

        /// <summary>
        ///     Sets the account id.
        /// </summary>
        public void SetAccountId(LogicLong value)
        {
            this._accountId = value;
        }

        /// <summary>
        ///     Removes the home id.
        /// </summary>
        public LogicLong RemoveHomeId()
        {
            LogicLong tmp = this._homeId;
            this._homeId = null;
            return tmp;
        }

        /// <summary>
        ///     Sets the home id.
        /// </summary>
        public void SetHomeId(LogicLong value)
        {
            this._homeId = value;
        }

        /// <summary>
        ///     Removes the pass token.
        /// </summary>
        public string RemovePassToken()
        {
            string tmp = this._passToken;
            this._passToken = null;
            return tmp;
        }

        /// <summary>
        ///     Sets the pass token.
        /// </summary>
        public void SetPassToken(string value)
        {
            this._passToken = value;
        }

        /// <summary>
        ///     Removes the facebook id.
        /// </summary>
        public string RemoveFacebookId()
        {
            string tmp = this._facebookId;
            this._facebookId = null;
            return tmp;
        }

        /// <summary>
        ///     Sets the facebook id.
        /// </summary>
        public void SetFacebookId(string value)
        {
            this._facebookId = value;
        }

        /// <summary>
        ///     Removes the gamecenter id.
        /// </summary>
        public string RemoveGamecenterId()
        {
            string tmp = this._gamecenterId;
            this._gamecenterId = null;
            return tmp;
        }

        /// <summary>
        ///     Sets the account created date.
        /// </summary>
        public void SetGamecenterId(string value)
        {
            this._gamecenterId = value;
        }

        /// <summary>
        ///     Removes the google service id.
        /// </summary>
        public string RemoveGoogleServiceId()
        {
            string tmp = this._googleServiceId;
            this._googleServiceId = null;
            return tmp;
        }

        /// <summary>
        ///     Sets the google service id.
        /// </summary>
        public void SetGoogleServiceId(string value)
        {
            this._googleServiceId = value;
        }

        /// <summary>
        ///     Removes the account created date.
        /// </summary>
        public string RemoveAccountCreatedDate()
        {
            string tmp = this._accountCreatedDate;
            this._accountCreatedDate = null;
            return tmp;
        }

        /// <summary>
        ///     Sets the account created date.
        /// </summary>
        public void SetAccountCreatedDate(string value)
        {
            this._accountCreatedDate = value;
        }

        /// <summary>
        ///     Gets session count
        /// </summary>
        public int GetSessionCount()
        {
            return this._sessionCount;
        }

        /// <summary>
        ///     Sets session count
        /// </summary>
        public void SetSessionCount(int value)
        {
            this._sessionCount = value;
        }

        /// <summary>
        ///     Gets the play time seconds.
        /// </summary>
        public int GetPlayTimeSeconds()
        {
            return this._playTimeSeconds;
        }

        /// <summary>
        ///     Sets the play time seconds.
        /// </summary>
        public void SetPlayTimeSeconds(int value)
        {
            this._playTimeSeconds = value;
        }

        /// <summary>
        ///     Gets the days since started playing.
        /// </summary>
        public int GetDaysSinceStartedPlaying()
        {
            return this._daysSinceStartedPlaying;
        }

        /// <summary>
        ///     Sets the days since started playing.
        /// </summary>
        public void SetDaysSinceStartedPlaying(int value)
        {
            this._daysSinceStartedPlaying = value;
        }
    }
}