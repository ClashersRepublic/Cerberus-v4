namespace ClashersRepublic.Magic.Services.Avatar.Game
{
    using ClashersRepublic.Magic.Logic.Helper;

    using ClashersRepublic.Magic.Services.Avatar.Network.Session;
    using ClashersRepublic.Magic.Services.Core.Message.Avatar;
    using ClashersRepublic.Magic.Titan.Json;
    using ClashersRepublic.Magic.Titan.Math;

    internal class AvatarAccount
    {
        /// <summary>
        ///     Gets the <see cref="AvatarAccount"/> id.
        /// </summary>
        internal LogicLong Id { get; private set; }

        /// <summary>
        ///     Gets the current session.
        /// </summary>
        internal NetAvatarSession Session { get; private set; }

        /// <summary>
        ///     Gets the avatar entry.
        /// </summary>
        internal AvatarEntry AvatarEntry { get; }
        
        /// <summary>
        ///     Initializes a new instance of the <see cref="AvatarAccount"/> class.
        /// </summary>
        internal AvatarAccount()
        {
            this.Id = new LogicLong();
            this.AvatarEntry = new AvatarEntry();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="AvatarAccount"/> class.
        /// </summary>
        internal AvatarAccount(LogicLong id) : this()
        {
            this.Id = id;
        }

        /// <summary>
        ///     Sets the <see cref="NetAvatarSession"/> instance.
        /// </summary>
        internal void SetSession(NetAvatarSession session)
        {
            this.Session = session;
        }
        
        /// <summary>
        ///     Loads this instance from json.
        /// </summary>
        internal void Load(string json)
        {
            LogicJSONObject jsonObject = (LogicJSONObject) LogicJSONParser.Parse(json);

            this.Id = new LogicLong(LogicJSONHelper.GetJSONNumber(jsonObject, "id_hi"), LogicJSONHelper.GetJSONNumber(jsonObject, "id_lo"));

            LogicJSONObject entryObject = jsonObject.GetJSONObject("entry");

            if (entryObject != null)
            {
                this.AvatarEntry.Load(entryObject);
            }
        }

        /// <summary>
        ///     Saves this instance to json.
        /// </summary>
        internal LogicJSONObject Save()
        {
            LogicJSONObject jsonObject = new LogicJSONObject();

            jsonObject.Put("id_hi", new LogicJSONNumber(this.Id.GetHigherInt()));
            jsonObject.Put("id_lo", new LogicJSONNumber(this.Id.GetLowerInt()));
            jsonObject.Put("entry", this.AvatarEntry.Save());

            return jsonObject;
        }
    }
}