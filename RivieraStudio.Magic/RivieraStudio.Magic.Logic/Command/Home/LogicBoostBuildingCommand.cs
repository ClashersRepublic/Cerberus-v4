namespace RivieraStudio.Magic.Logic.Command.Home
{
    using RivieraStudio.Magic.Logic.Avatar;
    using RivieraStudio.Magic.Logic.Data;
    using RivieraStudio.Magic.Logic.GameObject;
    using RivieraStudio.Magic.Logic.Level;
    using RivieraStudio.Magic.Titan.DataStream;
    using RivieraStudio.Magic.Titan.Util;

    public sealed class LogicBoostBuildingCommand : LogicCommand
    {
        private LogicArrayList<int> _gameObjectIds;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicBoostBuildingCommand" /> class.
        /// </summary>
        public LogicBoostBuildingCommand()
        {
            this._gameObjectIds = new LogicArrayList<int>();
        }
        
        /// <summary>
        ///     Decodes this instnace.
        /// </summary>
        public override void Decode(ByteStream stream)
        {
            for (int i = 0, size = stream.ReadInt(); i < size; i++)
            {
                this._gameObjectIds.Add(stream.ReadInt());
            }

            base.Decode(stream);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode(ChecksumEncoder encoder)
        {
            encoder.WriteInt(this._gameObjectIds.Count);

            for (int i = 0; i < this._gameObjectIds.Count; i++)
            {
                encoder.WriteInt(this._gameObjectIds[i]);
            }

            base.Encode(encoder);
        }

        /// <summary>
        ///     Gets the command type of this instance.
        /// </summary>
        public override int GetCommandType()
        {
            return 526;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();
            this._gameObjectIds = null;
        }

        /// <summary>
        ///     Executes this command.
        /// </summary>
        public override int Execute(LogicLevel level)
        {
            if (this._gameObjectIds.Count > 0)
            {
                int cost = 0;

                for (int i = 0; i < this._gameObjectIds.Count; i++)
                {
                    LogicGameObject gameObject = level.GetGameObjectManager().GetGameObjectByID(this._gameObjectIds[i]);

                    if (gameObject != null && gameObject.GetGameObjectType() == 0)
                    {
                        if (gameObject.GetData().GetVillageType() == level.GetVillageType())
                        {
                            LogicBuilding building = (LogicBuilding) gameObject;

                            if (!building.IsLocked())
                            {
                                if (!LogicDataTables.GetGlobals().UseNewTraining() || building.GetUnitProductionComponent() == null)
                                {
                                    if (building.CanBeBoosted())
                                    {
                                        cost += building.GetBoostCost();
                                    }

                                    continue;
                                }

                                return -3;
                            }

                            return -4;
                        }

                        return -32;
                    }

                    return -5;
                }

                LogicClientAvatar playerAvatar = level.GetPlayerAvatar();

                if (cost > 0)
                {
                    if (!playerAvatar.HasEnoughDiamonds(cost, true, level))
                    {
                        return -2;
                    }

                    playerAvatar.UseDiamonds(cost);
                }

                for (int i = 0; i < this._gameObjectIds.Count; i++)
                {
                    LogicGameObject gameObject = level.GetGameObjectManager().GetGameObjectByID(this._gameObjectIds[i]);

                    if (gameObject != null && gameObject.GetGameObjectType() == 0)
                    {
                        LogicBuilding building = (LogicBuilding) gameObject;

                        if (building.GetMaxBoostTime() != 0)
                        {
                            building.Boost();
                        }
                    }
                }

                return 0;
            }

            return -1;
        }
    }
}