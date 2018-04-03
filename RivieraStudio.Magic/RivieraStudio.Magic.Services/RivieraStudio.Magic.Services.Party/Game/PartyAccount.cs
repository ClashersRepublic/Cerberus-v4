namespace RivieraStudio.Magic.Services.Party.Game
{
    using RivieraStudio.Magic.Logic.Helper;
    using RivieraStudio.Magic.Logic.Message.Avatar.Stream;
    using RivieraStudio.Magic.Services.Core.Message.Avatar;
    using RivieraStudio.Magic.Services.Party.Game.Avatar;
    using RivieraStudio.Magic.Services.Party.Network.Session;

    using RivieraStudio.Magic.Titan.Json;
    using RivieraStudio.Magic.Titan.Math;
    using RivieraStudio.Magic.Titan.Util;

    internal class PartyAccount
    {
        /// <summary>
        ///     Gets the <see cref="PartyAccount"/> id.
        /// </summary>
        internal LogicLong Id { get; private set; }

        /// <summary>
        ///     Gets the current session.
        /// </summary>
        internal NetPartySession Session { get; private set; }

        /// <summary>
        ///     Gets the avatar entry.
        /// </summary>
        internal AvatarEntry AvatarEntry { get; private set; }
        
        private LogicArrayList<StreamEntry> _streams;

        /// <summary>
        ///     Initializes a new instance of the <see cref="PartyAccount"/> class.
        /// </summary>
        internal PartyAccount()
        {
            this.Id = new LogicLong();
            this.AvatarEntry = new AvatarEntry();
            this._streams = new LogicArrayList<StreamEntry>(50);
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="PartyAccount"/> class.
        /// </summary>
        internal PartyAccount(LogicLong id) : this()
        {
            this.Id = id;
        }

        /// <summary>
        ///     Sets the <see cref="NetPartySession"/> instance.
        /// </summary>
        internal void SetSession(NetPartySession session)
        {
            this.Session = session;
        }

        /// <summary>
        ///     Gets the avatar stream entry array.
        /// </summary>
        internal LogicArrayList<AvatarStreamEntry> GetStreamEntries()
        {
            LogicArrayList<AvatarStreamEntry> entries = new LogicArrayList<AvatarStreamEntry>(this._streams.Count);

            for (int i = 0; i < this._streams.Count; i++)
            {
                entries.Add(this._streams[i].GetAvatarStreamEntry());
            }

            return entries;
        }

        /// <summary>
        ///     Loads this instance from json.
        /// </summary>
        internal void Load(string json)
        {
            LogicJSONObject jsonObject = (LogicJSONObject) LogicJSONParser.Parse(json);

            this.Id = new LogicLong(LogicJSONHelper.GetJSONNumber(jsonObject, "id_hi"), LogicJSONHelper.GetJSONNumber(jsonObject, "id_lo"));

            LogicJSONArray streamArray = jsonObject.GetJSONArray("partyAccount");

            if (streamArray != null)
            {
                int cnt = LogicMath.Min(streamArray.Size(), 50);

                for (int i = 0; i < cnt; i++)
                {
                    LogicJSONObject streamObject = streamArray.GetJSONObject(i);

                    if (streamObject != null)
                    {
                        StreamEntry entry = new StreamEntry();
                        entry.Load(streamObject);
                        this._streams.Add(entry);
                    }
                }
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
            
            LogicJSONArray jsonArray = new LogicJSONArray();

            for (int i = 0; i < this._streams.Count; i++)
            {
                jsonArray.Add(this._streams[i].Save());
            }

            jsonObject.Put("partyAccount", jsonArray);

            return jsonObject;
        }
    }
}