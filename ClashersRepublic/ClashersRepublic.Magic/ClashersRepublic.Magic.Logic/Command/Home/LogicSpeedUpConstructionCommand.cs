namespace ClashersRepublic.Magic.Logic.Command.Home
{
    using ClashersRepublic.Magic.Logic.GameObject;
    using ClashersRepublic.Magic.Logic.Level;

    using ClashersRepublic.Magic.Titan.DataStream;

    public sealed class LogicSpeedUpConstructionCommand : LogicCommand
    {
        private int _gameObjectId;
        private int _villageType;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicBuyBuildingCommand" /> class.
        /// </summary>
        public LogicSpeedUpConstructionCommand()
        {
            // LogicSpeedUpConstructionCommand.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicBuyBuildingCommand" /> class.
        /// </summary>
        public LogicSpeedUpConstructionCommand(int gameObjectId)
        {
            this._gameObjectId = gameObjectId;
        }

        /// <summary>
        ///     Decodes this instnace.
        /// </summary>
        public override void Decode(ByteStream stream)
        {
            this._gameObjectId = stream.ReadInt();
            this._villageType = stream.ReadInt();

            base.Decode(stream);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode(ChecksumEncoder encoder)
        {
            encoder.WriteInt(this._gameObjectId);
            encoder.WriteInt(this._villageType);

            base.Encode(encoder);
        }

        /// <summary>
        ///     Gets the command type of this instance.
        /// </summary>
        public override int GetCommandType()
        {
            return 506;
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
            if (this._villageType <= 1)
            {
                if (level.GetGameObjectManagerAt(this._villageType) != null)
                {
                    LogicGameObject gameObject = level.GetGameObjectManagerAt(this._villageType).GetGameObjectByID(this._gameObjectId);

                    if (gameObject != null)
                    {
                        if (gameObject.GetGameObjectType() == 0)
                        {
                            return ((LogicBuilding) gameObject).SpeedUpConstruction() ? 0 : -1;
                        }
                        if (gameObject.GetGameObjectType() == 4)
                        {
                            return -1;
                        }
                        if (gameObject.GetGameObjectType() == 8)
                        {
                            return -1;
                        }
                    }
                }

                return -3;
            }

            return -3;
        }
    }
}