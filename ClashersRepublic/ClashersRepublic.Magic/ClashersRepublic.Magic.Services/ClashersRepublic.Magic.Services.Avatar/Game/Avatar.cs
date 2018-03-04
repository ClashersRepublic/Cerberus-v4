namespace ClashersRepublic.Magic.Services.Avatar.Game
{
    using ClashersRepublic.Magic.Logic.Avatar;
    using ClashersRepublic.Magic.Logic.Helper;
    
    using ClashersRepublic.Magic.Titan.Json;
    using ClashersRepublic.Magic.Titan.Math;
    
    internal class Avatar
    {
        /// <summary>
        ///     Gets the <see cref="Avatar"/> id.
        /// </summary>
        internal LogicLong Id { get; private set; }
        
        /// <summary>
        ///     Gets the <see cref="LogicClientAvatar"/> instance.
        /// </summary>
        internal LogicClientAvatar ClientAvatar { get; private set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Avatar"/> class.
        /// </summary>
        public Avatar()
        {
            this.Id = new LogicLong();
            this.ClientAvatar = new LogicClientAvatar();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Avatar"/> class.
        /// </summary>
        public Avatar(LogicLong avatarId, LogicClientAvatar clientAvatar) : this()
        {
            this.Id = avatarId;
            this.ClientAvatar = clientAvatar;
            this.ClientAvatar.SetId(avatarId);
            this.ClientAvatar.SetHomeId(avatarId);
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
        ///     Saves this instance to json.
        /// </summary>
        internal LogicJSONObject Save()
        {
            LogicJSONObject jsonObject = new LogicJSONObject();

            jsonObject.Put("acc_hi", new LogicJSONNumber(this.Id.GetHigherInt()));
            jsonObject.Put("acc_lo", new LogicJSONNumber(this.Id.GetLowerInt()));
            
            LogicJSONObject avatarObject = new LogicJSONObject();
            this.ClientAvatar.Save(avatarObject);
            jsonObject.Put("avatar", avatarObject);

            return jsonObject;
        }

        /// <summary>
        ///     Loads this instance from json.
        /// </summary>
        internal void Load(string json)
        {
            LogicJSONObject jsonObject = (LogicJSONObject) LogicJSONParser.Parse(json);

            this.Id = new LogicLong(LogicJSONHelper.GetJSONNumber(jsonObject, "acc_hi"), LogicJSONHelper.GetJSONNumber(jsonObject, "acc_lo"));

            LogicJSONObject avatarObject = jsonObject.GetJSONObject("avatar");

            if (avatarObject != null)
            {
                this.ClientAvatar.Load(avatarObject);
            }
        }
    }
}