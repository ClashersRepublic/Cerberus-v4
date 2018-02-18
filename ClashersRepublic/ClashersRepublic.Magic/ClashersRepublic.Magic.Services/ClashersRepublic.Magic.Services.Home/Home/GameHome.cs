namespace ClashersRepublic.Magic.Services.Home.Home
{
    using ClashersRepublic.Magic.Logic.Avatar;
    using ClashersRepublic.Magic.Logic.Home;

    using ClashersRepublic.Magic.Services.Logic.Snappy;
    using ClashersRepublic.Magic.Titan.Math;

    public class GameHome
    {
        public int HighId;
        public int LowId;

        public SnappyString LogicClientHomeJson;
        public SnappyString LogicClientAvatarJson;

        internal LogicLong Id { get; private set; }
        internal LogicClientHome LogicClientHomeInstance { get; private set; }
        internal LogicClientAvatar LogicClientAvatarInstance { get; private set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GameHome"/> class.
        /// </summary>
        public GameHome()
        {
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
                home.LogicClientHomeInstance = null;
            }

            if (home.LogicClientHomeInstance != null)
            {
                home.LogicClientHomeInstance.Destruct();
                home.LogicClientHomeInstance = null;
            }

            home.LogicClientHomeInstance = new LogicClientHome();
            home.LogicClientAvatarInstance = LogicClientAvatar.GetDefaultAvatar();

            return home;
        }

        /// <summary>
        ///     Sets logic datas.
        /// </summary>
        public void SetDatas(string homeData, string avatarData)
        {
            this.LogicClientHomeJson.Update(homeData);
            this.LogicClientAvatarJson.Update(avatarData);
        }
    }
}