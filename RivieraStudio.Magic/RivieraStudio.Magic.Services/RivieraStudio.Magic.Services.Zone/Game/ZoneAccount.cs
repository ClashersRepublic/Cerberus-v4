namespace RivieraStudio.Magic.Services.Zone.Game
{
    using RivieraStudio.Magic.Logic.Avatar;
    using RivieraStudio.Magic.Logic.Helper;
    using RivieraStudio.Magic.Logic.Home;

    using RivieraStudio.Magic.Services.Core;
    using RivieraStudio.Magic.Services.Zone.Game.Mode;
    using RivieraStudio.Magic.Services.Zone.Network.Session;
    using RivieraStudio.Magic.Services.Zone.Resource;

    using RivieraStudio.Magic.Titan.Json;
    using RivieraStudio.Magic.Titan.Math;
    using RivieraStudio.Magic.Titan.Util;

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
        ///     Gets the unload timestamp.
        /// </summary>
        internal int UnloadTimestamp { get; private set; }

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
        ///     Sets the unload timestamp.
        /// </summary>
        internal void SetUnloadTimestamp(int timestamp)
        {
            this.UnloadTimestamp = timestamp;
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
            jsonObject.Put("save_time", new LogicJSONNumber(this.SaveTimestamp));
            jsonObject.Put("unload_time", new LogicJSONNumber(this.UnloadTimestamp));
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
            LogicJSONNumber saveTime = jsonObject.GetJSONNumber("save_time");
            LogicJSONNumber unloadTime = jsonObject.GetJSONNumber("unload_time");

            if (saveTime == null)
            {
                saveTime = jsonObject.GetJSONNumber("last_save");
            }

            this.Id = new LogicLong(LogicJSONHelper.GetJSONNumber(jsonObject, "id_hi"), LogicJSONHelper.GetJSONNumber(jsonObject, "id_lo"));

            this.SaveTimestamp = saveTime != null ? saveTime.GetIntValue() : LogicTimeUtil.GetTimestamp();
            this.UnloadTimestamp = unloadTime != null ? unloadTime.GetIntValue() : -1;

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