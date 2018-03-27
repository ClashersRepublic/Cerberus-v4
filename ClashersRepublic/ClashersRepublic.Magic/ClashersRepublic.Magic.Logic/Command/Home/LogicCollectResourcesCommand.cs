namespace ClashersRepublic.Magic.Logic.Command.Home
{
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.GameObject;
    using ClashersRepublic.Magic.Logic.GameObject.Component;
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Logic.Level;
    using ClashersRepublic.Magic.Logic.Mission;
    using ClashersRepublic.Magic.Titan.DataStream;
    using ClashersRepublic.Magic.Titan.Debug;
    using ClashersRepublic.Magic.Titan.Util;

    public sealed class LogicCollectResourcesCommand : LogicCommand
    {
        private int _gameObjectId;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicCollectResourcesCommand" /> class.
        /// </summary>
        public LogicCollectResourcesCommand()
        {
            // LogicCollectResourcesCommand.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicMissionData" /> class.
        /// </summary>
        public LogicCollectResourcesCommand(int gameObjectId)
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
            LogicGameObject gameObject = level.GetGameObjectManager().GetGameObjectByID(this._gameObjectId);

            if (gameObject != null)
            {
                if (gameObject.GetGameObjectType() == 0)
                {
                    if (gameObject.GetVillageType() == level.GetVillageType())
                    {
                        LogicResourceProductionComponent resourceProductionComponent = gameObject.GetResourceProductionComponent();

                        if (resourceProductionComponent != null)
                        {
                            if (LogicDataTables.GetGlobals().CollectAllResourcesAtOnce())
                            {
                                int baseAvailableResources = resourceProductionComponent.GetResourceCount();
                                int baseCollectedResources = resourceProductionComponent.CollectResources(true);

                                bool storageIsFull = baseAvailableResources > 0 && baseCollectedResources == 0;

                                Debugger.Log("Collected: " + baseCollectedResources);

                                LogicArrayList<LogicComponent> components = level.GetComponentManager().GetComponents(resourceProductionComponent.GetComponentType());

                                for (int i = 0; i < components.Count; i++)
                                {
                                    LogicResourceProductionComponent component = (LogicResourceProductionComponent) components[i];

                                    if (resourceProductionComponent != component && resourceProductionComponent.GetResourceData() == component.GetResourceData())
                                    {
                                        int availableResources = component.GetResourceCount();
                                        int collectedResources = component.CollectResources(!storageIsFull);

                                        if (availableResources > 0)
                                        {
                                            if (collectedResources == 0)
                                            {
                                                storageIsFull = true;
                                            }
                                        }
                                    }
                                }
                            }
                            else
                            {
                                resourceProductionComponent.CollectResources(true);
                            }

                            return 0;
                        }

                        return -1;
                    }

                    return -3;
                }
            }

            return -2;
        }
    }
}