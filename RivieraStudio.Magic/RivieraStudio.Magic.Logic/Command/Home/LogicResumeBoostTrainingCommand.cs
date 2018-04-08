namespace RivieraStudio.Magic.Logic.Command.Home
{
    using RivieraStudio.Magic.Logic.Data;
    using RivieraStudio.Magic.Logic.GameObject;
    using RivieraStudio.Magic.Logic.Level;
    using RivieraStudio.Magic.Logic.Unit;
    using RivieraStudio.Magic.Titan.DataStream;
    using RivieraStudio.Magic.Titan.Util;

    public sealed class LogicResumeBoostTrainingCommand : LogicCommand
    {
        private int _gameObjectId;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicResumeBoostTrainingCommand" /> class.
        /// </summary>
        public LogicResumeBoostTrainingCommand()
        {
            // LogicResumeBoostTrainingCommand.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicResumeBoostTrainingCommand" /> class.
        /// </summary>
        public LogicResumeBoostTrainingCommand(int gameObjectId)
        {
            this._gameObjectId = gameObjectId;
        }

        /// <summary>
        ///     Decodes this instnace.
        /// </summary>
        public override void Decode(ByteStream stream)
        {
            this._gameObjectId = stream.ReadInt();
            base.Decode(stream);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode(ChecksumEncoder encoder)
        {
            encoder.WriteInt(this._gameObjectId);
            base.Encode(encoder);
        }

        /// <summary>
        ///     Gets the command type of this instance.
        /// </summary>
        public override int GetCommandType()
        {
            return 551;
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
                LogicUnitProduction unitProduction = this._gameObjectId == -2
                    ? level.GetGameObjectManager().GetUnitProduction()
                    : this._gameObjectId == -1
                        ? level.GetGameObjectManager().GetSpellProduction()
                        : null;

                if (unitProduction != null)
                {
                    unitProduction.SetBoostPause(false);
                    this.UpdateProductionHouseListeners(level);
                }
                
                return 0;
            }

            LogicGameObject gameObject = level.GetGameObjectManager().GetGameObjectByID(this._gameObjectId);

            if (gameObject != null &&
                gameObject.GetGameObjectType() == 0 &&
                gameObject.IsBoostPaused())
            {
                LogicBuilding building = (LogicBuilding) gameObject;

                if (building.CanStopBoost())
                {
                    building.SetBoostPause(false);
                    building.GetListener().RefreshState();

                    return 0;
                }
            }

            return -1;
        }

        /// <summary>
        ///     Updates the production house listeners.
        /// </summary>
        public void UpdateProductionHouseListeners(LogicLevel level)
        {
            LogicArrayList<LogicGameObject> gameObjects = level.GetGameObjectManager().GetGameObjects(0);

            for (int i = 0; i < gameObjects.Count; i++)
            {
                LogicBuilding building = (LogicBuilding) gameObjects[i];

                if (building.GetBuildingData().GetUnitProduction(0) > 0)
                {
                     building.GetListener().RefreshState();
                }
            }
        }
    }
}