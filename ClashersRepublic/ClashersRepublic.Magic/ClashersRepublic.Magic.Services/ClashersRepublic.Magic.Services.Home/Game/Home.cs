namespace ClashersRepublic.Magic.Services.Home.Game
{
    using ClashersRepublic.Magic.Logic.Avatar;
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Logic.Home;

    using ClashersRepublic.Magic.Services.Core;
    using ClashersRepublic.Magic.Services.Home.Game.Mode;
    using ClashersRepublic.Magic.Services.Home.Network.Session;
    using ClashersRepublic.Magic.Services.Home.Resource;

    using ClashersRepublic.Magic.Titan.Json;
    using ClashersRepublic.Magic.Titan.Math;
    using ClashersRepublic.Magic.Titan.Util;

    internal class Home
    {
        /// <summary>
        ///     Gets the <see cref="Home"/> id.
        /// </summary>
        internal LogicLong Id { get; private set; }
        
        /// <summary>
        ///     Gets the <see cref="LogicClientAvatar"/> instance.
        /// </summary>
        internal LogicClientAvatar ClientAvatar { get; private set; }

        /// <summary>
        ///     Gets the <see cref="LogicClientHome"/> instance.
        /// </summary>
        internal LogicClientHome ClientHome { get; private set; }

        /// <summary>
        ///     Gets the current session.
        /// </summary>
        internal NetHomeSession Session { get; private set; }

        /// <summary>
        ///     Gets the <see cref="Mode.GameMode"/> instance
        /// </summary>
        internal GameMode GameMode { get; }

        /// <summary>
        ///     Gets the save timestamp.
        /// </summary>
        internal int SaveTimestamp { get; private set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Home"/> class.
        /// </summary>
        public Home()
        {
            this.Id = new LogicLong();
            this.GameMode = new GameMode(this);
            this.ClientAvatar = new LogicClientAvatar();
            this.ClientHome = new LogicClientHome();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Home"/> class.
        /// </summary>
        public Home(LogicLong id) : this()
        {
            this.Id = id;
            this.ClientAvatar = LogicClientAvatar.GetDefaultAvatar();
            this.ClientAvatar.SetId(id);
            this.ClientAvatar.SetHomeId(id);
            this.ClientHome = new LogicClientHome();
            this.ClientHome.SetHomeId(id);
            this.ClientHome.SetShieldDurationSeconds(4 * 86400);
            this.ClientHome.SetGuardDurationSeconds(1 * 86400);
            this.ClientHome.SetNextMaintenanceSeconds(4 * 3600);
            this.ClientHome.SetHomeJSON(HomeResourceManager.GetStartingHomeJSON());
            
            CompressibleStringHelper.Compress(this.ClientHome.GetCompressibleHomeJSON());
        }

        /// <summary>
        ///     Sets the <see cref="LogicClientAvatar"/> instance.
        /// </summary>
        internal void SetClientAvatar(LogicClientAvatar instance)
        {
            this.ClientAvatar.Destruct();
            this.ClientAvatar = instance;
        }

        /// <summary>
        ///     Sets the <see cref="LogicClientAvatar"/> instance.
        /// </summary>
        internal void SetClientHome(LogicClientHome instance)
        {
            this.ClientHome.Destruct();
            this.ClientHome = instance;
        }

        /// <summary>
        ///     Sets the session instance.
        /// </summary>
        internal void SetSession(NetHomeSession session)
        {
            this.Session = session;
        }

        /// <summary>
        ///     Saves this instance to json.
        /// </summary>
        internal LogicJSONObject Save()
        {
            this.SaveTimestamp = LogicTimeUtil.GetTimestamp();

            LogicJSONObject jsonObject = new LogicJSONObject();

            jsonObject.Put("id_hi", new LogicJSONNumber(this.Id.GetHigherInt()));
            jsonObject.Put("id_lo", new LogicJSONNumber(this.Id.GetLowerInt()));
            jsonObject.Put("last_save", new LogicJSONNumber(this.SaveTimestamp));
            jsonObject.Put("avatar", this.ClientAvatar.Save());
            jsonObject.Put("home", this.ClientHome.Save());

            return jsonObject;
        }

        /// <summary>
        ///     Loads this instance from json.
        /// </summary>
        internal void Load(string json)
        {
            LogicJSONObject jsonObject = (LogicJSONObject) LogicJSONParser.Parse(json);

            this.Id = new LogicLong(LogicJSONHelper.GetJSONNumber(jsonObject, "id_hi"), LogicJSONHelper.GetJSONNumber(jsonObject, "id_lo"));

            LogicJSONNumber lastSaveObject = jsonObject.GetJSONNumber("last_save");

            if (lastSaveObject != null)
            {
                this.SaveTimestamp = lastSaveObject.GetIntValue();
            }
            else
            {
                Logging.Warning("Home::load save timestamp is not set");
                this.SaveTimestamp = LogicTimeUtil.GetTimestamp();
            }

            LogicJSONObject avatarObject = jsonObject.GetJSONObject("avatar");

            if (avatarObject != null)
            {
                this.ClientAvatar.Load(avatarObject);
            }

            LogicJSONObject homeObject = jsonObject.GetJSONObject("home");

            if (homeObject != null)
            {
                this.ClientHome.Load(homeObject);
            }
        }
    }
}