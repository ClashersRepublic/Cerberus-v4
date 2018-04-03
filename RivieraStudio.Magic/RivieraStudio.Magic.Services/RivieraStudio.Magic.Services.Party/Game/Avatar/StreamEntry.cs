namespace RivieraStudio.Magic.Services.Party.Game.Avatar
{
    using System;

    using RivieraStudio.Magic.Logic.Message.Avatar.Stream;
    using RivieraStudio.Magic.Titan.Json;
    using RivieraStudio.Magic.Titan.Util;

    public class StreamEntry
    {
        private AvatarStreamEntry _avatarStreamEntry;
        private DateTime _createTime;

        /// <summary>
        ///     Initializes a new instance of the <see cref="StreamEntry"/> class.
        /// </summary>
        public StreamEntry()
        {
            this._createTime = DateTime.UtcNow;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="StreamEntry"/> class.
        /// </summary>
        public StreamEntry(AvatarStreamEntry entry)
        {
            this._avatarStreamEntry = entry;
            this._createTime = DateTime.UtcNow;
        }

        /// <summary>
        ///     Gets the avatar partyAccount entry.
        /// </summary>
        public AvatarStreamEntry GetAvatarStreamEntry()
        {
            return this._avatarStreamEntry;
        }

        /// <summary>
        ///     Gets the creation time.
        /// </summary>
        public DateTime GetCreationTime()
        {
            return this._createTime;
        }

        /// <summary>
        ///     Saves this instance to json.
        /// </summary>
        public LogicJSONObject Save()
        {
            LogicJSONObject jsonObject = new LogicJSONObject();
            LogicJSONObject entryObject = new LogicJSONObject();

            this._avatarStreamEntry.Save(entryObject);

            jsonObject.Put("type", new LogicJSONNumber(this._avatarStreamEntry.GetAvatarStreamEntryType()));
            jsonObject.Put("entry", entryObject);
            jsonObject.Put("time", new LogicJSONNumber(LogicTimeUtil.GetTimestamp(this._createTime)));

            return jsonObject;
        }

        /// <summary>
        ///     Loads this instance from json.
        /// </summary>
        public void Load(LogicJSONObject jsonObject)
        {
            LogicJSONNumber typeObject = jsonObject.GetJSONNumber("type");

            if (typeObject != null)
            {
                this._avatarStreamEntry = AvatarStreamEntryFactory.CreateStreamEntryByType(typeObject.GetIntValue());

                LogicJSONObject entryObject = jsonObject.GetJSONObject("entry");

                if (entryObject != null)
                {
                    this._avatarStreamEntry.Load(entryObject);
                }
            }

            LogicJSONNumber timeObject = jsonObject.GetJSONNumber("time");
            
            if (timeObject != null)
            {
                this._createTime = LogicTimeUtil.GetDateTimeFromTimestamp(timeObject.GetIntValue());
            }
        }
    }
}