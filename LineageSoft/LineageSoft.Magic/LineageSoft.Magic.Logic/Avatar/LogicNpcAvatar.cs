namespace LineageSoft.Magic.Logic.Avatar
{
    using LineageSoft.Magic.Logic.Data;
    using LineageSoft.Magic.Logic.Helper;
    using LineageSoft.Magic.Logic.Util;
    using LineageSoft.Magic.Titan.DataStream;
    using LineageSoft.Magic.Titan.Util;

    public class LogicNpcAvatar : LogicAvatar
    {
        private LogicNpcData _npcData;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicNpcAvatar"/> class.
        /// </summary>
        public LogicNpcAvatar()
        {
            base.InitBase();
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();
            this._npcData = null;
        }

        /// <summary>
        ///     Gets the npc data.
        /// </summary>
        public LogicNpcData GetNpcData()
        {
            return this._npcData;
        }

        /// <summary>
        ///     Gets the exp level.
        /// </summary>
        public override int GetExpLevel()
        {
            return this._npcData.ExpLevel;
        }

        /// <summary>
        ///     Gets the name.
        /// </summary>
        public string GetName()
        {
            return this._npcData.PlayerName;
        }

        /// <summary>
        ///     Gets the alliance badge.
        /// </summary>
        public override int GetAllianceBadge()
        {
            return this._npcData.AllianceBadge;
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public void Encode(ChecksumEncoder encoder)
        {
            encoder.WriteDataReference(this._npcData);
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public void Decode(ByteStream stream)
        {
            this.SetNpcData((LogicNpcData) stream.ReadDataReference(16));
        }

        /// <summary>
        ///     Gets the <see cref="LogicNpcAvatar"/> instance.
        /// </summary>
        public static LogicNpcAvatar GetNpcAvatar(LogicNpcData data)
        {
            LogicNpcAvatar npcAvatar = new LogicNpcAvatar();
            npcAvatar.SetNpcData(data);
            return npcAvatar;
        }

        /// <summary>
        ///     Gets if this <see cref="LogicAvatar"/> instance is a npc avatar.
        /// </summary>
        public override bool IsNpcAvatar()
        {
            return true;
        }

        /// <summary>
        ///     Sets the npc data.
        /// </summary>
        public void SetNpcData(LogicNpcData data)
        {
            this._npcData = data;

            if (this._unitCount.Count != 0)
            {
                do
                {
                    this._unitCount[0].Destruct();
                    this._unitCount.Remove(0);
                } while (this._unitCount.Count != 0);

                this._unitCount = null;
            }

            this.SetResourceCount(LogicDataTables.GetGoldData(), this._npcData.Gold);
            this.SetResourceCount(LogicDataTables.GetElixirData(), this._npcData.Elixir);

            if (this._allianceUnitCount.Count != 0)
            {
                this.ClearUnitSlotArray(this._allianceUnitCount);

                do
                {
                    this._allianceUnitCount[0].Destruct();
                    this._allianceUnitCount.Remove(0);
                } while (this._allianceUnitCount.Count != 0);

                this._allianceUnitCount = null;
            }

            if (this._unitCount.Count != 0)
            {
                this.ClearDataSlotArray(this._unitCount);

                do
                {
                    this._unitCount[0].Destruct();
                    this._unitCount.Remove(0);
                } while (this._unitCount.Count != 0);

                this._unitCount = null;
            }

            this._allianceUnitCount = new LogicArrayList<LogicUnitSlot>();
            this._unitCount = this._npcData.GetClonedUnits();
        }
    }
}