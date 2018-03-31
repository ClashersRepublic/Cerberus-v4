namespace ClashersRepublic.Magic.Services.Chat.Game
{
    using ClashersRepublic.Magic.Logic.Message.Chat;

    using ClashersRepublic.Magic.Services.Core.Message.Avatar;
    using ClashersRepublic.Magic.Services.Core.Utils;
    using ClashersRepublic.Magic.Services.Chat.Network.Session;

    using ClashersRepublic.Magic.Titan.Util;

    internal class Room
    {
        private readonly LogicArrayList<NetGlobalChatSession> _sessions;
        private readonly int _roomId;

        /// <summary>
        ///     Gets the room id.
        /// </summary>
        internal int RoomId
        {
            get
            {
                return this._roomId;
            }
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Room"/> class.
        /// </summary>
        internal Room(int id)
        {
            this._sessions = new LogicArrayList<NetGlobalChatSession>(32);
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
            int index = -1;

            for (int i = 0; i < this._sessions.Count; i++)
            {
                if (this._sessions[i] == session)
                {
                    index = i;
                    break;
                }
            }

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

                globalChatLineMessage.Encode();

                for (int i = 0; i < this._sessions.Count; i++)
                {
                    this._sessions[i].SendPiranhaMessage(NetUtils.SERVICE_NODE_TYPE_PROXY_CONTAINER, globalChatLineMessage);
                }
            }
        }
    }
}