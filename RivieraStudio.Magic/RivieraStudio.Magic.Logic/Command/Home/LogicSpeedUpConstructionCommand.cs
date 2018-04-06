namespace RivieraStudio.Magic.Logic.Command.Home
{
    using RivieraStudio.Magic.Logic.GameObject;
    using RivieraStudio.Magic.Logic.Level;

    using RivieraStudio.Magic.Titan.DataStream;

    public sealed class LogicSpeedUpConstructionCommand : LogicCommand
    {
        private int _gameObjectId;
        private int _villageType;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicSpeedUpConstructionCommand" /> class.
        /// </summary>
        public LogicSpeedUpConstructionCommand()
        {
            // LogicSpeedUpConstructionCommand.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicSpeedUpConstructionCommand" /> class.
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
                            return ((LogicTrap) gameObject).SpeedUpConstruction() ? 0 : -1;
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