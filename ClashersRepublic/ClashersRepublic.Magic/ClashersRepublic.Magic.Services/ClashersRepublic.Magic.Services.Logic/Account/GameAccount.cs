namespace ClashersRepublic.Magic.Services.Logic.Account
{
    using System.Collections.Generic;

    using ClashersRepublic.Magic.Titan.DataStream;
    using ClashersRepublic.Magic.Titan.Debug;
    using ClashersRepublic.Magic.Titan.Math;
    using ClashersRepublic.Magic.Titan.Util;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    public class GameAccount
    {
        [BsonId]
        public ObjectId _id;

        public int HighId;
        public int LowId;

        public string PassToken;

        public Ban CurrentBan;
        public Session CurrentSession;

        public List<Ban> OldBans;
        public List<Session> OldSessions;

        public int PlayTimeSeconds;
        public int SessionCount;

        /// <summary>
        ///     Initializes a new instance of the <see cref="GameAccount"/> class.
        /// </summary>
        public GameAccount()
        {
            this.OldBans = new List<Ban>(16);
            this.OldSessions = new List<Session>(50);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GameAccount"/> class.
        /// </summary>
        public GameAccount(int highId, int lowId, string passToken) : this()
        {
            this.HighId = highId;
            this.LowId = lowId;
            this.PassToken = passToken;
        }

        /// <summary>
        ///     Starts a permanent ban.
        /// </summary>
        public void StartBan(int totalSecs, string reason)
        {
            if (this.CurrentBan != null)
            {
                if (this.CurrentBan.EndBanTime == -1)
                {
                    Debugger.Warning("GameAccount::startPermanentBan a permanent ban is already in progress");
                    return;
                }

                this.OldBans.Add(this.CurrentBan);
                this.CurrentBan = null;
            }

            int timestamp = LogicTimeUtil.GetTimestamp();
            
            this.CurrentBan = new Ban(timestamp, LogicMath.Max(totalSecs + timestamp, 1), reason);
        }

        /// <summary>
        ///     Starts a permanent ban.
        /// </summary>
        public void StartPermanentBan(string reason)
        {
            if (this.CurrentBan != null)
            {
                if (this.CurrentBan.EndBanTime == -1)
                {
                    Debugger.Warning("GameAccount::startPermanentBan a permanent ban is already in progress");
                    return;
                }

                this.OldBans.Add(this.CurrentBan);
                this.CurrentBan = null;
            }

            this.CurrentBan = new Ban(LogicTimeUtil.GetTimestamp(), -1, reason);
        }

        /// <summary>
        ///     Stops the ban in progress.
        /// </summary>
        public void StopBan()
        {
            if (this.CurrentBan != null)
            {
                this.OldBans.Add(this.CurrentBan);
                this.CurrentBan = null;
            }
            else
            {
                Debugger.Warning("GameAccount::stopBan no ban is in progress on the account " + this.HighId + "-" + this.LowId);
            }
        }

        /// <summary>
        ///     Stops the ban in progress.
        /// </summary>
        public void StopBanWithoutTrace()
        {
            this.CurrentBan = null;
        }

        /// <summary>
        ///     Starts a session.
        /// </summary>
        public void StartSession(string proxySessionId)
        {
            if (this.CurrentSession != null)
            {
                this.StopSession();
            }

            this.SessionCount += 1;
            this.CurrentSession = new Session(proxySessionId);
        }

        /// <summary>
        ///     Stops the session in progress.
        /// </summary>
        public void StopSession()
        {
            if (this.CurrentSession != null)
            {
                if (this.OldSessions.Count >= 50)
                {
                    this.OldSessions.RemoveAt(0);
                }

                this.CurrentSession.Stop();
                this.PlayTimeSeconds += this.CurrentSession.GetTotalSessionTime();
                this.OldSessions.Add(this.CurrentSession);
                this.CurrentSession = null;
            }
            else
            {
                Debugger.Warning("GameAccount::stopSession no session in progress");
            }
        }

        /// <summary>
        ///     Updates this account.
        /// </summary>
        public void Tick()
        {
            if (this.CurrentBan != null)
            {
                if (this.CurrentBan.EndBanTime != -1)
                {
                    if (LogicTimeUtil.GetTimestamp() > this.CurrentBan.EndBanTime)
                    {
                        this.OldBans.Add(this.CurrentBan);
                        this.CurrentBan = null;
                    }
                }
            }
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public void Decode(ByteStream stream)
        {
            this.OldSessions.Clear();
            this.OldBans.Clear();

            this.HighId = stream.ReadVInt();
            this.LowId = stream.ReadVInt();
            this.PassToken = stream.ReadString(40);

            if (stream.ReadBoolean())
            {
                this.CurrentSession = new Session();
                this.CurrentSession.Decode(stream);
            }

            if (stream.ReadBoolean())
            {
                this.CurrentBan = new Ban();
                this.CurrentBan.Decode(stream);
            }
            
            for (int arraySize = stream.ReadVInt(); arraySize > 0; arraySize--)
            {
                Session session = new Session();
                session.Decode(stream);
                this.OldSessions.Add(session);
            }

            for (int arraySize = stream.ReadVInt(); arraySize > 0; arraySize--)
            {
                Ban ban = new Ban();
                ban.Decode(stream);
                this.OldBans.Add(ban);
            }
        }
        
        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public void Encode(ChecksumEncoder encoder)
        {
            encoder.WriteVInt(this.HighId);
            encoder.WriteVInt(this.LowId);
            encoder.WriteString(this.PassToken);

            if (this.CurrentSession != null)
            {
                encoder.WriteBoolean(true);
                this.CurrentSession.Encode(encoder);
            }
            else
            {
                encoder.WriteBoolean(false);
            }

            if (this.CurrentBan != null)
            {
                encoder.WriteBoolean(true);
                this.CurrentBan.Encode(encoder);
            }
            else
            {
                encoder.WriteBoolean(false);
            }

            encoder.WriteVInt(this.OldSessions.Count);

            for (int i = 0; i < this.OldSessions.Count; i++)
            {
                this.OldSessions[i].Encode(encoder);
            }

            encoder.WriteVInt(this.OldBans.Count);

            for (int i = 0; i < this.OldBans.Count; i++)
            {
                this.OldBans[i].Encode(encoder);
            }
        }
    }

    public class Ban
    {
        public int StartBanTime;
        public int EndBanTime;

        public string Reason;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Ban"/> class.
        /// </summary>
        public Ban()
        {
            // Ban.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Ban"/> class.
        /// </summary>
        public Ban(int startBanTime, int endBanTime, string reason)
        {
            this.StartBanTime = startBanTime;
            this.EndBanTime = endBanTime;
            this.Reason = reason;
        }

        /// <summary>
        ///     Gets the remaining ban time.
        /// </summary>
        public int GetRemainingBanTime()
        {
            if (this.EndBanTime != -1)
            {
                return this.EndBanTime - this.StartBanTime;
            }

            return -1;
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public void Decode(ByteStream stream)
        {
            this.StartBanTime = stream.ReadInt();
            this.EndBanTime = stream.ReadInt();
            this.Reason = stream.ReadString(900000);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public void Encode(ChecksumEncoder encoder)
        {
            encoder.WriteInt(this.StartBanTime);
            encoder.WriteInt(this.EndBanTime);
            encoder.WriteString(this.Reason);
        }
    }

    public class Session
    {
        public int State;
        public int StartTime;
        public int EndTime;
        
        public string ProxySessionId;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Session"/> class.
        /// </summary>
        public Session()
        {
            // Session
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Session"/> class.
        /// </summary>
        public Session(string proxySessionId)
        {
            this.ProxySessionId = proxySessionId;
            this.StartTime = LogicTimeUtil.GetTimestamp();
            this.State = 1;
        }

        /// <summary>
        ///     Stops the session.
        /// </summary>
        public void Stop()
        {
            this.State = 5;
            this.EndTime = LogicTimeUtil.GetTimestamp();
        }

        /// <summary>
        ///     Gets a value indicating whether this session is started.
        /// </summary>
        public bool IsStarted()
        {
            return this.State == 1;
        }

        /// <summary>
        ///     Gets a value indicating whether this session is started.
        /// </summary>
        public bool IsStopped()
        {
            return this.State > 1;
        }

        /// <summary>
        ///     Gets the total session time.
        /// </summary>
        public int GetTotalSessionTime()
        {
            return LogicMath.Max(this.EndTime - this.StartTime, 1);
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public void Decode(ByteStream stream)
        {
            this.StartTime = stream.ReadInt();
            this.EndTime = stream.ReadInt();
            this.State = stream.ReadVInt();
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public void Encode(ChecksumEncoder encoder)
        {
            encoder.WriteInt(this.StartTime);
            encoder.WriteInt(this.EndTime);
            encoder.WriteVInt(this.State);
        }
    }
}