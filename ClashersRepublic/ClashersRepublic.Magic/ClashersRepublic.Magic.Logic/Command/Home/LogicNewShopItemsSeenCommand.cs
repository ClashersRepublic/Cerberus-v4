namespace ClashersRepublic.Magic.Logic.Command.Home
{
    using ClashersRepublic.Magic.Logic.Avatar;
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.GameObject;
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Logic.Level;
    using ClashersRepublic.Magic.Titan.DataStream;

    public sealed class LogicNewShopItemsSeenCommand : LogicCommand
    {
        private int _newShopItemsType;
        private int _newShopItemsIndex;
        private int _newShopItemsCount;


        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicBuyBuildingCommand" /> class.
        /// </summary>
        public LogicNewShopItemsSeenCommand()
        {
            // LogicNewShopItemsSeenCommand.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicBuyBuildingCommand" /> class.
        /// </summary>
        public LogicNewShopItemsSeenCommand(int index, int type, int count)
        {
            this._newShopItemsIndex = index;
            this._newShopItemsType = type;
            this._newShopItemsCount = count;
        }

        /// <summary>
        ///     Decodes this instnace.
        /// </summary>
        public override void Decode(ByteStream stream)
        {
            this._newShopItemsIndex = stream.ReadInt();
            this._newShopItemsType = stream.ReadInt();
            this._newShopItemsCount = stream.ReadInt();

            base.Decode(stream);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode(ChecksumEncoder encoder)
        {
            encoder.WriteInt(this._newShopItemsIndex);
            encoder.WriteInt(this._newShopItemsType);
            encoder.WriteInt(this._newShopItemsCount);

            base.Encode(encoder);
        }

        /// <summary>
        ///     Gets the command type of this instance.
        /// </summary>
        public override int GetCommandType()
        {
            return 532;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();
        }

        /// <summary>
        ///     Executes this command.
        /// </summary>
        public override int Execute(LogicLevel level)
        {
            if (this._newShopItemsType <= 17)
            {
                if(level.SetUnlockedShopItemCount(LogicDataTables.GetTable(this._newShopItemsType).GetItemAt(this._newShopItemsIndex),
                                                  this._newShopItemsIndex,
                                                  this._newShopItemsCount,
                                                  level.GetVillageType()))
                {
                    return 0;
                }

                return -2;
            }

            return -1;
        }
    }
}