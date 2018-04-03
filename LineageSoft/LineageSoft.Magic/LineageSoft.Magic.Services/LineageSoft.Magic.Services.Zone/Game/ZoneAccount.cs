namespace LineageSoft.Magic.Services.Zone.Game
{
    using LineageSoft.Magic.Logic.Avatar;
    using LineageSoft.Magic.Logic.Helper;
    using LineageSoft.Magic.Logic.Home;

    using LineageSoft.Magic.Services.Core;
    using LineageSoft.Magic.Services.Zone.Game.Mode;
    using LineageSoft.Magic.Services.Zone.Network.Session;
    using LineageSoft.Magic.Services.Zone.Resource;

    using LineageSoft.Magic.Titan.Json;
    using LineageSoft.Magic.Titan.Math;
    using LineageSoft.Magic.Titan.Util;

    internal class ZoneAccount
    {
        /// <summary>
        ///     Gets the <see cref="ZoneAccount"/> id.
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
        internal NetZoneSession Session { get; private set; }

        /// <summary>
        ///     Gets the <see cref="Mode.GameMode"/> instance
        /// </summary>
        internal GameMode GameMode { get; }

        /// <summary>
        ///     Gets the save timestamp.
        /// </summary>
        internal int SaveTimestamp { get; private set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ZoneAccount"/> class.
        /// </summary>
        public ZoneAccount()
        {
            this.Id = new LogicLong();
            this.GameMode = new GameMode(this);
            this.ClientAvatar = new LogicClientAvatar();
            this.ClientHome = new LogicClientHome();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="ZoneAccount"/> class.
        /// </summary>
        public ZoneAccount(LogicLong id) : this()
        {
            this.Id = id;
            this.ClientAvatar = LogicClientAvatar.GetDefaultAvatar();
            this.ClientAvatar.SetId(id);
            this.ClientAvatar.SetCurrentHomeId(id);
            this.ClientHome = new LogicClientHome();
            this.ClientHome.SetHomeId(id);
            this.ClientHome.SetShieldDurationSeconds(4 * 86400);
            this.ClientHome.SetGuardDurationSeconds(1 * 86400);
            this.ClientHome.SetNextMaintenanceSeconds(4 * 3600);
            this.ClientHome.SetHomeJSON(HomeResourceManager.GetStartingHomeJSON());
            
            CompressibleStringHelper.Compress(this.ClientHome.GetCompressibleHomeJSON());
        }

        /// <summary>
        ///     Sets the save timestamp.
        /// </summary>
        internal void SetSaveTimestamp(int timestamp)
        {
            this.SaveTimestamp = timestamp;
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
        internal void SetSession(NetZoneSession session)
        {
            this.Session = session;
        }

        /// <summary>
        ///     Saves this instance to json.
        /// </summary>
        internal LogicJSONObject Save()
        {
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
                Logging.Warning("ZoneAccount::load save timestamp is not set");
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