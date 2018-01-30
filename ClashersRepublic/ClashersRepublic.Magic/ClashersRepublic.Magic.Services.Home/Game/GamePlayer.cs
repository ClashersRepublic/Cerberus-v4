namespace ClashersRepublic.Magic.Services.Home.Game
{
    using ClashersRepublic.Magic.Logic.Avatar;
    using ClashersRepublic.Magic.Logic.Home;
    using ClashersRepublic.Magic.Services.Logic.Resource;
    using ClashersRepublic.Magic.Titan.Math;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    internal class GamePlayer
    {
        [BsonId]
        public ObjectId _id;

        public int HighId;
        public int LowId;

        public int SaveTime;

        internal string CurrentSessionId;

        public BsonDocument HomeDocument;
        public BsonDocument AvatarDocument;

        internal LogicClientHome LogicClientHomeInstance;
        internal LogicClientAvatar LogicClientAvatarInstance;

        /// <summary>
        ///     Initializes a new instance of the <see cref="GamePlayer"/> class.
        /// </summary>
        public GamePlayer()
        {
            this.LogicClientHomeInstance = new LogicClientHome();
            this.LogicClientAvatarInstance = new LogicClientAvatar();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GamePlayer"/> class.
        /// </summary>
        public GamePlayer(LogicLong accountId)
        {
            this.LogicClientHomeInstance = new LogicClientHome();
            this.LogicClientAvatarInstance = LogicClientAvatar.GetDefaultAvatar();

            this.HighId = accountId.GetHigherInt();
            this.LowId = accountId.GetLowerInt();

            this.LogicClientHomeInstance.SetHomeJSON(ResourceManager.StartingHomeJson);
            this.LogicClientHomeInstance.SetShieldDurationSeconds(ResourceManager.StartingHomeShieldDurationSeconds);
            this.LogicClientHomeInstance.SetGuardDurationSeconds(ResourceManager.StartingHomeGuardDurationSeconds);
            this.LogicClientHomeInstance.SetNextMaintenanceSeconds(14400);
        }
    }
}