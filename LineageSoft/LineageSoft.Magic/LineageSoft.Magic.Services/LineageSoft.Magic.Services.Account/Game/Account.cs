namespace LineageSoft.Magic.Services.Account.Game
{
    using LineageSoft.Magic.Logic.Helper;

    using LineageSoft.Magic.Services.Account.Network.Session;
    using LineageSoft.Magic.Services.Core;

    using LineageSoft.Magic.Titan.Json;
    using LineageSoft.Magic.Titan.Math;
    using LineageSoft.Magic.Titan.Util;
    
    internal class Account
    {
        /// <summary>
        ///     Gets the <see cref="Account"/> id.
        /// </summary>
        internal LogicLong Id { get; private set; }
        
        /// <summary>
        ///     Gets the password token.
        /// </summary>
        internal string PassToken { get; private set; }
        
        /// <summary>
        ///     Gets the list of last sessions.
        /// </summary>
        internal LogicArrayList<AccountSession> LastSessions { get; }

        /// <summary>
        ///     Gets the current ban.
        /// </summary>
        internal AccountBan CurrentBan { get; private set; }

        /// <summary>
        ///     Gets the <see cref="NetAccountSession"/> in progress.
        /// </summary>
        internal NetAccountSession Session { get; private set; }

        /// <summary>
        ///     Gets the total ban count.
        /// </summary>
        internal int TotalBan { get; private set; }

        /// <summary>
        ///     Gets the play time in seconds
        /// </summary>
        internal int PlayTimeSecs { get; private set; }

        /// <summary>
        ///     Gets the total sessions.
        /// </summary>
        internal int TotalSessions { get; private set; }

        /// <summary>
        ///     Gets the account created date.
        /// </summary>
        internal string AccountCreatedDate { get; private set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Account"/> class.
        /// </summary>
        public Account()
        {
            this.Id = new LogicLong();
            this.LastSessions = new LogicArrayList<AccountSession>(20);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Account"/> class.
        /// </summary>
        public Account(LogicLong accountId, string passToken) : this()
        {
            this.Id = accountId;
            this.PassToken = passToken;

            this.AccountCreatedDate = LogicTimeUtil.GetTimestampMS();
        }
        
        /// <summary>
        ///     Called when a new session is started.
        /// </summary>
        internal void SessionStarted(int timestamp, string ip, string deviceModel)
        {
            if (this.LastSessions.Count >= 20)
            {
                this.LastSessions.Remove(0);
            }

            this.TotalSessions += 1;
            this.LastSessions.Add(new AccountSession(timestamp, ip, deviceModel));
        }

        /// <summary>
        ///     Called when the session is ended.
        /// </summary>
        internal void EndSession()
        {
            if (this.LastSessions.Count != 0)
            {
                AccountSession session = this.LastSessions[this.LastSessions.Count - 1];

                if (session.EndTime == 0)
                {
                    session.SetEndTime(LogicTimeUtil.GetTimestamp());

                    if (session.EndTime > session.StartTime)
                    {
                        this.PlayTimeSecs += session.EndTime - session.StartTime;
                    }
                }
            }
        }

        /// <summary>
        ///     Creates a new ban for this <see cref="Account"/>.
        /// </summary>
        internal bool CreateBan(string reason, int endTime)
        {
            if (this.CurrentBan != null)
            {
                Logging.Warning("Account::createBan ban already in progress");
            }
            else
            {
                this.CurrentBan = new AccountBan(reason, endTime);
                this.TotalBan += 1;

                return true;
            }

            return false;
        }

        /// <summary>
        ///     Gets if this <see cref="Account"/> instance is banned.
        /// </summary>
        internal bool IsBanned()
        {
            if (this.CurrentBan != null)
            {
                if (this.CurrentBan.EndBanTime != -1)
                {
                    if (LogicTimeUtil.GetTimestamp() > this.CurrentBan.EndBanTime)
                    {
                        this.CurrentBan = null;
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        /// <summary>
        ///     Gets the remaining ban time.
        /// </summary>
        internal int GetRemainingBanTime()
        {
            if (this.CurrentBan != null)
            {
                if (this.CurrentBan.EndBanTime != -1)
                {
                    return LogicMath.Max(LogicTimeUtil.GetTimestamp() - this.CurrentBan.EndBanTime, 0);
                }

                return -1;
            }

            return 0;
        }

        /// <summary>
        ///     Revokes the current ban.
        /// </summary>
        internal bool RevokeBan()
        {
            bool success = this.CurrentBan != null;

            if (success)
            {
                this.CurrentBan = null;
            }

            return success;
        }

        /// <summary>
        ///     Sets the <see cref="NetAccountSession"/> instance.
        /// </summary>
        internal void SetSession(NetAccountSession session)
        {
            this.Session = session;
        }

        /// <summary>
        ///     Saves this instance to json.
        /// </summary>
        internal LogicJSONObject Save()
        {
            LogicJSONObject jsonObject = new LogicJSONObject();

            jsonObject.Put("id_hi", new LogicJSONNumber(this.Id.GetHigherInt()));
            jsonObject.Put("id_lo", new LogicJSONNumber(this.Id.GetLowerInt()));
            jsonObject.Put("pass_t", new LogicJSONString(this.PassToken));
            jsonObject.Put("pt_secs", new LogicJSONNumber(this.PlayTimeSecs));
            jsonObject.Put("acc_cr", new LogicJSONString(this.AccountCreatedDate));
            jsonObject.Put("t_ban", new LogicJSONNumber(this.TotalBan));
            
            LogicJSONArray sessionArray = new LogicJSONArray();

            for (int i = 0; i < this.LastSessions.Count; i++)
            {
                LogicJSONObject obj = new LogicJSONObject();
                this.LastSessions[i].Save(obj);
                sessionArray.Add(obj);
            }

            jsonObject.Put("sessions", sessionArray);

            if (this.CurrentBan != null)
            {
                LogicJSONObject banObject = new LogicJSONObject();
                this.CurrentBan.Save(banObject);
                jsonObject.Put("ban", banObject);
            }

            return jsonObject;
        }

        /// <summary>
        ///     Loads this instance from json.
        /// </summary>
        internal void Load(string json)
        {
            LogicJSONObject jsonObject = (LogicJSONObject) LogicJSONParser.Parse(json);

            this.Id = new LogicLong(LogicJSONHelper.GetJSONNumber(jsonObject, "id_hi"), LogicJSONHelper.GetJSONNumber(jsonObject, "id_lo"));
            this.PassToken = LogicJSONHelper.GetJSONString(jsonObject, "pass_t");
            this.PlayTimeSecs = LogicJSONHelper.GetJSONNumber(jsonObject, "pt_secs");
            this.AccountCreatedDate = LogicJSONHelper.GetJSONString(jsonObject, "acc_cr");
            this.TotalBan = LogicJSONHelper.GetJSONNumber(jsonObject, "t_ban");

            LogicJSONArray sessionArray = jsonObject.GetJSONArray("sessions");

            if (sessionArray != null)
            {
                int size = LogicMath.Min(sessionArray.Size(), 20);

                for (int i = 0; i < size; i++)
                {
                    LogicJSONObject obj = sessionArray.GetJSONObject(i);
                    AccountSession session = new AccountSession();
                    session.Load(obj);
                    this.LastSessions.Add(session);
                }
            }

            LogicJSONObject banObject = jsonObject.GetJSONObject("ban");

            if (banObject != null)
            {
                this.CurrentBan = new AccountBan();
                this.CurrentBan.Load(banObject);
            }
        }

        internal class AccountBan
        {
            /// <summary>
            ///     Gets the ban reason.
            /// </summary>
            internal string BanReason { get; private set; }

            /// <summary>
            ///     Gets the timestamp of the end ban time.
            /// </summary>
            internal int EndBanTime { get; private set; }

            /// <summary>
            ///     Initializes a new instance of the <see cref="AccountBan"/> class.
            /// </summary>
            internal AccountBan()
            {
                // AccountBan.
            }

            /// <summary>
            ///     Initializes a new instance of the <see cref="AccountBan"/> class.
            /// </summary>
            internal AccountBan(string banReason, int endBanTime)
            {
                this.BanReason = banReason;
                this.EndBanTime = endBanTime;
            }

            /// <summary>
            ///     Gets if this ban is permanent.
            /// </summary>
            internal bool IsPermanentBan()
            {
                return this.EndBanTime == -1;
            }

            /// <summary>
            ///     Saves this instance to json.
            /// </summary>
            internal void Save(LogicJSONObject jsonObject)
            {
                jsonObject.Put("rs", new LogicJSONString(this.BanReason));
                jsonObject.Put("et", new LogicJSONNumber(this.EndBanTime));
            }

            /// <summary>
            ///     Loads this instance from json.
            /// </summary>
            internal void Load(LogicJSONObject jsonObject)
            {
                this.BanReason = LogicJSONHelper.GetJSONString(jsonObject, "rs");
                this.EndBanTime = LogicJSONHelper.GetJSONNumber(jsonObject, "et");
            }
        }

        internal class AccountSession
        {
            /// <summary>
            ///     Gets the start time.
            /// </summary>
            internal int StartTime { get; private set; }

            /// <summary>
            ///     Gets the end time.
            /// </summary>
            internal int EndTime { get; private set; }

            /// <summary>
            ///     Gets the end point.
            /// </summary>
            internal string EndPoint { get; private set; }

            /// <summary>
            ///     Gets the device model.
            /// </summary>
            internal string DeviceModel { get; private set; }

            /// <summary>
            ///     Initializes a new instance of the <see cref="AccountSession"/> class.
            /// </summary>
            internal AccountSession()
            {
                // AccountSession.
            }

            /// <summary>
            ///     Initializes a new instance of the <see cref="AccountSession"/> class.
            /// </summary>
            internal AccountSession(int startTime, string ipAddress, string deviceModel)
            {
                this.StartTime = startTime;
                this.DeviceModel = deviceModel;
                this.EndPoint = ipAddress;
            }

            /// <summary>
            ///     Sets the end time.
            /// </summary>
            internal void SetEndTime(int endTime)
            {
                this.EndTime = endTime;
            }

            /// <summary>
            ///     Saves this instance to json.
            /// </summary>
            internal void Save(LogicJSONObject jsonObject)
            {
                jsonObject.Put("st", new LogicJSONNumber(this.StartTime));
                jsonObject.Put("et", new LogicJSONNumber(this.EndTime));
                jsonObject.Put("ip", new LogicJSONString(this.EndPoint));
                jsonObject.Put("dm", new LogicJSONString(this.DeviceModel));
            }

            /// <summary>
            ///     Loads this instance from json.
            /// </summary>
            internal void Load(LogicJSONObject jsonObject)
            {
                this.StartTime = LogicJSONHelper.GetJSONNumber(jsonObject, "st");
                this.EndTime = LogicJSONHelper.GetJSONNumber(jsonObject, "et");
                this.EndPoint = LogicJSONHelper.GetJSONString(jsonObject, "ip");
                this.DeviceModel = LogicJSONHelper.GetJSONString(jsonObject, "dm");
            }
        }
    }
}