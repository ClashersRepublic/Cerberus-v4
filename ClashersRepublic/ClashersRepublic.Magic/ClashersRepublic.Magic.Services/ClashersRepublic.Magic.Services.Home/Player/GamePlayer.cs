namespace ClashersRepublic.Magic.Services.Home.Player
{
    using ClashersRepublic.Magic.Logic.Avatar;
    using ClashersRepublic.Magic.Logic.Home;

    using ClashersRepublic.Magic.Services.Logic.Resource;
    using ClashersRepublic.Magic.Services.Logic.Snappy;

    using ClashersRepublic.Magic.Titan.Json;
    using ClashersRepublic.Magic.Titan.Math;
    using ClashersRepublic.Magic.Titan.Util;

    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    internal class GamePlayer
    {
        [BsonId] public ObjectId _id;

        public int HighId;
        public int LowId;

        public int LastSaveTime;
        
        public SnappyString LogicClientHomeJSON;
        public SnappyString LogicClientAvatarJSON;

        internal LogicClientHome LogicClientHome;
        internal LogicClientAvatar LogicClientAvatar;

        internal string CurrentSessionId { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GamePlayer"/> class.
        /// </summary>
        public GamePlayer()
        {
            // GamePlayer.
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

            this.LogicClientAvatar.SetId(accountId);
            this.LogicClientAvatar.SetHomeId(accountId);

            this.LogicClientHome.SetHomeId(accountId);
            this.LogicClientHome.SetHomeJSON(ResourceManager.StartingHomeJson);

            this.UpdateDocuments();
        }

        /// <summary>
        ///     Updates documents instance.
        /// </summary>
        public void UpdateDocuments()
        {
            this.LogicClientHomeJSON.Update(LogicJSONParser.CreateJSONString(this.LogicClientHome.Save()));
            this.LogicClientAvatarJSON.Update(LogicJSONParser.CreateJSONString(this.LogicClientAvatar.Save()));
            this.LastSaveTime = LogicTimeUtil.GetTimestamp();
        }

        /// <summary>
        ///     Loads instances from documents.
        /// </summary>
        public void LoadFromDocuments()
        {
            this.LogicClientHome = new LogicClientHome();
            this.LogicClientAvatar = new LogicClientAvatar();

            this.LogicClientHome.Load((LogicJSONObject) LogicJSONParser.Parse(this.LogicClientHomeJSON));
            this.LogicClientAvatar.Load((LogicJSONObject) LogicJSONParser.Parse(this.LogicClientAvatarJSON));
        }
    }
}