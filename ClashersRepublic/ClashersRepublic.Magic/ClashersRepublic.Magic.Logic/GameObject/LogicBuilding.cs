namespace ClashersRepublic.Magic.Logic.GameObject
{
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.GameObject.Component;
    using ClashersRepublic.Magic.Logic.Level;
    using ClashersRepublic.Magic.Logic.Time;
    using ClashersRepublic.Magic.Titan.Math;

    public sealed class LogicBuilding : LogicGameObject
    {
        private int _upgLevel;
        private bool _locked;

        private LogicTimer _constructionTimer;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicBuilding" /> class.
        /// </summary>
        public LogicBuilding(LogicData data, LogicLevel level, int villageType) : base(data, level, villageType)
        {
            // LogicBuilding.
        }

        /// <summary>
        ///     Gets the <see cref="LogicBuildingData" /> instance.
        /// </summary>
        public LogicBuildingData GetBuildingData()
        {
            return (LogicBuildingData) this._data;
        }

        /// <summary>
        ///     Sets the building upgrade level.
        /// </summary>
        public void SetUpgradeLevel(int level)
        {
            LogicBuildingData buildingData = (LogicBuildingData) this._data;

            this._upgLevel = LogicMath.Clamp(level, 0, buildingData.GetUpgradeLevelCount() - 1);

            if (this._level.GetHomeOwnerAvatar() != null)
            {
                if (buildingData.IsAllianceCastle())
                {
                    if (!this._locked)
                    {
                        this._level.GetHomeOwnerAvatar().SetAllianceCastleLevel(this._upgLevel);
                    }
                }
                else if (buildingData.IsTownHall())
                {
                    this._level.GetHomeOwnerAvatar().SetTownHallLevel(this._upgLevel);
                }
                else if (buildingData.IsTownHall2())
                {
                    this._level.GetHomeOwnerAvatar().SetTownHallVillage2Level(this._upgLevel);
                }
            }

            if (this._upgLevel != 0 || this._constructionTimer == null)
            {
                if (this.GetHitpointComponent() != null)
                {
                    LogicHitpointComponent hitpointComponent = this.GetHitpointComponent();

                    if (this._locked)
                    {
                        hitpointComponent.SetMaxHitpoints(0);
                        hitpointComponent.SetHitpoints(0);
                        hitpointComponent.SetMaxRegenerationTime(100);
                    }
                    else
                    {
                        hitpointComponent.SetMaxHitpoints(buildingData.GetHitpoints(this._upgLevel));
                        hitpointComponent.SetHitpoints(buildingData.GetHitpoints(this._upgLevel));
                        hitpointComponent.SetMaxRegenerationTime(buildingData.GetRegenerationTime(this._upgLevel));
                    }
                }
            }

            if (this.GetComponent(5, true) != null)
            {
            }
        }
    }
}