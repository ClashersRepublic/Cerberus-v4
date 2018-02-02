namespace ClashersRepublic.Magic.Services.Home.Player
{
    using ClashersRepublic.Magic.Logic.Avatar;
    using ClashersRepublic.Magic.Logic.Home;
    using ClashersRepublic.Magic.Titan.Math;
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    internal class GamePlayer
    {
        [BsonId]
        public ObjectId _id;

        public int HighId;
        public int LowId;

        public BsonDocument HomeDocument;
        public BsonDocument AvatarDocument;

        internal LogicClientHome LogicClientHome;
        internal LogicClientAvatar LogicClientAvatar;

        /// <summary>
        ///     Initializes a new instance of the <see cref="GamePlayer"/> class.
        /// </summary>
        public GamePlayer()
        {
            this.LogicClientHome = new LogicClientHome();
            this.LogicClientAvatar = new LogicClientAvatar();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GamePlayer"/> class.
        /// </summary>
        public GamePlayer(LogicLong accountId)
        {
            this.HighId = accountId.GetHigherInt();
            this.LowId = accountId.GetLowerInt();

            this.LogicClientHome = new LogicClientHome();
            this.LogicClientAvatar = LogicClientAvatar.GetDefaultAvatar();
        }
    }
}