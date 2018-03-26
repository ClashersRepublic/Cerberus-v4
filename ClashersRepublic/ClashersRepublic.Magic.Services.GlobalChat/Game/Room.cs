namespace ClashersRepublic.Magic.Services.GlobalChat.Game
{
    using ClashersRepublic.Magic.Logic.Message.Chat;

    using ClashersRepublic.Magic.Services.Core.Message.Avatar;
    using ClashersRepublic.Magic.Services.Core.Utils;
    using ClashersRepublic.Magic.Services.GlobalChat.Network.Session;

    using ClashersRepublic.Magic.Titan.Util;

    internal class Room
    {
        private readonly LogicArrayList<NetGlobalChatSession> _sessions;

        /// <summary>
        ///     Initializes a new instance of the <see cref="Room"/> class.
        /// </summary>
        internal Room()
        {
            this._sessions = new LogicArrayList<NetGlobalChatSession>(64);
        }
        
        /// <summary>
        ///     Gets the number of clients.
        /// </summary>
        internal int GetClients()
        {
            return this._sessions.Count;
        }

        /// <summary>
        ///     Adds the specified session to list.
        /// </summary>
        internal void AddSession(NetGlobalChatSession session)
        {
            this._sessions.Add(session);
        }

        /// <summary>
        ///     Removes the specified session to list.
        /// </summary>
        internal void RemoveSession(NetGlobalChatSession session)
        {
            int index = this._sessions.IndexOf(session);

            if (index != -1)
            {
                this._sessions.Remove(index);
            }
        }

        /// <summary>
        ///     Called when a message is received.
        /// </summary>
        internal void ReceiveMessage(AvatarEntry entry, string message)
        {
            if (!string.IsNullOrWhiteSpace(message))
            {
                message = ServiceGlobalChat.Regex.Replace(message, " ");

                for (int i = 0; i < this._sessions.Count; i++)
                {
                    GlobalChatLineMessage globalChatLineMessage = new GlobalChatLineMessage();

                    globalChatLineMessage.SetMessage(message);
                    globalChatLineMessage.SetAvatarId(entry.GetAvatarId());
                    globalChatLineMessage.SetHomeId(entry.GetHomeId());
                    globalChatLineMessage.SetAvatarName(entry.GetAvatarName());
                    globalChatLineMessage.SetAvatarExpLevel(entry.GetAvatarExpLevel());
                    globalChatLineMessage.SetAvatarLeagueType(entry.GetAvatarLeagueType());

                    if (entry.GetAllianceId() != null)
                    {
                        globalChatLineMessage.SetAllianceId(entry.GetAllianceId());
                        globalChatLineMessage.SetAllianceName(entry.GetAllianceName());
                        globalChatLineMessage.SetAllianceBadgeId(entry.GetAllianceBadgeId());
                    }

                    this._sessions[i].SendPiranhaMessage(NetUtils.SERVICE_NODE_TYPE_PROXY_CONTAINER, globalChatLineMessage);
                }
            }
        }
    }
}