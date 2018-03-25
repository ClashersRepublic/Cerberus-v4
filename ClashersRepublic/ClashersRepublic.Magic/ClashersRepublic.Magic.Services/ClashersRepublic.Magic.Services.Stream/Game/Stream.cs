namespace ClashersRepublic.Magic.Services.Stream.Game
{
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Services.Core.Message.Avatar;
    using ClashersRepublic.Magic.Services.Stream.Network.Session;
    using ClashersRepublic.Magic.Titan.Json;
    using ClashersRepublic.Magic.Titan.Math;
    using ClashersRepublic.Magic.Titan.Util;

    internal class Stream
    {
        /// <summary>
        ///     Gets the <see cref="Stream"/> id.
        /// </summary>
        internal LogicLong Id { get; private set; }

        /// <summary>
        ///     Gets the current session.
        /// </summary>
        internal NetStreamSession Session { get; private set; }

        /// <summary>
        ///     Gets the avatar entry.
        /// </summary>
        internal AvatarEntry AvatarEntry { get; private set; }

        internal LogicArrayList<Stream> Streams { get; private set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Stream"/> class.
        /// </summary>
        internal Stream()
        {
            this.Id = new LogicLong();
            this.AvatarEntry = new AvatarEntry();
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Stream"/> class.
        /// </summary>
        internal Stream(LogicLong id) : this()
        {
            this.Id = id;
        }

        /// <summary>
        ///     Sets the <see cref="NetStreamSession"/> instance.
        /// </summary>
        internal void SetSession(NetStreamSession session)
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
            this.AvatarEntry = new AvatarEntry();
        }

        /// <summary>
        ///     Saves this instance to json.
        /// </summary>
        internal LogicJSONObject Save()
        {
            LogicJSONObject jsonObject = new LogicJSONObject();

            jsonObject.Put("id_hi", new LogicJSONNumber(this.Id.GetHigherInt()));
            jsonObject.Put("id_lo", new LogicJSONNumber(this.Id.GetLowerInt()));

            return jsonObject;
        }
    }
}