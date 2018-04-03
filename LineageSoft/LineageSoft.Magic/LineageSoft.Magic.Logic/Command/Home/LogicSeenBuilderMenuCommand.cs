namespace LineageSoft.Magic.Logic.Command.Home
{
    using LineageSoft.Magic.Logic.Level;
    using LineageSoft.Magic.Titan.DataStream;

    public sealed class LogicSeenBuilderMenuCommand : LogicCommand
    {
        private int _villageType;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicSeenBuilderMenuCommand" /> class.
        /// </summary>
        public LogicSeenBuilderMenuCommand()
        {
            // LogicSeenBuilderMenuCommand.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicSeenBuilderMenuCommand" /> class.
        /// </summary>
        public LogicSeenBuilderMenuCommand(int villageType)
        {
            this._villageType = villageType;
        }

        /// <summary>
        ///     Decodes this instnace.
        /// </summary>
        public override void Decode(ByteStream stream)
        {
            this._villageType = stream.ReadInt();
            base.Decode(stream);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode(ChecksumEncoder encoder)
        {
            encoder.WriteInt(this._villageType);
            base.Encode(encoder);
        }

        /// <summary>
        ///     Gets the command type of this instance.
        /// </summary>
        public override int GetCommandType()
        {
            return 604;
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
            if (this._villageType == 0)
            {
                level.GetPlayerAvatar().SetVariableByName("SeenBuilderMenu", 1);
                return 0;
            }

            return -1;
        }
    }
}