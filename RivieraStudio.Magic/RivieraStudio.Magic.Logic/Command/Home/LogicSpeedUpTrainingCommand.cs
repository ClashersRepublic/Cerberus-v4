namespace RivieraStudio.Magic.Logic.Command.Home
{
    using System;
    using RivieraStudio.Magic.Logic.Avatar;
    using RivieraStudio.Magic.Logic.Data;
    using RivieraStudio.Magic.Logic.GameObject;
    using RivieraStudio.Magic.Logic.Level;
    using RivieraStudio.Magic.Logic.Unit;
    using RivieraStudio.Magic.Logic.Util;
    using RivieraStudio.Magic.Titan.DataStream;
    using RivieraStudio.Magic.Titan.Debug;

    public sealed class LogicSpeedUpTrainingCommand : LogicCommand
    {
        private int _gameObjectId;
        private bool _spellProduction;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicSpeedUpTrainingCommand" /> class.
        /// </summary>
        public LogicSpeedUpTrainingCommand()
        {
            // LogicSpeedUpTrainingCommand.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicSpeedUpTrainingCommand" /> class.
        /// </summary>
        public LogicSpeedUpTrainingCommand(int gameObjectId)
        {
            this._gameObjectId = gameObjectId;
        }

        /// <summary>
        ///     Decodes this instnace.
        /// </summary>
        public override void Decode(ByteStream stream)
        {
            this._gameObjectId = stream.ReadInt();
            this._spellProduction = stream.ReadBoolean();

            base.Decode(stream);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode(ChecksumEncoder encoder)
        {
            encoder.WriteInt(this._gameObjectId);
            encoder.WriteBoolean(this._spellProduction);

            base.Encode(encoder);
        }

        /// <summary>
        ///     Gets the command type of this instance.
        /// </summary>
        public override int GetCommandType()
        {
            return 513;
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
            if (level.GetVillageType() <= 1)
            {
                if (!LogicDataTables.GetGlobals().UseNewTraining())
                {
                    return -1; // TODO: Implement this.
                }

                return this.NewTrainingUnit(level);
            }

            return -32;
        }

        /// <summary>
        ///     Trains the unit with new training.
        /// </summary>
        public int NewTrainingUnit(LogicLevel level)
        {
            if (LogicDataTables.GetGlobals().UseNewTraining())
            {
                LogicUnitProduction unitProduction = this._spellProduction ? level.GetGameObjectManager().GetSpellProduction() : level.GetGameObjectManager().GetUnitProduction();
                LogicClientAvatar playerAvatar = level.GetPlayerAvatar();
                Int32 remainingSecs = unitProduction.GetTotalRemainingSeconds();
                Int32 speedUpCost = LogicGamePlayUtil.GetSpeedUpCost(remainingSecs, this._spellProduction ? 1 : 4, level.GetVillageType());

                if (!level.GetMissionManager().IsTutorialFinished())
                {
                    if (speedUpCost > 0 && LogicDataTables.GetGlobals().GetTutorialTrainingSpeedUpCost() > 0)
                    {
                        speedUpCost = LogicDataTables.GetGlobals().GetTutorialTrainingSpeedUpCost();
                    }
                }

                if (playerAvatar.HasEnoughDiamonds(speedUpCost, true, level))
                {
                    Debugger.Print("LogicSpeedUpTrainingCommand::newTrainingUnit cost: " + speedUpCost);

                    playerAvatar.UseDiamonds(speedUpCost);
                    unitProduction.SpeedUp();

                    return 0;
                }

                return -1;
            }

            return -99;
        }
    }
}