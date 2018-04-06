namespace RivieraStudio.Magic.Logic.Command.Home
{
    using System;
    using RivieraStudio.Magic.Logic.Avatar;
    using RivieraStudio.Magic.Logic.Data;
    using RivieraStudio.Magic.Logic.GameObject;
    using RivieraStudio.Magic.Logic.GameObject.Component;
    using RivieraStudio.Magic.Logic.GameObject.Listener;
    using RivieraStudio.Magic.Logic.Helper;
    using RivieraStudio.Magic.Logic.Level;

    using RivieraStudio.Magic.Titan.DataStream;
    using RivieraStudio.Magic.Titan.Debug;
    using RivieraStudio.Magic.Titan.Math;
    using RivieraStudio.Magic.Titan.Util;

    public sealed class LogicRemoveUnitsCommand : LogicCommand
    {
        private LogicArrayList<int> _removeType;
        private LogicArrayList<int> _unitsUpgLevel;
        private LogicArrayList<int> _unitsCount;

        private LogicArrayList<LogicCombatItemData> _unitsData;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicRemoveUnitsCommand"/> class.
        /// </summary>
        public LogicRemoveUnitsCommand()
        {
            this._removeType = new LogicArrayList<int>();
            this._unitsUpgLevel = new LogicArrayList<int>();
            this._unitsCount = new LogicArrayList<int>();
            this._unitsData = new LogicArrayList<LogicCombatItemData>();
        }

        /// <summary>
        ///     Decodes this instnace.
        /// </summary>
        public override void Decode(ByteStream stream)
        {
            for (int i = 0, size = stream.ReadInt(); i < size; i++)
            {
                this._removeType.Add(stream.ReadInt());

                if (stream.ReadInt() != 0)
                {
                    this._unitsData.Add((LogicCombatItemData) stream.ReadDataReference(25));
                }
                else
                {
                    this._unitsData.Add((LogicCombatItemData) stream.ReadDataReference(3));
                }

                this._unitsCount.Add(stream.ReadInt());
                this._unitsUpgLevel.Add(stream.ReadInt());
            }

            base.Decode(stream);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode(ChecksumEncoder encoder)
        {
            base.Encode(encoder);
        }

        /// <summary>
        ///     Gets the command type of this instance.
        /// </summary>
        public override int GetCommandType()
        {
            return 550;
        }

        /// <summary>
        ///     Executes this instance.
        /// </summary>
        public override int Execute(LogicLevel level)
        {
            for (int i = 0; i < this._unitsCount.Count; i++)
            {
                if (this._unitsCount[i] < 0)
                {
                    return -1;
                }
            }

            if (LogicDataTables.GetGlobals().EnableTroopDeletion() && level.GetState() == 1 && this._unitsData.Count > 0)
            {
                LogicClientAvatar playerAvatar = level.GetPlayerAvatar();
                Int32 removedUnits = 0;

                for (int i = 0; i < this._unitsData.Count; i++)
                {
                    LogicCombatItemData data = this._unitsData[i];
                    Int32 unitCount = this._unitsCount[i];

                    if (this._removeType[i] != 0)
                    {
                        Int32 upgLevel = this._unitsUpgLevel[i];

                        if (data.GetCombatItemType() != 0)
                        {
                            if (data.GetCombatItemType() == 1)
                            {
                                playerAvatar.SetAllianceUnitCount(data, upgLevel, LogicMath.Max(0, playerAvatar.GetAllianceUnitCount(data, upgLevel) - unitCount));

                                if (unitCount > 0)
                                {
                                    do
                                    {
                                        playerAvatar.GetChangeListener().AllianceUnitRemoved(data, upgLevel);
                                    } while (--unitCount != 0);
                                }

                                removedUnits |= 2;
                            }
                        }
                        else
                        {
                            LogicBuilding allianceCastle = level.GetGameObjectManagerAt(0).GetAllianceCastle();

                            if (allianceCastle != null)
                            {
                                LogicBunkerComponent bunkerComponent = allianceCastle.GetBunkerComponent();
                                Int32 unitTypeIndex = bunkerComponent.GetUnitTypeIndex(data);

                                if (unitTypeIndex != -1)
                                {
                                    Int32 cnt = bunkerComponent.GetUnitCount(unitTypeIndex);

                                    if (cnt > 0)
                                    {
                                        bunkerComponent.RemoveUnits(data, upgLevel, cnt);
                                        playerAvatar.SetAllianceUnitCount(data, upgLevel, LogicMath.Max(0, playerAvatar.GetAllianceUnitCount(data, upgLevel) - unitCount));

                                        removedUnits |= 1;

                                        if (unitCount > 0)
                                        {
                                            do
                                            {
                                                playerAvatar.GetChangeListener().AllianceUnitRemoved(data, upgLevel);
                                            } while (--unitCount != 0);
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else
                    {
                        if (playerAvatar != null && data != null)
                        {
                            playerAvatar.CommodityCountChangeHelper(0, data, -unitCount);
                        }

                        LogicArrayList<LogicComponent> components = level.GetComponentManager().GetComponents(0);

                        for (int j = 0; j < components.Count; j++)
                        {
                            if (unitCount > 0)
                            {
                                LogicUnitStorageComponent storageComponent = (LogicUnitStorageComponent) components[i];
                                Int32 unitTypeIndex = storageComponent.GetUnitTypeIndex(data);

                                if (unitTypeIndex != -1)
                                {
                                    Int32 cnt = storageComponent.GetUnitCount(unitTypeIndex);

                                    if (cnt > 0)
                                    {
                                        cnt = LogicMath.Min(cnt, unitCount);
                                        storageComponent.RemoveUnits(data, cnt);

                                        int type = 2;

                                        if (storageComponent.GetUnitCount(unitTypeIndex) == 0)
                                        {
                                            if (storageComponent.GetParentListener() != null)
                                            {
                                                LogicGameObjectListener listener = storageComponent.GetParentListener();

                                                for (int k = 0; k < cnt; k++)
                                                {
                                                    listener.UnitRemoved(data);
                                                }
                                            }

                                            type = 1;
                                        }

                                        unitCount -= cnt;
                                        removedUnits |= type;
                                    }
                                }
                            }
                        }
                    }
                }

                switch (removedUnits)
                {
                    case 3:
                        if (LogicDataTables.GetGlobals().UseNewTraining())
                        {
                            level.GetGameObjectManager().GetUnitProduction().MergeSlots();
                            level.GetGameObjectManager().GetSpellProduction().MergeSlots();
                        }
                        break;
                    case 2:
                        if (LogicDataTables.GetGlobals().UseNewTraining())
                        {
                            level.GetGameObjectManager().GetSpellProduction().MergeSlots();
                        }
                        break;
                    case 1:
                        if (LogicDataTables.GetGlobals().UseNewTraining())
                        {
                            level.GetGameObjectManager().GetUnitProduction().MergeSlots();
                        }
                        break;
                    default:
                        Debugger.Print("WTF: " + removedUnits);
                        break;
                }
            }

            return 0;
        }
    }
}