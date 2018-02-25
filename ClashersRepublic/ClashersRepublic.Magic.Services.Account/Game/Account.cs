namespace ClashersRepublic.Magic.Services.Account.Game
{
    using ClashersRepublic.Magic.Services.Core;
    using ClashersRepublic.Magic.Services.Core.Network.Session;
    using ClashersRepublic.Magic.Titan.Math;
    using ClashersRepublic.Magic.Titan.Util;

    internal class Account
    {
        /// <summary>
        ///     Gets the <see cref="Account"/> id.
        /// </summary>
        internal LogicLong Id { get; }

        /// <summary>
        ///     Gets the password token.
        /// </summary>
        internal string PassToken { get; }

        /// <summary>
        ///     Gets the list of different ip addresses that connected to this <see cref="Account"/>.
        /// </summary>
        internal LogicArrayList<string> IPs { get; }

        /// <summary>
        ///     Gets the list of last sessions.
        /// </summary>
        internal LogicArrayList<int> LastSessions { get; }

        /// <summary>
        ///     Gets the current ban.
        /// </summary>
        internal AccountBan CurrentBan { get; private set; }

        /// <summary>
        ///     Gets the total ban count.
        /// </summary>
        internal int TotalBan { get; private set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Account"/> class.
        /// </summary>
        public Account(LogicLong accountId, string passToken)
        {
            this.Id = accountId;
            this.PassToken = passToken;

            this.IPs = new LogicArrayList<string>(20);
            this.LastSessions = new LogicArrayList<int>(50);
        }

        /// <summary>
        ///     Called when a new session is started.
        /// </summary>
        internal void SessionStarted(int timestamp, string ip)
        {
            if (this.IPs.IndexOf(ip) == -1)
            {
                if (this.IPs.Count >= 20)
                {
                    this.IPs.Remove(0);
                }

                this.IPs.Add(ip);
            }

            if (this.LastSessions.Count >= 50)
            {
                this.LastSessions.Remove(0);
            }

            this.LastSessions.Add(timestamp);
        }

        /// <summary>
        ///     Creates a new ban for this <see cref="Account"/>.
        /// </summary>
        internal bool CreateBan(string reason, int endTime)
        {
            if (this.CurrentBan != null)
            {
                Logging.Warning(this, "Account::createBan ban already in progress");
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
        ///     Revokes the current ban.
        /// </summary>
        internal bool RevokeBan()
        {
            if (this.CurrentBan == null)
            {
                Logging.Warning(this, "Account::createBan ban is NULL");
            }
            else
            {
                this.CurrentBan = null;
                return true;
            }

            return false;
        }

        internal class AccountBan
        {
            /// <summary>
            ///     Gets the ban reason.
            /// </summary>
            internal string BanReason { get; }

            /// <summary>
            ///     Gets the timestamp of the end ban time.
            /// </summary>
            internal int EndBanTime { get; }

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
        }
    }
}