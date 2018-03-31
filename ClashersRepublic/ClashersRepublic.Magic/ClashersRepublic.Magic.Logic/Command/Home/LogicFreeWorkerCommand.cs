namespace ClashersRepublic.Magic.Logic.Command.Home
{
    using ClashersRepublic.Magic.Logic.Level;
    using ClashersRepublic.Magic.Titan.DataStream;

    public sealed class LogicFreeWorkerCommand : LogicCommand
    {
        private int _villageType;
        private LogicCommand _command;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicBuyResourcesCommand" /> class.
        /// </summary>
        public LogicFreeWorkerCommand()
        {
            // LogicBuyResourcesCommand.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicBuyResourcesCommand" /> class.
        /// </summary>
        public LogicFreeWorkerCommand(LogicCommand resourceCommand, int villageType)
        {
            this._command = resourceCommand;
            this._villageType = villageType;
        }

        /// <summary>
        ///     Decodes this instnace.
        /// </summary>
        public override void Decode(ByteStream stream)
        {
            base.Decode(stream);

            this._villageType = stream.ReadInt();

            if (stream.ReadBoolean())
            {
                this._command = LogicCommandManager.DecodeCommand(stream);
            }
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode(ChecksumEncoder encoder)
        {
            base.Encode(encoder);

            encoder.WriteInt(this._villageType);

            if (this._command != null)
            {
                encoder.WriteBoolean(true);
                LogicCommandManager.EncodeCommand(encoder, this._command);
            }
            else
            {
                encoder.WriteBoolean(false);
            }
        }

        /// <summary>
        ///     Gets the command type of this instance.
        /// </summary>
        public override int GetCommandType()
        {
            return 521;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();

            if (this._command != null)
            {
                this._command.Destruct();
                this._command = null;
            }
        }

        /// <summary>
        ///     Executes this command.
        /// </summary>
        public override int Execute(LogicLevel level)
        {
            int villageType = level.GetVillageType();

            if (this._villageType != -1)
            {
                villageType = this._villageType;
            }

            int freeWorkers = level.GetWorkerManagerAt(villageType).GetFreeWorkers();

            if (freeWorkers == 0)
            {
                if (level.GetWorkerManagerAt(villageType).FinishTaskOfOneWorker())
                {
                    if (this._command != null)
                    {
                        int commandType = this._command.GetCommandType();

                        if (commandType < 1000)
                        {
                            if (commandType >= 500 && commandType < 700)
                            {
                                this._command.Execute(level);
                            }
                        }
                    }

                    return 0;
                }
            }

            return -1;
        }
    }
}