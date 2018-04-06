namespace RivieraStudio.Magic.Logic.Command.Home
{
    using RivieraStudio.Magic.Logic.Avatar;
    using RivieraStudio.Magic.Logic.Data;
    using RivieraStudio.Magic.Logic.Level;
    using RivieraStudio.Magic.Logic.Unit;
    using RivieraStudio.Magic.Titan.DataStream;

    public sealed class LogicBoostTrainingCommand : LogicCommand
    {
        private int _productionType;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicBoostTrainingCommand" /> class.
        /// </summary>
        public LogicBoostTrainingCommand()
        {
            // LogicBoostTrainingCommand.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicBoostTrainingCommand" /> class.
        /// </summary>
        public LogicBoostTrainingCommand(int productionType)
        {
            this._productionType = productionType;
        }

        /// <summary>
        ///     Decodes this instnace.
        /// </summary>
        public override void Decode(ByteStream stream)
        {
            this._productionType = stream.ReadInt();
            base.Decode(stream);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode(ChecksumEncoder encoder)
        {
            encoder.WriteInt(this._productionType);
            base.Encode(encoder);
        }

        /// <summary>
        ///     Gets the command type of this instance.
        /// </summary>
        public override int GetCommandType()
        {
            return 584;
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
            if (LogicDataTables.GetGlobals().UseNewTraining())
            {
                if (level.GetVillageType() == 0)
                {
                    LogicClientAvatar playerAvatar = level.GetPlayerAvatar();
                    LogicUnitProduction unitProduction = this._productionType == 1
                        ? level.GetGameObjectManager().GetSpellProduction() 
                        : level.GetGameObjectManager().GetUnitProduction();

                    if (unitProduction.CanBeBoosted())
                    {
                        int cost = unitProduction.GetBoostCost();

                        if (playerAvatar.HasEnoughDiamonds(cost, true, level))
                        {
                            playerAvatar.UseDiamonds(cost);
                            unitProduction.Boost();

                            return 0;
                        }

                        return -2;
                    }

                    return -1;
                }

                return -32;
            }

            return -99;
        }
    }
}