namespace RivieraStudio.Magic.Logic
{
    using RivieraStudio.Magic.Logic.Avatar;
    using RivieraStudio.Magic.Logic.Command.Battle;
    using RivieraStudio.Magic.Logic.Data;
    using RivieraStudio.Magic.Logic.GameObject;
    using RivieraStudio.Magic.Logic.Level;
    using RivieraStudio.Magic.Logic.Util;
    using RivieraStudio.Magic.Titan.Debug;
    using RivieraStudio.Magic.Titan.Util;

    public class LogicNpcAttack
    {
        private LogicLevel _level;
        private LogicNpcAvatar _npcAvatar;
        private LogicBuildingClassData _buildingClass;

        private bool _unitsDeployed;
        private int _placePositionX;
        private int _placePositionY;
        private int _nextUnit;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicNpcAttack"/> class.
        /// </summary>
        public LogicNpcAttack(LogicLevel level)
        {
            this._placePositionX = -1;
            this._placePositionY = -1;
            this._level = level;
            this._npcAvatar = (LogicNpcAvatar) level.GetVisitorAvatar();
            this._buildingClass = LogicDataTables.GetBuildingClassByName("Defense");

            if (this._buildingClass == null)
            {
                Debugger.Error("LogicNpcAttack - Unable to find Defense building class");
            }
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public void Destruct()
        {
            this._level = null;
            this._npcAvatar = null;
            this._buildingClass = null;
        }

        /// <summary>
        ///     Places one unit.
        /// </summary>
        public bool PlaceOneUnit()
        {
            if (this._placePositionX == -1 && this._placePositionY == -1)
            {
                int widthArea = this._level.GetPlayArea().GetStartX();
                int width = this._level.GetWidthInTiles();

                if (width > 0)
                {
                    int tileIdx = -1;
                    int tmp = width / 2;

                    for (int x = 0; x < width; x++)
                    {
                        if (widthArea >= 2)
                        {
                            int middleX = (widthArea - 1) / 2;
                            int square = (tmp - x) * (tmp - x);

                            for (int y = 0; y != widthArea - 1; y++, middleX--)
                            {
                                if (tileIdx == -1 || square + middleX * middleX < tileIdx)
                                {
                                    if (this._level.GetTileMap().GetTile(x, y).GetPassableFlag() == 1)
                                    {
                                        this._placePositionX = x;
                                        this._placePositionY = y;
                                        tileIdx = square + middleX * middleX;
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (this._placePositionX == -1 && this._placePositionY == -1)
            {
                Debugger.Error("LogicNpcAttack::placeOneUnit - No attack position found!");
            }
            else
            {
                LogicArrayList<LogicDataSlot> units = this._npcAvatar.GetUnits();

                for (int i = 0; i < units.Count; i++)
                {
                    LogicDataSlot slot = units[i];

                    if (slot.GetCount() > 0)
                    {
                        slot.SetCount(slot.GetCount() - 1);

                        LogicCharacter character = LogicPlaceAttackerCommand.PlaceAttacker(this._npcAvatar, (LogicCharacterData) slot.GetData(), this._level, this._placePositionX, this._placePositionY);

                        if (!this._unitsDeployed)
                        {
                            character.GetListener().MapUnlocked();
                        }
                        
                        // character.GetCombatComponent().SetPreferredTarget(this._buildingClass, 100, 0);
                        this._unitsDeployed = true;

                        return true;
                    }
                }
            }

            return false;
        }

        /// <summary>
        ///     Ticks this instance.
        /// </summary>
        public void Tick()
        {
            if (!this._unitsDeployed)
            {
                if (this._nextUnit <= 64)
                {
                    this._unitsDeployed = !this.PlaceOneUnit();
                    this._nextUnit = 200;
                }
                else
                {
                    this._nextUnit -= 64;
                }
            }
            else
            {
                this._level.GetGameMode().EndDefendState(); // DEBUG
            }
        }
    }
}