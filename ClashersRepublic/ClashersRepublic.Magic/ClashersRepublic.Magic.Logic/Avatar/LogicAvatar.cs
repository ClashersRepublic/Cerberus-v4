namespace ClashersRepublic.Magic.Logic.Avatar
{
    using ClashersRepublic.Magic.Logic.Avatar.Change;
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Logic.Level;
    using ClashersRepublic.Magic.Logic.Util;
    using ClashersRepublic.Magic.Titan.Debug;
    using ClashersRepublic.Magic.Titan.Math;
    using ClashersRepublic.Magic.Titan.Util;

    public class LogicAvatar
    {
        protected LogicAvatarChangeListener _listener;

        protected int _townHallLevel;
        protected int _townHallLevelVillage2;
        protected int _allianceCastleLevel;
        protected int _allianceCastleTotalCapacity;
        protected int _allianceCastleUsedCapacity;
        protected int _allianceCastleTotalSpellCapacity;
        protected int _allianceCastleUsedSpellCapacity;
        protected int _allianceUnitVisitCapacity;
        protected int _allianceUnitSpellVisitCapacity;
        protected int _badgeId;
        protected int _leagueType;

        protected LogicLevel _level;

        protected LogicArrayList<LogicDataSlot> _resourceCount;
        protected LogicArrayList<LogicDataSlot> _resourceCap;
        protected LogicArrayList<LogicDataSlot> _unitCount;
        protected LogicArrayList<LogicDataSlot> _spellCount;
        protected LogicArrayList<LogicDataSlot> _unitUpgrade;
        protected LogicArrayList<LogicDataSlot> _spellUpgrade;
        protected LogicArrayList<LogicDataSlot> _heroUpgrade;
        protected LogicArrayList<LogicDataSlot> _heroHealth;
        protected LogicArrayList<LogicDataSlot> _heroState;
        protected LogicArrayList<LogicDataSlot> _heroMode;
        protected LogicArrayList<LogicDataSlot> _unitCountVillage2;
        protected LogicArrayList<LogicDataSlot> _unitCountNewVillage2;
        protected LogicArrayList<LogicDataSlot> _achievementProgress;
        protected LogicArrayList<LogicDataSlot> _lootedNpcGold;
        protected LogicArrayList<LogicDataSlot> _lootedNpcElixir;
        protected LogicArrayList<LogicDataSlot> _npcStars;
        protected LogicArrayList<LogicDataSlot> _variables;
        protected LogicArrayList<LogicDataSlot> _unitPreset1;
        protected LogicArrayList<LogicDataSlot> _unitPreset2;
        protected LogicArrayList<LogicDataSlot> _unitPreset3;
        protected LogicArrayList<LogicDataSlot> _previousArmy;
        protected LogicArrayList<LogicDataSlot> _eventUnitCounter;

        protected LogicArrayList<LogicUnitSlot> _allianceUnitCount;

        protected LogicArrayList<LogicData> _achievementRewardClaimed;
        protected LogicArrayList<LogicData> _missionCompleted;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicAvatar" /> class.
        /// </summary>
        public LogicAvatar()
        {
            this._allianceCastleLevel = -1;
        }

        /// <summary>
        ///     Inits the base of members.
        /// </summary>
        public virtual void InitBase()
        {
            this._listener = new LogicAvatarChangeListener();
            this._resourceCount = new LogicArrayList<LogicDataSlot>();
            this._resourceCap = new LogicArrayList<LogicDataSlot>();
            this._unitCount = new LogicArrayList<LogicDataSlot>();
            this._spellCount = new LogicArrayList<LogicDataSlot>();
            this._unitUpgrade = new LogicArrayList<LogicDataSlot>();
            this._spellUpgrade = new LogicArrayList<LogicDataSlot>();
            this._heroUpgrade = new LogicArrayList<LogicDataSlot>();
            this._heroHealth = new LogicArrayList<LogicDataSlot>();
            this._heroState = new LogicArrayList<LogicDataSlot>();
            this._heroMode = new LogicArrayList<LogicDataSlot>();
            this._unitCountVillage2 = new LogicArrayList<LogicDataSlot>();
            this._unitCountNewVillage2 = new LogicArrayList<LogicDataSlot>();
            this._achievementProgress = new LogicArrayList<LogicDataSlot>();
            this._lootedNpcGold = new LogicArrayList<LogicDataSlot>();
            this._lootedNpcElixir = new LogicArrayList<LogicDataSlot>();
            this._npcStars = new LogicArrayList<LogicDataSlot>();
            this._variables = new LogicArrayList<LogicDataSlot>();
            this._unitPreset1 = new LogicArrayList<LogicDataSlot>();
            this._unitPreset2 = new LogicArrayList<LogicDataSlot>();
            this._unitPreset3 = new LogicArrayList<LogicDataSlot>();
            this._previousArmy = new LogicArrayList<LogicDataSlot>();
            this._eventUnitCounter = new LogicArrayList<LogicDataSlot>();
            this._allianceUnitCount = new LogicArrayList<LogicUnitSlot>();
            this._achievementRewardClaimed = new LogicArrayList<LogicData>();
            this._missionCompleted = new LogicArrayList<LogicData>();
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public virtual void Destruct()
        {
            if (this._listener != null)
            {
                this._listener.Destruct();
                this._listener = null;
            }

            if (this._resourceCap != null)
            {
                this.ClearDataSlotArray(this._resourceCap);
                this._resourceCap = null;
            }

            if (this._unitCount != null)
            {
                this.ClearDataSlotArray(this._unitCount);
                this._unitCount = null;
            }

            if (this._spellCount != null)
            {
                this.ClearDataSlotArray(this._spellCount);
                this._spellCount = null;
            }

            if (this._unitUpgrade != null)
            {
                this.ClearDataSlotArray(this._unitUpgrade);
                this._unitUpgrade = null;
            }

            if (this._spellUpgrade != null)
            {
                this.ClearDataSlotArray(this._spellUpgrade);
                this._spellUpgrade = null;
            }

            if (this._heroUpgrade != null)
            {
                this.ClearDataSlotArray(this._heroUpgrade);
                this._heroUpgrade = null;
            }

            if (this._heroHealth != null)
            {
                this.ClearDataSlotArray(this._heroHealth);
                this._heroHealth = null;
            }

            if (this._heroState != null)
            {
                this.ClearDataSlotArray(this._heroState);
                this._heroState = null;
            }

            if (this._allianceUnitCount != null)
            {
                this.ClearUnitSlotArray(this._allianceUnitCount);
                this._allianceUnitCount = null;
            }

            if (this._achievementProgress != null)
            {
                this.ClearDataSlotArray(this._achievementProgress);
                this._achievementProgress = null;
            }

            if (this._npcStars != null)
            {
                this.ClearDataSlotArray(this._npcStars);
                this._npcStars = null;
            }

            if (this._lootedNpcGold != null)
            {
                this.ClearDataSlotArray(this._lootedNpcGold);
                this._lootedNpcGold = null;
            }

            if (this._lootedNpcElixir != null)
            {
                this.ClearDataSlotArray(this._lootedNpcElixir);
                this._lootedNpcElixir = null;
            }

            if (this._heroMode != null)
            {
                this.ClearDataSlotArray(this._heroMode);
                this._heroMode = null;
            }

            if (this._variables != null)
            {
                this.ClearDataSlotArray(this._variables);
                this._variables = null;
            }

            if (this._unitPreset1 != null)
            {
                this.ClearDataSlotArray(this._unitPreset1);
                this._unitPreset1 = null;
            }

            if (this._unitPreset2 != null)
            {
                this.ClearDataSlotArray(this._unitPreset2);
                this._unitPreset2 = null;
            }

            if (this._unitPreset3 != null)
            {
                this.ClearDataSlotArray(this._unitPreset3);
                this._unitPreset3 = null;
            }

            if (this._previousArmy != null)
            {
                this.ClearDataSlotArray(this._previousArmy);
                this._previousArmy = null;
            }

            if (this._eventUnitCounter != null)
            {
                this.ClearDataSlotArray(this._eventUnitCounter);
                this._eventUnitCounter = null;
            }

            if (this._unitCountVillage2 != null)
            {
                this.ClearDataSlotArray(this._unitCountVillage2);
                this._unitCountVillage2 = null;
            }

            if (this._unitCountNewVillage2 != null)
            {
                this.ClearDataSlotArray(this._unitCountNewVillage2);
                this._unitCountNewVillage2 = null;
            }
        }

        /// <summary>
        ///     Gets the <see cref="LogicAvatarChangeListener" /> instance.
        /// </summary>
        public LogicAvatarChangeListener GetChangeListener()
        {
            return this._listener;
        }

        /// <summary>
        ///     Sets the <see cref="LogicAvatarChangeListener" /> instance.
        /// </summary>
        public void SetChangeListener(LogicAvatarChangeListener listener)
        {
            this._listener = listener;
        }

        /// <summary>
        ///     Sets the level instance.
        /// </summary>
        public void SetLevel(LogicLevel level)
        {
            this._level = level;
        }

        /// <summary>
        ///     Gets a value indicating whether this avatar is a client avatar instance.
        /// </summary>
        public virtual bool IsClientAvatar()
        {
            return false;
        }

        /// <summary>
        ///     Gets a value indicating whether this avatar is a npc avatar instance.
        /// </summary>
        public virtual bool IsNpcAvatar()
        {
            return false;
        }

        /// <summary>
        ///     Gets the checksum of this instance.
        /// </summary>
        public virtual void GetChecksum(ChecksumHelper checksumHelper)
        {
            checksumHelper.StartObject("LogicAvatar");
            checksumHelper.StartArray("m_pResourceCount");

            for (int i = 0; i < this._resourceCount.Count; i++)
            {
                this._resourceCount[i].GetChecksum(checksumHelper);
            }

            checksumHelper.EndArray();
            checksumHelper.StartArray("m_pResourceCap");

            for (int i = 0; i < this._resourceCap.Count; i++)
            {
                this._resourceCap[i].GetChecksum(checksumHelper);
            }

            checksumHelper.EndArray();
            checksumHelper.StartArray("m_pUnitCount");

            for (int i = 0; i < this._unitCount.Count; i++)
            {
                this._unitCount[i].GetChecksum(checksumHelper);
            }

            checksumHelper.EndArray();
            checksumHelper.StartArray("m_pSpellCount");

            for (int i = 0; i < this._spellCount.Count; i++)
            {
                this._spellCount[i].GetChecksum(checksumHelper);
            }

            checksumHelper.EndArray();
            checksumHelper.StartArray("m_pAllianceUnitCount");

            for (int i = 0; i < this._allianceUnitCount.Count; i++)
            {
                this._allianceUnitCount[i].GetChecksum(checksumHelper);
            }

            checksumHelper.EndArray();
            checksumHelper.StartArray("m_pUnitUpgrade");

            for (int i = 0; i < this._unitUpgrade.Count; i++)
            {
                this._unitUpgrade[i].GetChecksum(checksumHelper);
            }

            checksumHelper.EndArray();
            checksumHelper.StartArray("m_pSpellUpgrade");

            for (int i = 0; i < this._spellUpgrade.Count; i++)
            {
                this._spellUpgrade[i].GetChecksum(checksumHelper);
            }

            checksumHelper.EndArray();
            checksumHelper.StartArray("m_pUnitCountVillage2");

            for (int i = 0; i < this._unitCountVillage2.Count; i++)
            {
                this._unitCountVillage2[i].GetChecksum(checksumHelper);
            }

            checksumHelper.EndArray();
            checksumHelper.WriteValue("m_townHallLevel", this._townHallLevel);
            checksumHelper.WriteValue("m_townHallLevelVillage2", this._townHallLevelVillage2);
            checksumHelper.EndObject();
        }

        /// <summary>
        ///     Helpers for change the count of commodity array.
        /// </summary>
        public void CommodityCountChangeHelper(int commodityType, LogicData data, int count)
        {
            switch (data.GetDataType())
            {
                case 2:
                {
                    if (commodityType == 0)
                    {
                        int resourceCount = this.GetResourceCount((LogicResourceData) data);
                        int newResourceCount = LogicMath.Max(resourceCount + count, 0);

                        if (newResourceCount > 0)
                        {
                            int resourceCap = this.GetResourceCap((LogicResourceData) data);

                            if (resourceCount < resourceCap)
                            {
                                if (newResourceCount > resourceCap)
                                {
                                    newResourceCount = resourceCap;
                                }

                                if (newResourceCount > resourceCount)
                                {
                                    this.SetResourceCount((LogicResourceData) data, count);
                                    this._listener.CommodityCountChanged(0, data, count);
                                }
                            }
                        }
                        else
                        {
                            this.SetResourceCount((LogicResourceData) data, count);
                            this._listener.CommodityCountChanged(0, data, count);
                        }
                    }
                    else if (commodityType == 1)
                    {
                        this.SetResourceCap((LogicResourceData) data, this.GetResourceCap((LogicResourceData) data) + count);
                    }

                    break;
                }
            }
        }

        /// <summary>
        ///     Clears the data slot array.
        /// </summary>
        public void ClearDataSlotArray(LogicArrayList<LogicDataSlot> dataSlotArray)
        {
            if (dataSlotArray.Count != 0)
            {
                do
                {
                    dataSlotArray[0].Destruct();
                    dataSlotArray.Remove(0);
                } while (dataSlotArray.Count != 0);
            }
        }

        /// <summary>
        ///     Clears the unit slot array.
        /// </summary>
        public void ClearUnitSlotArray(LogicArrayList<LogicUnitSlot> unitSlotArray)
        {
            if (unitSlotArray.Count != 0)
            {
                do
                {
                    unitSlotArray[0].Destruct();
                    unitSlotArray.Remove(0);
                } while (unitSlotArray.Count != 0);
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the player is in alliance.
        /// </summary>
        /// <returns></returns>
        public virtual bool IsInAlliance()
        {
            return false;
        }

        /// <summary>
        ///     Gets the alliance id.
        /// </summary>
        public virtual LogicLong GetAllianceId()
        {
            return 0;
        }

        /// <summary>
        ///     Gets the alliance badge.
        /// </summary>
        public virtual int GetAllianceBadge()
        {
            return 0;
        }

        /// <summary>
        ///     Gets the alliance role.
        /// </summary>
        public virtual int GetAllianceRole()
        {
            return 1;
        }

        /// <summary>
        ///     Gets the experience level.
        /// </summary>
        public virtual int GetExpLevel()
        {
            return 1;
        }

        /// <summary>
        ///     Gets the unused resource cap.
        /// </summary>
        public int GetUnusedResourceCap(LogicResourceData data)
        {
            return LogicMath.Max(this.GetResourceCap(data) - this.GetResourceCount(data), 0);
        }

        /// <summary>
        ///     Gets the resource cap.
        /// </summary>
        public int GetResourceCap(LogicResourceData data)
        {
            if (data.PremiumCurrency)
            {
                Debugger.Warning("LogicClientAvatar::getResourceCap shouldn't be used for diamonds");
            }
            else
            {
                int index = -1;

                for (int i = 0; i < this._resourceCap.Count; i++)
                {
                    if (this._resourceCount[i].GetData() == data)
                    {
                        index = i;
                        break;
                    }
                }

                if (index != -1)
                {
                    return this._resourceCap[index].GetCount();
                }
            }

            return 0;
        }

        /// <summary>
        ///     Sets the resource cap.
        /// </summary>
        public void SetResourceCap(LogicResourceData data, int count)
        {
            if (data.PremiumCurrency)
            {
                Debugger.Warning("LogicClientAvatar::setResourceCap shouldn't be used for diamonds");
            }
            else
            {
                int index = -1;

                for (int i = 0; i < this._resourceCap.Count; i++)
                {
                    if (this._resourceCount[i].GetData() == data)
                    {
                        index = i;
                        break;
                    }
                }

                if (index != -1)
                {
                    this._resourceCap[index].SetCount(count);
                }
                else
                {
                    this._resourceCap.Add(new LogicDataSlot(data, count));
                }
            }
        }

        /// <summary>
        ///     Gets the resource count.
        /// </summary>
        public virtual int GetResourceCount(LogicResourceData data)
        {
            if (!data.PremiumCurrency)
            {
                int index = -1;

                for (int i = 0; i < this._resourceCount.Count; i++)
                {
                    if (this._resourceCount[i].GetData() == data)
                    {
                        index = i;
                        break;
                    }
                }

                if (index != -1)
                {
                    return this._resourceCount[index].GetCount();
                }
            }
            else
            {
                Debugger.Warning("LogicAvatar::setResourceCount shouldn't be used for diamonds");
            }

            return 0;
        }

        /// <summary>
        ///     Sets the resource count.
        /// </summary>
        public void SetResourceCount(LogicResourceData data, int count)
        {
            if (!data.PremiumCurrency)
            {
                int index = -1;

                for (int i = 0; i < this._resourceCount.Count; i++)
                {
                    if (this._resourceCount[i].GetData() == data)
                    {
                        index = i;
                        break;
                    }
                }

                if (index != -1)
                {
                    this._resourceCount[index].SetCount(count);
                }
                else
                {
                    this._resourceCount.Add(new LogicDataSlot(data, count));
                }
            }
            else
            {
                Debugger.Warning("LogicAvatar::setResourceCount shouldn't be used for diamonds");
            }
        }

        /// <summary>
        ///     Gets the units total capacity.
        /// </summary>
        public int GetUnitsTotalCapacity()
        {
            int cnt = 0;

            for (int i = 0; i < this._unitCount.Count; i++)
            {
                cnt += ((LogicCombatItemData) this._unitCount[i].GetData()).HousingSpace * this._unitCount[i].GetCount();
            }

            return cnt;
        }

        /// <summary>
        ///     Gets the unit count.
        /// </summary>
        public int GetUnitCount(LogicCombatItemData data)
        {
            int index = -1;

            for (int i = 0; i < this._resourceCount.Count; i++)
            {
                if (this._unitCount[i].GetData() == data)
                {
                    index = i;
                    break;
                }
            }

            if (index != -1)
            {
                return this._unitCount[index].GetCount();
            }

            return 0;
        }

        /// <summary>
        ///     Sets the unit count.
        /// </summary>
        public void SetUnitCount(LogicCombatItemData data, int count)
        {
            int index = -1;

            for (int i = 0; i < this._resourceCount.Count; i++)
            {
                if (this._unitCount[i].GetData() == data)
                {
                    index = i;
                    break;
                }
            }

            if (index != -1)
            {
                this._unitCount[index].SetCount(count);
            }
            else
            {
                this._unitCount.Add(new LogicDataSlot(data, count));
            }
        }

        /// <summary>
        ///     Gets the unit upgrade level.
        /// </summary>
        public int GetUnitUpgradeLevel(LogicCombatItemData data)
        {
            if (data.GetCombatItemType() == 0)
            {
                int index = -1;

                for (int i = 0; i < this._unitUpgrade.Count; i++)
                {
                    if (this._unitUpgrade[i].GetData() == data)
                    {
                        index = i;
                        break;
                    }
                }

                if (index != -1)
                {
                    return this._unitUpgrade[index].GetCount();
                }
            }
            else
            {
                if (data.GetCombatItemType() == 1)
                {
                    int index = -1;

                    for (int i = 0; i < this._spellUpgrade.Count; i++)
                    {
                        if (this._spellUpgrade[i].GetData() == data)
                        {
                            index = i;
                            break;
                        }
                    }

                    if (index != -1)
                    {
                        return this._spellUpgrade[index].GetCount();
                    }
                }
                else
                {
                    int index = -1;

                    for (int i = 0; i < this._heroUpgrade.Count; i++)
                    {
                        if (this._heroUpgrade[i].GetData() == data)
                        {
                            index = i;
                            break;
                        }
                    }

                    if (index != -1)
                    {
                        return this._heroUpgrade[index].GetCount();
                    }
                }
            }

            return 0;
        }

        /// <summary>
        ///     Sets the unit upgrade level.
        /// </summary>
        public void SetUnitUpgradeLevel(LogicCombatItemData data, int count)
        {
            int combatItemType = data.GetCombatItemType();
            int upgradeCount = data.GetUpgradeLevelCount();

            if (upgradeCount <= count)
            {
                Debugger.Warning("LogicAvatar::setUnitUpgradeLevel - Level is out of bounds!");
                count = upgradeCount - 1;
            }

            if (combatItemType > 0)
            {
                if (combatItemType == 2)
                {
                    int index = -1;

                    for (int i = 0; i < this._heroUpgrade.Count; i++)
                    {
                        if (this._heroUpgrade[i].GetData() == data)
                        {
                            index = i;
                            break;
                        }
                    }

                    if (index != -1)
                    {
                        this._heroUpgrade[index].SetCount(count);
                    }
                    else
                    {
                        this._heroUpgrade.Add(new LogicDataSlot(data, count));
                    }
                }
                else
                {
                    int index = -1;

                    for (int i = 0; i < this._spellUpgrade.Count; i++)
                    {
                        if (this._spellUpgrade[i].GetData() == data)
                        {
                            index = i;
                            break;
                        }
                    }

                    if (index != -1)
                    {
                        this._spellUpgrade[index].SetCount(count);
                    }
                    else
                    {
                        this._spellUpgrade.Add(new LogicDataSlot(data, count));
                    }
                }
            }
            else
            {
                int index = -1;

                for (int i = 0; i < this._unitUpgrade.Count; i++)
                {
                    if (this._unitUpgrade[i].GetData() == data)
                    {
                        index = i;
                        break;
                    }
                }

                if (index != -1)
                {
                    this._unitUpgrade[index].SetCount(count);
                }
                else
                {
                    this._unitUpgrade.Add(new LogicDataSlot(data, count));
                }
            }
        }

        /// <summary>
        ///     Gets the npc stars.
        /// </summary>
        public int GetNpcStars(LogicNpcData data)
        {
            int index = -1;

            for (int i = 0; i < this._npcStars.Count; i++)
            {
                if (this._npcStars[i].GetData() == data)
                {
                    index = i;
                    break;
                }
            }

            if (index != -1)
            {
                return this._npcStars[index].GetCount();
            }

            return 0;
        }

        /// <summary>
        ///     Sets the npc stars.
        /// </summary>
        public void SetNpcStars(LogicNpcData data, int count)
        {
            int index = -1;

            for (int i = 0; i < this._npcStars.Count; i++)
            {
                if (this._npcStars[i].GetData() == data)
                {
                    index = i;
                    break;
                }
            }

            if (index != -1)
            {
                this._npcStars[index].SetCount(count);
            }
            else
            {
                this._npcStars.Add(new LogicDataSlot(data, count));
            }
        }

        /// <summary>
        ///     Gets the looted npc gold count.
        /// </summary>
        public int GetLootedNpcGold(LogicNpcData data)
        {
            int index = -1;

            for (int i = 0; i < this._lootedNpcGold.Count; i++)
            {
                if (this._lootedNpcGold[i].GetData() == data)
                {
                    index = i;
                    break;
                }
            }

            if (index != -1)
            {
                return this._lootedNpcGold[index].GetCount();
            }

            return 0;
        }

        /// <summary>
        ///     Sets the looted npc gold count.
        /// </summary>
        public void SetLootedNpcGold(LogicNpcData data, int count)
        {
            int index = -1;

            for (int i = 0; i < this._lootedNpcGold.Count; i++)
            {
                if (this._lootedNpcGold[i].GetData() == data)
                {
                    index = i;
                    break;
                }
            }

            if (index != -1)
            {
                this._lootedNpcGold[index].SetCount(count);
            }
            else
            {
                this._lootedNpcGold.Add(new LogicDataSlot(data, count));
            }
        }

        /// <summary>
        ///     Gets the looted npc elixir count.
        /// </summary>
        public int GetLootedNpcElixir(LogicNpcData data)
        {
            int index = -1;

            for (int i = 0; i < this._lootedNpcElixir.Count; i++)
            {
                if (this._lootedNpcElixir[i].GetData() == data)
                {
                    index = i;
                    break;
                }
            }

            if (index != -1)
            {
                return this._lootedNpcElixir[index].GetCount();
            }

            return 0;
        }

        /// <summary>
        ///     Sets the looted npc elixir count.
        /// </summary>
        public void SetLootedNpcElixir(LogicNpcData data, int count)
        {
            int index = -1;

            for (int i = 0; i < this._lootedNpcElixir.Count; i++)
            {
                if (this._lootedNpcElixir[i].GetData() == data)
                {
                    index = i;
                    break;
                }
            }

            if (index != -1)
            {
                this._lootedNpcElixir[index].SetCount(count);
            }
            else
            {
                this._lootedNpcElixir.Add(new LogicDataSlot(data, count));
            }
        }

        /// <summary>
        ///     Gets the all units.
        /// </summary>
        public LogicArrayList<LogicDataSlot> GetUnits()
        {
            return this._unitCount;
        }

        /// <summary>
        ///     Gets the all spells.
        /// </summary>
        public LogicArrayList<LogicDataSlot> GetSpells()
        {
            return this._spellCount;
        }

        /// <summary>
        ///     Gets the all resource caps.
        /// </summary>
        public LogicArrayList<LogicDataSlot> GetResourceCaps()
        {
            return this._resourceCap;
        }

        /// <summary>
        ///     Completes the specified mission.
        /// </summary>
        public void SetMissionCompleted(LogicMissionData data)
        {
            int index = -1;

            for (int i = 0; i < this._missionCompleted.Count; i++)
            {
                if (this._missionCompleted[i] == data)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
            {
                this._missionCompleted.Add(data);
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the player has the specified mission completed.
        /// </summary>
        public bool IsMissionCompleted(LogicMissionData data)
        {
            int index = -1;

            for (int i = 0; i < this._missionCompleted.Count; i++)
            {
                if (this._missionCompleted[i] == data)
                {
                    index = i;
                    break;
                }
            }

            return index != -1;
        }

        /// <summary>
        ///     Sets the value of specified achievement progress.
        /// </summary>
        public int GetAchievementProgress(LogicAchievementData data)
        {
            int index = -1;

            for (int i = 0; i < this._achievementProgress.Count; i++)
            {
                if (this._achievementProgress[i].GetData() == data)
                {
                    index = i;
                    break;
                }
            }

            if (index != -1)
            {
                return this._achievementProgress[index].GetCount();
            }

            return 0;
        }

        /// <summary>
        ///     Sets the value of specified achievement progress.
        /// </summary>
        public void SetAchievementProgress(LogicAchievementData data, int count)
        {
            int index = -1;

            for (int i = 0; i < this._achievementProgress.Count; i++)
            {
                if (this._achievementProgress[i].GetData() == data)
                {
                    index = i;
                    break;
                }
            }

            if (index != -1)
            {
                this._achievementProgress[index].SetCount(count);
            }
            else
            {
                this._achievementProgress.Add(new LogicDataSlot(data, count));
            }
        }

        /// <summary>
        ///     Sets the value indicating if the achievement is claimed.
        /// </summary>
        public void SetAchievementRewardClaimed(LogicAchievementData data)
        {
            int index = -1;

            for (int i = 0; i < this._achievementRewardClaimed.Count; i++)
            {
                if (this._achievementRewardClaimed[i] == data)
                {
                    index = i;
                    break;
                }
            }

            if (index == -1)
            {
                this._achievementRewardClaimed.Add(data);
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the player has the specified achievement claimed.
        /// </summary>
        public bool IsAchievementRewardClaimed(LogicAchievementData data)
        {
            int index = -1;

            for (int i = 0; i < this._achievementRewardClaimed.Count; i++)
            {
                if (this._achievementRewardClaimed[i] == data)
                {
                    index = i;
                    break;
                }
            }

            return index != -1;
        }

        /// <summary>
        ///     Gets a value indicating whether the player has the specified achievement claimed.
        /// </summary>
        public bool IsAchievementCompleted(LogicAchievementData data)
        {
            int index = -1;
            int progressCount = 0;

            for (int i = 0; i < this._achievementProgress.Count; i++)
            {
                if (this._achievementProgress[i].GetData() == data)
                {
                    index = i;
                    break;
                }
            }

            if (index != -1)
            {
                progressCount = this._achievementProgress[index].GetCount();
            }

            return progressCount >= data.ActionCount;
        }

        /// <summary>
        ///     Gets the town hall level.
        /// </summary>
        public int GetTownHallLevel()
        {
            return this._townHallLevel;
        }

        /// <summary>
        ///     Sets the town hall level.
        /// </summary>
        public void SetTownHallLevel(int level)
        {
            this._townHallLevel = level;
        }

        /// <summary>
        ///     Gets the town hall village 2 level.
        /// </summary>
        public int GetTownHallVillage2Level()
        {
            return this._townHallLevelVillage2;
        }

        /// <summary>
        ///     Sets the town hall village 2 level.
        /// </summary>
        public void SetTownHallVillage2Level(int level)
        {
            this._townHallLevelVillage2 = level;
        }

        /// <summary>
        ///     Gets the alliance castle level.
        /// </summary>
        public int GetAllianceCastleLevel()
        {
            return this._allianceCastleLevel;
        }

        /// <summary>
        ///     Sets the alliance castle level.
        /// </summary>
        public void SetAllianceCastleLevel(int level)
        {
            this._allianceCastleLevel = level;

            if (this._allianceCastleLevel == -1)
            {
                this._allianceCastleTotalCapacity = 0;
                this._allianceCastleTotalSpellCapacity = 0;
            }
            else
            {
                LogicBuildingData allianceCastleData = LogicDataTables.GetAllianceCastleData();

                this._allianceCastleTotalCapacity = allianceCastleData.GetUnitStorageCapacity(level);
                this._allianceCastleTotalSpellCapacity = allianceCastleData.GetAltUnitStorageCapacity(level);
            }
        }

        /// <summary>
        ///     Gets a value indicating whether the avatar have a alliance castle.
        /// </summary>
        public bool HasAllianceCastle()
        {
            return this._allianceCastleLevel != -1;
        }

        /// <summary>
        ///     Gets the total capacity of the alliance castle.
        /// </summary>
        public int GetAllianceCastleTotalCapacity()
        {
            return this._allianceCastleTotalCapacity;
        }

        /// <summary>
        ///     Gets the total spell capacity of the alliance castle.
        /// </summary>
        public int GetAllianceCastleTotalSpellCapacity()
        {
            return this._allianceCastleTotalSpellCapacity;
        }

        /// <summary>
        ///     Gets the total used capacity of the alliance castle.
        /// </summary>
        public int GetAllianceCastleUsedCapacity()
        {
            return this._allianceCastleTotalCapacity;
        }

        /// <summary>
        ///     Gets the used spell capacity of the alliance castle.
        /// </summary>
        public int GetAllianceCastleUsedSpellCapacity()
        {
            return this._allianceCastleUsedSpellCapacity;
        }

        /// <summary>
        ///     Gets the free capacity of alliance castle.
        /// </summary>
        public int GetAllianceCastleFreeCapacity()
        {
            return this._allianceCastleTotalCapacity - this._allianceCastleUsedCapacity;
        }

        /// <summary>
        ///     Gets the free spell capacity of alliance castle.
        /// </summary>
        public int GetAllianceCastleFreeSpellCapacity()
        {
            return this._allianceCastleTotalSpellCapacity - this._allianceCastleUsedSpellCapacity;
        }
    }
}