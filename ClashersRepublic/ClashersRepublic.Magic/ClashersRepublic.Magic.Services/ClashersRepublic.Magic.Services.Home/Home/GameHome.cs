// #define USE_SNAPPY

namespace ClashersRepublic.Magic.Services.Home.Home
{
    using ClashersRepublic.Magic.Logic.Avatar;
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Logic.Home;
    using ClashersRepublic.Magic.Services.Home.Game.Mode;
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
        
        public string LogicClientHomeJson;
        public string LogicClientAvatarJson;

        public int SaveTimestamp;
        
        internal GameMode GameMode { get; private set; }
        internal LogicClientHome LogicClientHomeInstance { get; private set; }
        internal LogicClientAvatar LogicClientAvatarInstance { get; private set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GameHome"/> class.
        /// </summary>
        public GameHome()
        {
            this.GameMode = new GameMode(this);
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

            ZLibHelper.CompressInZLibFormat(LogicStringUtil.GetBytes(ServiceLogicConfig.GetStartingHomeJson()), out byte[] compressed);

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
            this.LogicClientAvatarJson = avatarData;
            this.LogicClientHomeJson = homeData;
        }
    }
}