namespace ClashersRepublic.Magic.Services.Avatar.Game
{
    using System.Threading.Tasks;
    using System.Collections.Concurrent;
    using ClashersRepublic.Magic.Logic.Avatar;
    using ClashersRepublic.Magic.Services.Avatar.Database;
    using ClashersRepublic.Magic.Services.Core;
    using ClashersRepublic.Magic.Services.Core.Database;

    using ClashersRepublic.Magic.Titan.Json;
    using ClashersRepublic.Magic.Titan.Math;

    internal static class AvatarManager
    {
        private static int[] _avatarCounters;
        private static string _passTokenChars = "abcdefghijklmnopqrstuvwxyz0123456789";
        
        private static ConcurrentDictionary<long, Avatar> _avatars;

        /// <summary>
        ///     Gets the total accounts.
        /// </summary>
        internal static int TotalAvatars
        {
            get
            {
                return AvatarManager._avatars.Count;
            }
        }

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            AvatarManager._avatarCounters = new int[DatabaseManager.GetDatabaseCount()];
            AvatarManager._avatars = new ConcurrentDictionary<long, Avatar>();
        }

        /// <summary>
        ///     Loads all avatars from database.
        /// </summary>
        internal static void LoadAvatars()
        {
            for (int highId = 0; highId < DatabaseManager.GetDatabaseCount(); highId++)
            {
                IDatabase database = DatabaseManager.GetDatabase(highId);

                if (database != null)
                {
                    int maxLowId = database.GetHigherId();
                    
                    Parallel.For(1, maxLowId + 1, new ParallelOptions { MaxDegreeOfParallelism = 3 }, id =>
                    {
                        string json = database.GetDocument(new LogicLong(highId, id));

                        if (json != null)
                        {
                            Avatar avatar = new Avatar();
                            avatar.Load(json);
                            AvatarManager.TryAdd(avatar);

                            AvatarManager._avatarCounters[highId] = id;
                        }
                    });
                }
                else
                {
                    Logging.Warning(typeof(AvatarManager), "AvatarManager::loadAccounts pDatabase->NULL");
                }
            }
        }

        /// <summary>
        ///     Tries to add the specified <see cref="Avatar"/> instance.
        /// </summary>
        private static bool TryAdd(Avatar avatar)
        {
            return AvatarManager._avatars.TryAdd(avatar.Id, avatar);
        }

        /// <summary>
        ///     Tries to get the instance of the specified <see cref="Avatar"/> id.
        /// </summary>
        private static bool TryGet(LogicLong avatarId, out Avatar avatar)
        {
            return AvatarManager._avatars.TryGetValue(avatarId, out avatar);
        }

        /// <summary>
        ///     Tries to remove the specified <see cref="Avatar"/> instance.
        /// </summary>
        private static bool TryRemove(LogicLong avatarId, out Avatar avatar)
        {
            return AvatarManager._avatars.TryRemove(avatarId, out avatar);
        }
        
        /// <summary>
        ///     Tries to create a new <see cref="Avatar"/> instance.
        /// </summary>
        internal static bool TryCreateAvatar(LogicLong avatarId, out Avatar avatar)
        {
            IDatabase database = DatabaseManager.GetDatabase(avatarId.GetHigherInt());

            if (database != null)
            {
                avatar = new Avatar(avatarId, LogicClientAvatar.GetDefaultAvatar());

                bool success = AvatarManager.TryAdd(avatar);

                if (success)
                {
                    database.InsertDocument(avatarId, LogicJSONParser.CreateJSONString(avatar.Save()));
                }

                return success;
            }

            avatar = null;

            return false;
        }

        /// <summary>
        ///     Tries to get the <see cref="Avatar"/> instance with id.
        /// </summary>
        internal static bool TryGetAvatar(LogicLong avatarId, out Avatar avatar)
        {
            return AvatarManager.TryGet(avatarId, out avatar);
        }
    }
}