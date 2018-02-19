namespace ClashersRepublic.Magic.Services.Home.Home
{
    using ClashersRepublic.Magic.Logic.Avatar;
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Logic.Home;
    using ClashersRepublic.Magic.Logic.Utils;
    using ClashersRepublic.Magic.Services.Logic.Math;
    using ClashersRepublic.Magic.Services.Logic.Service.Setting;
    using ClashersRepublic.Magic.Services.Logic.Snappy;

    using ClashersRepublic.Magic.Titan.Json;
    using ClashersRepublic.Magic.Titan.Math;
    using ClashersRepublic.Magic.Titan.Util;

    using Newtonsoft.Json;

    public class GameHome
    {
        [JsonConverter(typeof(LogicLongSerializer))] public LogicLong Id;

        [JsonConverter(typeof(SnappyStringSerializer))] public SnappyString LogicClientHomeJson;
        [JsonConverter(typeof(SnappyStringSerializer))] public SnappyString LogicClientAvatarJson;

        public int SaveTimestamp;

        internal int State { get; private set; }

        internal LogicClientHome LogicClientHomeInstance { get; private set; }
        internal LogicClientAvatar LogicClientAvatarInstance { get; private set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GameHome"/> class.
        /// </summary>
        public GameHome()
        {
            this.LogicClientHomeJson = new SnappyString();
            this.LogicClientAvatarJson = new SnappyString();
            this.LogicClientHomeInstance = new LogicClientHome();
            this.LogicClientAvatarInstance = new LogicClientAvatar();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GameHome"/> class.
        /// </summary>
        public GameHome(LogicLong id) : this()
        {
            this.Id = id;
        }

        /// <summary>
        ///     Creates default <see cref="GameHome"/> instance.
        /// </summary>
        public static GameHome GetDefaultGameHome(LogicLong id)
        {
            GameHome home = new GameHome(id);

            if (home.LogicClientAvatarInstance != null)
            {
                home.LogicClientAvatarInstance.Destruct();
                home.LogicClientAvatarInstance = null;
            }

            if (home.LogicClientHomeInstance != null)
            {
                home.LogicClientHomeInstance.Destruct();
                home.LogicClientHomeInstance = null;
            }

            ZLibHelper.ConpressInZLibFormat(LogicStringUtil.GetBytes(ServiceLogicConfig.GetStartingHomeJson()), out byte[] compressed);

            home.LogicClientHomeInstance = new LogicClientHome();
            home.LogicClientAvatarInstance = LogicClientAvatar.GetDefaultAvatar();

            home.LogicClientAvatarInstance.SetId(id);
            home.LogicClientAvatarInstance.SetHomeId(id);

            home.LogicClientHomeInstance.SetHomeId(id);
            home.LogicClientHomeInstance.GetCompressibleHomeJSON().Set(compressed, compressed.Length);

            home.UpdateDatas();

            return home;
        }

        /// <summary>
        ///     Updates logic datas.
        /// </summary>
        public void UpdateDatas()
        {
            this.SaveTimestamp = LogicTimeUtil.GetTimestamp();
            this.SetDatas(LogicJSONParser.CreateJSONString(this.LogicClientHomeInstance.Save()), LogicJSONParser.CreateJSONString(this.LogicClientAvatarInstance.Save()));
        }

        /// <summary>
        ///     Reloads logic datas.
        /// </summary>
        public void ReloadDatas()
        {
            this.LogicClientHomeInstance.Load((LogicJSONObject) LogicJSONParser.Parse(this.LogicClientHomeJson));
            this.LogicClientAvatarInstance.Load((LogicJSONObject) LogicJSONParser.Parse(this.LogicClientAvatarJson));
        }

        /// <summary>
        ///     Sets logic datas.
        /// </summary>
        public void SetDatas(string homeData, string avatarData)
        {
            this.LogicClientHomeJson.Update(homeData);
            this.LogicClientAvatarJson.Update(avatarData);
        }

        /// <summary>
        ///     Gets if this <see cref="GameHome"/> instance is in none state.
        /// </summary>
        public bool IsInNoneState()
        {
            return this.State == 0;
        }

        /// <summary>
        ///     Gets if this <see cref="GameHome"/> instance is in home state.
        /// </summary>
        public bool IsInHomeState()
        {
            return this.State == 1;
        }

        /// <summary>
        ///     Gets if this <see cref="GameHome"/> instance is in attack state.
        /// </summary>
        public bool IsInAttackState()
        {
            return this.State == 2;
        }

        /// <summary>
        ///     Gets if this <see cref="GameHome"/> instance is in defense state.
        /// </summary>
        public bool IsInDefenseState()
        {
            return this.State == 3;
        }

        /// <summary>
        ///     Gets if this <see cref="GameHome"/> instance is in visit state.
        /// </summary>
        public bool IsInVisitState()
        {
            return this.State == 4;
        }

        /// <summary>
        ///     Gets if this <see cref="GameHome"/> instance is in replay state.
        /// </summary>
        public bool IsInReplayState()
        {
            return this.State == 5;
        }

        /// <summary>
        ///     Sets the game state.
        /// </summary>
        internal void SetState(int state)
        {
            this.State = state;
        }
    }
}