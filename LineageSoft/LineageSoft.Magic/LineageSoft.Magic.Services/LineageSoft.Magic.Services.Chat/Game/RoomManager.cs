namespace LineageSoft.Magic.Services.Chat.Game
{
    using LineageSoft.Magic.Services.Chat.Network.Session;
    using LineageSoft.Magic.Titan.Util;

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
        ///     Gets the total sessions in room.
        /// </summary>
        internal static int InRoomSessions
        {
            get
            {
                int count = 0;

                for (int i = 0; i < RoomManager._rooms.Count; i++)
                {
                    count += RoomManager._rooms[i].GetClients();
                }

                return count;
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

                if (room.GetClients() < room.GetCapacity())
                {
                    room.AddSession(session);
                    session.SetRoom(room);

                    return;
                }
            }

            Room newRoom = new Room(RoomManager._rooms.Count);

            newRoom.AddSession(session);
            session.SetRoom(newRoom);

            RoomManager._rooms.Add(newRoom);
        }

        /// <summary>
        ///     Leaves the joined room.
        /// </summary>
        internal static void LeaveRoom(NetGlobalChatSession session)
        {
            Room room = session.Room;

            session.SetRoom(null);
            room.RemoveSession(session);
        }
    }
}