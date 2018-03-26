namespace ClashersRepublic.Magic.Services.GlobalChat.Game
{
    using ClashersRepublic.Magic.Services.GlobalChat.Network.Session;
    using ClashersRepublic.Magic.Titan.Util;

    internal static class RoomManager
    {
        private static LogicArrayList<Room> _rooms;

        /// <summary>
        ///     Gets the total rooms.
        /// </summary>
        internal static int TotalRooms
        {
            get
            {
                return RoomManager._rooms.Count;
            }
        }

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            RoomManager._rooms = new LogicArrayList<Room>(1024);
        }

        /// <summary>
        ///     Joins a room.
        /// </summary>
        internal static void JoinRoom(NetGlobalChatSession session)
        {
            for (int i = 0; i < RoomManager._rooms.Count; i++)
            {
                Room room = RoomManager._rooms[i];

                if (room.GetClients() < 64)
                {
                    room.AddSession(session);
                    session.SetRoom(room);

                    return;
                }
            }

            Room newRoom = new Room();
            newRoom.AddSession(session);
            session.SetRoom(newRoom);
            RoomManager._rooms.Add(newRoom);
        }

        /// <summary>
        ///     Leaves the joined room.
        /// </summary>
        internal static void LeaveRoom(NetGlobalChatSession session)
        {
            session.Room.RemoveSession(session);
        }
    }
}