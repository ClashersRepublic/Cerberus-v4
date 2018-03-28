namespace ClashersRepublic.Magic.Logic.Avatar
{
    using ClashersRepublic.Magic.Logic.Command;
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Logic.League.Entry;
    using ClashersRepublic.Magic.Logic.Level;
    using ClashersRepublic.Magic.Logic.Util;
    using ClashersRepublic.Magic.Titan.DataStream;
    using ClashersRepublic.Magic.Titan.Debug;
    using ClashersRepublic.Magic.Titan.Json;
    using ClashersRepublic.Magic.Titan.Math;
    using ClashersRepublic.Magic.Titan.Util;

    public sealed class LogicClientAvatar : LogicAvatar
    {
        private LogicLong _id;
        private LogicLong _currentHomeId;
        private LogicLong _allianceId;
        private LogicLong _leagueInstanceId;

        private LogicLegendLeagueTournamentEntry _legendLeagueTournamentEntry;
        private LogicLegendLeagueTournamentEntry _legendLeagueTournamentVillage2Entry;

        private bool _nameSetByUser;

        private int _allianceBadgeId;
        private int _allianceRole;
        private int _allianceExpLevel;
        private int _legendaryScore;
        private int _legendaryScoreVillage2;
        private int _expLevel;
        private int _expPoints;
        private int _diamonds;
        private int _freeDiamonds;
        private int _score;
        private int _duelScore;
        private int _warPreference;
        private int _attackRating;
        private int _attackKFactor;
        private int _attackWinCount;
        private int _attackLoseCount;
        private int _defenseWinCount;
        private int _defenseLoseCount;
        private int _treasuryGoldCount;
        private int _treasuryElixirCount;
        private int _treasuryDarkElixirCount;
        private int _redPackageState;
        private int _nameChangeState;

        private string _facebookId;
        private string _allianceName;
        private string _name;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicClientAvatar" /> class.
        /// </summary>
        public LogicClientAvatar()
        {
            this._legendLeagueTournamentEntry = new LogicLegendLeagueTournamentEntry();
            this._legendLeagueTournamentVillage2Entry = new LogicLegendLeagueTournamentEntry();

            this._expLevel = 1;
            this._badgeId = -1;
            this._nameChangeState = -1;
            this._attackRating = 1200;
            this._attackKFactor = 60;
            this._warPreference = 1;

            this.InitBase();
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();

            if (this._legendLeagueTournamentEntry != null)
            {
                this._legendLeagueTournamentEntry.Destruct();
                this._legendLeagueTournamentEntry = null;
            }

            if (this._legendLeagueTournamentVillage2Entry != null)
            {
                this._legendLeagueTournamentVillage2Entry.Destruct();
                this._legendLeagueTournamentVillage2Entry = null;
            }

            this._id = null;
            this._currentHomeId = null;
            this._leagueInstanceId = null;
            this._allianceId = null;
            this._allianceName = null;
            this._facebookId = null;
            this._name = null;
        }

        /// <summary>
        ///     Inits the base of members.
        /// </summary>
        public override void InitBase()
        {
            base.InitBase();

            this._name = string.Empty;

            this._id = new LogicLong();
            this._currentHomeId = new LogicLong();
        }

        /// <summary>
        ///     Gets the checksum of this instance.
        /// </summary>
        public override void GetChecksum(ChecksumHelper checksumHelper)
        {
            checksumHelper.StartObject("LogicClientAvatar");
            base.GetChecksum(checksumHelper);
            checksumHelper.WriteValue("m_expPoints", this._expPoints);
            checksumHelper.WriteValue("m_expLevel", this._expLevel);
            checksumHelper.WriteValue("m_diamonds", this._diamonds);
            checksumHelper.WriteValue("m_freeDiamonds", this._freeDiamonds);
            checksumHelper.WriteValue("m_score", this._score);
            checksumHelper.WriteValue("m_duelScore", this._duelScore);

            if (this._allianceId != null)
            {
                checksumHelper.WriteValue("isInAlliance", 13);
            }

            checksumHelper.EndObject();
        }

        /// <summary>
        ///     Gets the default avatar.
        /// </summary>
        public static LogicClientAvatar GetDefaultAvatar()
        {
            LogicClientAvatar defaultAvatar = new LogicClientAvatar();
            LogicGlobals globalsInstance = LogicDataTables.GetGlobals();

            defaultAvatar._diamonds = globalsInstance.GetStartingDiamonds();
            defaultAvatar._freeDiamonds = globalsInstance.GetStartingDiamonds();

            defaultAvatar.SetResourceCount(LogicDataTables.GetGoldData(), globalsInstance.GetStartingGold());
            defaultAvatar.SetResourceCount(LogicDataTables.GetGold2Data(), globalsInstance.GetStartingGold2());
            defaultAvatar.SetResourceCount(LogicDataTables.GetElixirData(), globalsInstance.GetStartingElixir());
            defaultAvatar.SetResourceCount(LogicDataTables.GetElixir2Data(), globalsInstance.GetStartingElixir2());

            return defaultAvatar;
        }

        /// <summary>
        ///     Gets a value indicating whether this avatar is a client avatar instance.
        /// </summary>
        public override bool IsClientAvatar()
        {
            return true;
        }

        /// <summary>
        ///     Gets the alliance id.
        /// </summary>
        public override LogicLong GetAllianceId()
        {
            return this._allianceId;
        }

        /// <summary>
        ///     Sets the alliance id.
        /// </summary>
        public void SetAllianceId(LogicLong value)
        {
            this._allianceId = value;
        }

        /// <summary>
        ///     Gets the alliance name.
        /// </summary>
        public string GetAllianceName()
        {
            return this._allianceName;
        }

        /// <summary>
        ///     Sets the alliance name.
        /// </summary>
        public void SetAllianceName(string value)
        {
            this._allianceName = value;
        }

        /// <summary>
        ///     Gets the alliance role.
        /// </summary>
        public override int GetAllianceRole()
        {
            return this._allianceRole;
        }

        /// <summary>
        ///     Sets the alliance role.
        /// </summary>
        public void SetAllianceRole(int value)
        {
            this._allianceRole = value;
        }

        /// <summary>
        ///     Gets the alliance badge.
        /// </summary>
        /// <returns></returns>
        public override int GetAllianceBadge()
        {
            return this._allianceBadgeId;
        }

        /// <summary>
        ///     Sets the alliance badge.
        /// </summary>
        public void SetAllianceBadge(int value)
        {
            this._allianceBadgeId = value;
        }

        /// <summary>
        ///     Gets the alliance exp level.
        /// </summary>
        /// <returns></returns>
        public int GetAllianceExpLevel()
        {
            return this._allianceExpLevel;
        }

        /// <summary>
        ///     Sets the alliance exp level.
        /// </summary>
        public void SetAllianceExpLevel(int value)
        {
            this._allianceExpLevel = value;
        }

        /// <summary>
        ///     Gets a value indicating whether the avatar is in alliance.
        /// </summary>
        public override bool IsInAlliance()
        {
            return this._allianceId != null;
        }

        /// <summary>
        ///     Gets the experience level.
        /// </summary>
        public override int GetExpLevel()
        {
            return this._expLevel;
        }

        /// <summary>
        ///     Gets the experience level.
        /// </summary>
        public void SetExpLevel(int expLevel)
        {
            this._expLevel = expLevel;
        }

        /// <summary>
        ///     Gets the id.
        /// </summary>
        public LogicLong GetId()
        {
            return this._id;
        }

        /// <summary>
        ///     Sets the avatar id.
        /// </summary>
        public void SetId(LogicLong id)
        {
            this._id = id;
        }

        /// <summary>
        ///     Gets the current home id.
        /// </summary>
        public LogicLong GetCurrentHomeId()
        {
            return this._currentHomeId;
        }

        /// <summary>
        ///     Sets the current home id.
        /// </summary>
        public void SetCurrentHomeId(LogicLong id)
        {
            this._currentHomeId = id;
        }

        /// <summary>
        ///     Gets if the name has been set by user.
        /// </summary>
        public bool GetNameSetByUser()
        {
            return this._nameSetByUser;
        }

        /// <summary>
        ///     Sets if the name has been set by user.
        /// </summary>
        public void SetNameSetByUser(bool set)
        {
            this._nameSetByUser = set;
        }

        /// <summary>
        ///     Gets the name change state.
        /// </summary>
        public int GetNameChangeState()
        {
            return this._nameChangeState;
        }

        /// <summary>
        ///     Sets the name change state.
        /// </summary>
        public void SetNameChangeState(int state)
        {
            this._nameChangeState = state;
        }

        /// <summary>
        ///     Gets the name.
        /// </summary>
        public string GetName()
        {
            return this._name;
        }

        /// <summary>
        ///     Sets the name.
        /// </summary>
        public void SetName(string name)
        {
            this._name = name;
        }

        /// <summary>
        ///     Gets the facebook id.
        /// </summary>
        public string GetFacebookId()
        {
            return this._facebookId;
        }

        /// <summary>
        ///     Sets the facebook id.
        /// </summary>
        public void SetFacebookId(string facebookId)
        {
            this._facebookId = facebookId;
        }

        /// <summary>
        ///     Gets a value indicating whether the avatar has enough diamonds.
        /// </summary>
        public bool HasEnoughDiamonds(int count, bool callListener, LogicLevel level)
        {
            bool enough = this._diamonds >= count;

            if (!enough)
            {
                level.GetGameListener().NotEnoughDiamonds();
            }

            return enough;
        }

        /// <summary>
        ///     Gets a value indicating whether the avatar has enough resources.
        /// </summary>
        public bool HasEnoughResources(LogicResourceData data, int count, bool callListener, LogicCommand command, bool unk)
        {
            bool enough = this.GetResourceCount(data) >= count;

            if (!enough)
            {
                if (this._level != null)
                {
                    this._level.GetGameListener().NotEnoughResources(data, count, command, unk);
                }
            }

            return enough;
        }

        /// <summary>
        ///     Gets the diamonds count.
        /// </summary>
        public int GetDiamonds()
        {
            return this._diamonds;
        }

        /// <summary>
        ///     Sets the diamonds count.
        /// </summary>
        public void SetDiamonds(int count)
        {
            this._diamonds = count;
        }

        /// <summary>
        ///     Uses the number of specified diamonds.
        /// </summary>
        public void UseDiamonds(int count)
        {
            int currentDiamonds = this._diamonds;
            int newDiamonds = this._diamonds -= count;

            if (currentDiamonds > newDiamonds)
            {
                this._freeDiamonds = newDiamonds;
            }
        }

        /// <summary>
        ///     Gets the free diamonds count.
        /// </summary>
        public int GetFreeDiamonds()
        {
            return this._freeDiamonds;
        }

        /// <summary>
        ///     Sets the free diamonds count.
        /// </summary>
        public void SetFreeDiamonds(int count)
        {
            this._freeDiamonds = count;
        }

        /// <summary>
        ///     Gets the league type.
        /// </summary>
        public int GetLeagueType()
        {
            return LogicMath.Clamp(this._leagueType, 0, LogicDataTables.GetTable(28).GetItemCount() - 1);
        }

        /// <summary>
        ///     Gets the score.
        /// </summary>
        public int GetScore()
        {
            return this._score;
        }

        /// <summary>
        ///     Gets the duel score.
        /// </summary>
        public int GetDuelScore()
        {
            return this._duelScore;
        }

        /// <summary>
        ///     Sets the score.
        /// </summary>
        public void SetScore(int value)
        {
            this._score = value;
        }

        /// <summary>
        ///     Gets the resource count.
        /// </summary>
        public override int GetResourceCount(LogicResourceData data)
        {
            if (data.PremiumCurrency)
            {
                return this._diamonds;
            }

            return base.GetResourceCount(data);
        }
        
        /// <summary>
        ///     Gets the league type data instance.
        /// </summary>
        public LogicLeagueData GetLeagueTypeData()
        {
            LogicDataTable table = LogicDataTables.GetTable(28);
            Debugger.DoAssert(this._leagueType > -1 && table.GetItemCount() > this._leagueType, "Player league ranking out of bounds");
            return (LogicLeagueData) table.GetItemAt(this._leagueType);
        }

        /// <summary>
        ///     Helper for exp gain.
        /// </summary>
        public override void XpGainHelper(int count)
        {
            if (count > 0)
            {
                int maxExpPoints = LogicDataTables.GetExperienceLevel(this._expLevel).GetMaxExpPoints();

                if (this._expLevel < LogicExperienceLevelData.GetLevelCap())
                {
                    int gainExpPoints = this._expPoints + count;

                    if (gainExpPoints >= maxExpPoints)
                    {
                        if (this._expLevel + 1 == LogicExperienceLevelData.GetLevelCap())
                        {
                            gainExpPoints = maxExpPoints;
                        }

                        gainExpPoints -= maxExpPoints;

                        this._expLevel += 1;
                        this._listener.ExpLevelGained(this._expLevel);

                        if (this._level != null)
                        {
                            if (this._level.GetPlayerAvatar() == this)
                            {
                                this._level.GetGameListener().LevelUp(this._expLevel);
                            }

                            if (this._level.GetHomeOwnerAvatar() == this)
                            {
                                this._level.RefreshNewShopUnlocksExp();
                            }
                        }
                    }
                    else
                    {
                        this._listener.ExpPointsGained(gainExpPoints);
                    }

                    this._expPoints = gainExpPoints;
                }
            }
        }

        /// <summary>
        ///     Adds the mission resource reward.
        /// </summary>
        public void AddMisisonResourceReward(LogicResourceData resourceData, int count)
        {
            if (resourceData != null)
            {
                if (count > 0)
                {
                    this.SetResourceCount(resourceData, this.GetResourceCount(resourceData) + count);
                    this._listener.CommodityCountChanged(0, resourceData, count);
                }
            }
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public void Decode(ByteStream stream)
        {
            this._id = stream.ReadLong();
            this._currentHomeId = stream.ReadLong();

            if (stream.ReadBoolean())
            {
                this._allianceId = stream.ReadLong();
                this._allianceName = stream.ReadString(900000);
                this._allianceBadgeId = stream.ReadInt();
                this._allianceRole = stream.ReadInt();
                this._allianceExpLevel = stream.ReadInt();
            }

            if (stream.ReadBoolean())
            {
                this._leagueInstanceId = stream.ReadLong();
            }

            this._legendaryScore = stream.ReadInt();
            this._legendaryScoreVillage2 = stream.ReadInt();
            this._legendLeagueTournamentEntry.Decode(stream);
            this._legendLeagueTournamentVillage2Entry.Decode(stream);

            stream.ReadInt();
            stream.ReadInt();
            stream.ReadInt();

            this._leagueType = stream.ReadInt();
            this._allianceCastleLevel = stream.ReadInt();
            this._allianceCastleUsedCapacity = stream.ReadInt();
            this._allianceCastleTotalCapacity = stream.ReadInt();
            this._allianceCastleUsedSpellCapacity = stream.ReadInt();
            this._allianceCastleTotalSpellCapacity = stream.ReadInt();

            this._townHallLevel = stream.ReadInt();
            this._townHallLevelVillage2 = stream.ReadInt();

            this._name = stream.ReadString(900000);
            this._facebookId = stream.ReadString(900000);

            this._expLevel = stream.ReadInt();
            this._expPoints = stream.ReadInt();
            this._diamonds = stream.ReadInt();
            this._freeDiamonds = stream.ReadInt();
            this._attackRating = stream.ReadInt();
            this._attackKFactor = stream.ReadInt();
            this._score = stream.ReadInt();
            this._duelScore = stream.ReadInt();
            this._attackWinCount = stream.ReadInt();
            this._attackLoseCount = stream.ReadInt();
            this._defenseWinCount = stream.ReadInt();
            this._defenseLoseCount = stream.ReadInt();
            this._treasuryGoldCount = stream.ReadInt();
            this._treasuryElixirCount = stream.ReadInt();
            this._treasuryDarkElixirCount = stream.ReadInt();

            stream.ReadInt();

            if (stream.ReadBoolean())
            {
                stream.ReadLong();
            }

            this._nameSetByUser = stream.ReadBoolean();
            this._nameChangeState = stream.ReadInt();

            stream.ReadInt();

            this._redPackageState = stream.ReadInt();
            this._warPreference = stream.ReadInt();

            stream.ReadInt();
            stream.ReadInt();

            if (stream.ReadBoolean())
            {
                stream.ReadInt();
                stream.ReadLong();
            }

            this.ClearDataSlotArray(this._resourceCap);
            this.ClearDataSlotArray(this._resourceCount);
            this.ClearDataSlotArray(this._unitCount);
            this.ClearDataSlotArray(this._spellCount);
            this.ClearDataSlotArray(this._unitUpgrade);
            this.ClearDataSlotArray(this._spellUpgrade);
            this.ClearDataSlotArray(this._heroUpgrade);
            this.ClearDataSlotArray(this._heroHealth);
            this.ClearDataSlotArray(this._heroState);
            this.ClearUnitSlotArray(this._allianceUnitCount);
            this.ClearDataSlotArray(this._achievementProgress);
            this.ClearDataSlotArray(this._npcStars);
            this.ClearDataSlotArray(this._lootedNpcGold);
            this.ClearDataSlotArray(this._lootedNpcElixir);
            this.ClearDataSlotArray(this._heroMode);
            this.ClearDataSlotArray(this._variables);
            this.ClearDataSlotArray(this._unitPreset1);
            this.ClearDataSlotArray(this._unitPreset2);
            this.ClearDataSlotArray(this._unitPreset3);
            this.ClearDataSlotArray(this._previousArmy);
            this.ClearDataSlotArray(this._eventUnitCounter);
            this.ClearDataSlotArray(this._unitCountVillage2);
            this.ClearDataSlotArray(this._unitCountNewVillage2);

            if (this._missionCompleted.Count != 0)
            {
                do
                {
                    this._missionCompleted.Remove(0);
                } while (this._missionCompleted.Count != 0);
            }

            if (this._achievementRewardClaimed.Count != 0)
            {
                do
                {
                    this._achievementRewardClaimed.Remove(0);
                } while (this._achievementRewardClaimed.Count != 0);
            }

            for (int i = 0, size = stream.ReadInt(); i < size; i++)
            {
                LogicDataSlot slot = new LogicDataSlot(null, 0);
                slot.Decode(stream);
                this._resourceCap.Add(slot);
            }

            for (int i = 0, size = stream.ReadInt(); i < size; i++)
            {
                LogicDataSlot slot = new LogicDataSlot(null, 0);

                slot.Decode(stream);

                if (slot.GetData() != null)
                {
                    this._resourceCount.Add(slot);
                }
                else
                {
                    slot.Destruct();
                    slot = null;

                    Debugger.Error("LogicClientAvatar::decode - resource slot data is NULL");
                }
            }

            for (int i = 0, size = stream.ReadInt(); i < size; i++)
            {
                LogicDataSlot slot = new LogicDataSlot(null, 0);

                slot.Decode(stream);

                if (slot.GetData() != null)
                {
                    this._unitCount.Add(slot);
                }
                else
                {
                    slot.Destruct();
                    slot = null;

                    Debugger.Error("LogicClientAvatar::decode - unit slot data is NULL");
                }
            }

            for (int i = 0, size = stream.ReadInt(); i < size; i++)
            {
                LogicDataSlot slot = new LogicDataSlot(null, 0);

                slot.Decode(stream);

                if (slot.GetData() != null)
                {
                    this._spellCount.Add(slot);
                }
                else
                {
                    slot.Destruct();
                    slot = null;

                    Debugger.Error("LogicClientAvatar::decode - spell slot data is NULL");
                }
            }

            for (int i = 0, size = stream.ReadInt(); i < size; i++)
            {
                LogicDataSlot slot = new LogicDataSlot(null, 0);

                slot.Decode(stream);

                if (slot.GetData() != null)
                {
                    this._unitUpgrade.Add(slot);
                }
                else
                {
                    slot.Destruct();
                    slot = null;

                    Debugger.Error("LogicClientAvatar::decode - unit upgrade slot data is NULL");
                }
            }

            for (int i = 0, size = stream.ReadInt(); i < size; i++)
            {
                LogicDataSlot slot = new LogicDataSlot(null, 0);

                slot.Decode(stream);

                if (slot.GetData() != null)
                {
                    this._spellUpgrade.Add(slot);
                }
                else
                {
                    slot.Destruct();
                    slot = null;

                    Debugger.Error("LogicClientAvatar::decode - spell upgrade slot data is NULL");
                }
            }

            for (int i = 0, size = stream.ReadInt(); i < size; i++)
            {
                LogicDataSlot slot = new LogicDataSlot(null, 0);

                slot.Decode(stream);

                if (slot.GetData() != null)
                {
                    this._heroUpgrade.Add(slot);
                }
                else
                {
                    slot.Destruct();
                    slot = null;

                    Debugger.Error("LogicClientAvatar::decode - hero upgrade slot data is NULL");
                }
            }

            for (int i = 0, size = stream.ReadInt(); i < size; i++)
            {
                LogicDataSlot slot = new LogicDataSlot(null, 0);

                slot.Decode(stream);

                if (slot.GetData() != null)
                {
                    this._heroHealth.Add(slot);
                }
                else
                {
                    slot.Destruct();
                    slot = null;

                    Debugger.Error("LogicClientAvatar::decode - hero health slot data is NULL");
                }
            }

            for (int i = 0, size = stream.ReadInt(); i < size; i++)
            {
                LogicDataSlot slot = new LogicDataSlot(null, 0);

                slot.Decode(stream);

                if (slot.GetData() != null)
                {
                    this._heroState.Add(slot);
                }
                else
                {
                    slot.Destruct();
                    slot = null;

                    Debugger.Error("LogicClientAvatar::decode - hero state slot data is NULL");
                }
            }

            for (int i = 0, size = stream.ReadInt(); i < size; i++)
            {
                LogicUnitSlot slot = new LogicUnitSlot(null, 0, 0);

                slot.Decode(stream);

                if (slot.GetData() != null)
                {
                    this._allianceUnitCount.Add(slot);
                }
                else
                {
                    slot.Destruct();
                    slot = null;

                    Debugger.Error("LogicClientAvatar::decode - alliance unit data is NULL");
                }
            }

            for (int i = 0, size = stream.ReadInt(); i < size; i++)
            {
                LogicMissionData data = (LogicMissionData) stream.ReadDataReference(20);

                if (data != null)
                {
                    this._missionCompleted.Add(data);
                }
            }

            for (int i = 0, size = stream.ReadInt(); i < size; i++)
            {
                LogicAchievementData data = (LogicAchievementData) stream.ReadDataReference(23);

                if (data != null)
                {
                    this._achievementRewardClaimed.Add(data);
                }
            }

            for (int i = 0, size = stream.ReadInt(); i < size; i++)
            {
                LogicDataSlot slot = new LogicDataSlot(null, 0);

                slot.Decode(stream);

                if (slot.GetData() != null)
                {
                    this._achievementProgress.Add(slot);
                }
                else
                {
                    slot.Destruct();
                    slot = null;

                    Debugger.Error("LogicClientAvatar::decode - achievement progress data is NULL");
                }
            }

            for (int i = 0, size = stream.ReadInt(); i < size; i++)
            {
                LogicDataSlot slot = new LogicDataSlot(null, 0);

                slot.Decode(stream);

                if (slot.GetData() != null)
                {
                    this._npcStars.Add(slot);
                }
                else
                {
                    slot.Destruct();
                    slot = null;

                    Debugger.Error("LogicClientAvatar::decode - npc map progress data is NULL");
                }
            }

            for (int i = 0, size = stream.ReadInt(); i < size; i++)
            {
                LogicDataSlot slot = new LogicDataSlot(null, 0);

                slot.Decode(stream);

                if (slot.GetData() != null)
                {
                    this._lootedNpcGold.Add(slot);
                }
                else
                {
                    slot.Destruct();
                    slot = null;

                    Debugger.Error("LogicClientAvatar::decode - npc looted gold data is NULL");
                }
            }

            for (int i = 0, size = stream.ReadInt(); i < size; i++)
            {
                LogicDataSlot slot = new LogicDataSlot(null, 0);

                slot.Decode(stream);

                if (slot.GetData() != null)
                {
                    this._lootedNpcElixir.Add(slot);
                }
                else
                {
                    slot.Destruct();
                    slot = null;

                    Debugger.Error("LogicClientAvatar::decode - npc looted elixir data is NULL");
                }
            }

            this._allianceUnitVisitCapacity = stream.ReadInt();
            this._allianceUnitSpellVisitCapacity = stream.ReadInt();

            for (int i = 0, size = stream.ReadInt(); i < size; i++)
            {
                LogicDataSlot slot = new LogicDataSlot(null, 0);

                slot.Decode(stream);

                if (slot.GetData() != null)
                {
                    this._heroMode.Add(slot);
                }
                else
                {
                    slot.Destruct();
                    slot = null;

                    Debugger.Error("LogicClientAvatar::decode - hero mode slot data is NULL");
                }
            }

            for (int i = 0, size = stream.ReadInt(); i < size; i++)
            {
                LogicDataSlot slot = new LogicDataSlot(null, 0);

                slot.Decode(stream);

                if (slot.GetData() != null)
                {
                    this._variables.Add(slot);
                }
                else
                {
                    slot.Destruct();
                    slot = null;

                    Debugger.Error("LogicClientAvatar::decode - variables data is NULL");
                }
            }

            for (int i = 0, size = stream.ReadInt(); i < size; i++)
            {
                LogicDataSlot slot = new LogicDataSlot(null, 0);

                slot.Decode(stream);

                if (slot.GetData() != null)
                {
                    this._unitPreset1.Add(slot);
                }
                else
                {
                    slot.Destruct();
                    slot = null;

                    Debugger.Error("LogicClientAvatar::decode - unitPreset1 data is NULL");
                }
            }

            for (int i = 0, size = stream.ReadInt(); i < size; i++)
            {
                LogicDataSlot slot = new LogicDataSlot(null, 0);

                slot.Decode(stream);

                if (slot.GetData() != null)
                {
                    this._unitPreset2.Add(slot);
                }
                else
                {
                    slot.Destruct();
                    slot = null;

                    Debugger.Error("LogicClientAvatar::decode - unitPreset2 data is NULL");
                }
            }

            for (int i = 0, size = stream.ReadInt(); i < size; i++)
            {
                LogicDataSlot slot = new LogicDataSlot(null, 0);

                slot.Decode(stream);

                if (slot.GetData() != null)
                {
                    this._unitPreset3.Add(slot);
                }
                else
                {
                    slot.Destruct();
                    slot = null;

                    Debugger.Error("LogicClientAvatar::decode - unitPreset3 data is NULL");
                }
            }

            for (int i = 0, size = stream.ReadInt(); i < size; i++)
            {
                LogicDataSlot slot = new LogicDataSlot(null, 0);

                slot.Decode(stream);

                if (slot.GetData() != null)
                {
                    this._previousArmy.Add(slot);
                }
                else
                {
                    slot.Destruct();
                    slot = null;

                    Debugger.Error("LogicClientAvatar::decode - previousArmySize data is NULL");
                }
            }

            for (int i = 0, size = stream.ReadInt(); i < size; i++)
            {
                LogicDataSlot slot = new LogicDataSlot(null, 0);

                slot.Decode(stream);

                if (slot.GetData() != null)
                {
                    this._eventUnitCounter.Add(slot);
                }
                else
                {
                    slot.Destruct();
                    slot = null;

                    Debugger.Error("LogicClientAvatar::decode - unitCounterForEvent data is NULL");
                }
            }

            for (int i = 0, size = stream.ReadInt(); i < size; i++)
            {
                LogicDataSlot slot = new LogicDataSlot(null, 0);

                slot.Decode(stream);

                if (slot.GetData() != null)
                {
                    this._unitCountVillage2.Add(slot);
                }
                else
                {
                    slot.Destruct();
                    slot = null;

                    Debugger.Error("LogicClientAvatar::decode - unit village2 slot data is NULL");
                }
            }

            for (int i = 0, size = stream.ReadInt(); i < size; i++)
            {
                LogicDataSlot slot = new LogicDataSlot(null, 0);

                slot.Decode(stream);

                if (slot.GetData() != null)
                {
                    this._unitCountNewVillage2.Add(slot);
                }
                else
                {
                    slot.Destruct();
                    slot = null;

                    Debugger.Error("LogicClientAvatar::decode - unit village2 new slot data is NULL");
                }
            }

            for (int i = 0, size = stream.ReadInt(); i < size; i++)
            {
                LogicDataSlot slot = new LogicDataSlot(null, 0);

                slot.Decode(stream);

                if (slot.GetData() != null)
                {
                }
                else
                {
                    slot.Destruct();
                    slot = null;

                    Debugger.Error("LogicClientAvatar::decode - slot data is NULL");
                }
            }
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public void Encode(ChecksumEncoder encoder)
        {
            encoder.WriteLong(this._id);
            encoder.WriteLong(this._currentHomeId);

            if (this._allianceId != null)
            {
                encoder.WriteBoolean(true);
                encoder.WriteLong(this._allianceId);
                encoder.WriteString(this._allianceName);
                encoder.WriteInt(this._allianceBadgeId);
                encoder.WriteInt(this._allianceRole);
                encoder.WriteInt(this._allianceExpLevel);
            }
            else
            {
                encoder.WriteBoolean(false);
            }

            if (this._leagueInstanceId != null)
            {
                encoder.WriteBoolean(true);
                encoder.WriteLong(this._leagueInstanceId);
            }
            else
            {
                encoder.WriteBoolean(false);
            }

            encoder.WriteInt(this._legendaryScore);
            encoder.WriteInt(this._legendaryScoreVillage2);

            this._legendLeagueTournamentEntry.Encode(encoder);
            this._legendLeagueTournamentVillage2Entry.Encode(encoder);

            encoder.WriteInt(0);
            encoder.WriteInt(0);
            encoder.WriteInt(0);

            encoder.WriteInt(this._leagueType);
            encoder.WriteInt(this._allianceCastleLevel);
            encoder.WriteInt(this._allianceCastleTotalCapacity);
            encoder.WriteInt(this._allianceCastleUsedCapacity);
            encoder.WriteInt(this._allianceCastleTotalSpellCapacity);
            encoder.WriteInt(this._allianceCastleUsedSpellCapacity);

            encoder.WriteInt(this._townHallLevel);
            encoder.WriteInt(this._townHallLevelVillage2);

            encoder.WriteString(this._name);
            encoder.WriteString(this._facebookId);

            encoder.WriteInt(this._expLevel);
            encoder.WriteInt(this._expPoints);
            encoder.WriteInt(this._diamonds);
            encoder.WriteInt(this._freeDiamonds);
            encoder.WriteInt(this._attackRating);
            encoder.WriteInt(this._attackKFactor);
            encoder.WriteInt(this._score);
            encoder.WriteInt(this._duelScore);
            encoder.WriteInt(this._attackWinCount);
            encoder.WriteInt(this._attackLoseCount);
            encoder.WriteInt(this._defenseWinCount);
            encoder.WriteInt(this._defenseLoseCount);
            encoder.WriteInt(this._treasuryGoldCount);
            encoder.WriteInt(this._treasuryElixirCount);
            encoder.WriteInt(this._treasuryDarkElixirCount);
            encoder.WriteInt(0);

            if (true)
            {
                encoder.WriteBoolean(true);
                encoder.WriteLong(new LogicLong(220, 1828055880));
            }
            else
            {
                encoder.WriteBoolean(false);
            }

            encoder.WriteBoolean(this._nameSetByUser);
            encoder.WriteBoolean(false);
            encoder.WriteInt(this._nameChangeState);
            encoder.WriteInt(6900);
            encoder.WriteInt(this._redPackageState);
            encoder.WriteInt(this._warPreference);
            encoder.WriteInt(1);
            encoder.WriteInt(1);

            if (false)
            {
                encoder.WriteBoolean(true);
                encoder.WriteInt(0);
                encoder.WriteLong(0);
            }
            else
            {
                encoder.WriteBoolean(false);
            }

            encoder.WriteInt(this._resourceCap.Count);

            for (int i = 0; i < this._resourceCap.Count; i++)
            {
                this._resourceCap[i].Encode(encoder);
            }

            encoder.WriteInt(this._resourceCount.Count);

            for (int i = 0; i < this._resourceCount.Count; i++)
            {
                this._resourceCount[i].Encode(encoder);
            }

            encoder.WriteInt(this._unitCount.Count);

            for (int i = 0; i < this._unitCount.Count; i++)
            {
                this._unitCount[i].Encode(encoder);
            }

            encoder.WriteInt(this._spellCount.Count);

            for (int i = 0; i < this._spellCount.Count; i++)
            {
                this._spellCount[i].Encode(encoder);
            }

            encoder.WriteInt(this._unitUpgrade.Count);

            for (int i = 0; i < this._unitUpgrade.Count; i++)
            {
                this._unitUpgrade[i].Encode(encoder);
            }

            encoder.WriteInt(this._spellUpgrade.Count);

            for (int i = 0; i < this._spellUpgrade.Count; i++)
            {
                this._spellUpgrade[i].Encode(encoder);
            }

            encoder.WriteInt(this._heroUpgrade.Count);

            for (int i = 0; i < this._heroUpgrade.Count; i++)
            {
                this._heroUpgrade[i].Encode(encoder);
            }

            encoder.WriteInt(this._heroHealth.Count);

            for (int i = 0; i < this._heroHealth.Count; i++)
            {
                this._heroHealth[i].Encode(encoder);
            }

            encoder.WriteInt(this._heroState.Count);

            for (int i = 0; i < this._heroState.Count; i++)
            {
                this._heroState[i].Encode(encoder);
            }

            encoder.WriteInt(this._allianceUnitCount.Count);

            for (int i = 0; i < this._allianceUnitCount.Count; i++)
            {
                this._allianceUnitCount[i].Encode(encoder);
            }

            encoder.WriteInt(this._missionCompleted.Count);

            for (int i = 0; i < this._missionCompleted.Count; i++)
            {
                encoder.WriteDataReference(this._missionCompleted[i]);
            }

            encoder.WriteInt(this._achievementRewardClaimed.Count);

            for (int i = 0; i < this._achievementRewardClaimed.Count; i++)
            {
                encoder.WriteDataReference(this._achievementRewardClaimed[i]);
            }

            encoder.WriteInt(this._achievementProgress.Count);

            for (int i = 0; i < this._achievementProgress.Count; i++)
            {
                this._achievementProgress[i].Encode(encoder);
            }

            encoder.WriteInt(this._npcStars.Count);

            for (int i = 0; i < this._npcStars.Count; i++)
            {
                this._npcStars[i].Encode(encoder);
            }

            encoder.WriteInt(this._lootedNpcGold.Count);

            for (int i = 0; i < this._lootedNpcGold.Count; i++)
            {
                this._lootedNpcGold[i].Encode(encoder);
            }

            encoder.WriteInt(this._lootedNpcElixir.Count);

            for (int i = 0; i < this._lootedNpcElixir.Count; i++)
            {
                this._lootedNpcElixir[i].Encode(encoder);
            }

            encoder.WriteInt(this._allianceUnitVisitCapacity);
            encoder.WriteInt(this._allianceUnitSpellVisitCapacity);

            encoder.WriteInt(this._heroMode.Count);

            for (int i = 0; i < this._heroMode.Count; i++)
            {
                this._heroMode[i].Encode(encoder);
            }

            encoder.WriteInt(this._variables.Count);

            for (int i = 0; i < this._variables.Count; i++)
            {
                this._variables[i].Encode(encoder);
            }

            encoder.WriteInt(this._unitPreset1.Count);

            for (int i = 0; i < this._unitPreset1.Count; i++)
            {
                this._unitPreset1[i].Encode(encoder);
            }

            encoder.WriteInt(this._unitPreset2.Count);

            for (int i = 0; i < this._unitPreset2.Count; i++)
            {
                this._unitPreset2[i].Encode(encoder);
            }
            
            encoder.WriteInt(this._unitPreset3.Count);

            for (int i = 0; i < this._unitPreset3.Count; i++)
            {
                this._unitPreset3[i].Encode(encoder);
            }

            encoder.WriteInt(this._previousArmy.Count);

            for (int i = 0; i < this._previousArmy.Count; i++)
            {
                this._previousArmy[i].Encode(encoder);
            }

            encoder.WriteInt(this._eventUnitCounter.Count);

            for (int i = 0; i < this._eventUnitCounter.Count; i++)
            {
                this._eventUnitCounter[i].Encode(encoder);
            }

            encoder.WriteInt(this._unitCountVillage2.Count);

            for (int i = 0; i < this._unitCountVillage2.Count; i++)
            {
                this._unitCountVillage2[i].Encode(encoder);
            }

            encoder.WriteInt(this._unitCountNewVillage2.Count);

            for (int i = 0; i < this._unitCountNewVillage2.Count; i++)
            {
                this._unitCountNewVillage2[i].Encode(encoder);
            }

            encoder.WriteInt(0); // logicData[]
        }

        /// <summary>
        ///     Loads this instance from json.
        /// </summary>
        public void Load(LogicJSONObject jsonObject)
        {
            LogicJSONNumber avatarIdLowObject = jsonObject.GetJSONNumber("avatar_id_low");
            LogicJSONNumber avatarIdHighObject = jsonObject.GetJSONNumber("avatar_id_high");

            if (avatarIdHighObject != null && avatarIdLowObject != null)
            {
                this._id = new LogicLong(avatarIdHighObject.GetIntValue(), avatarIdLowObject.GetIntValue());
            }

            LogicJSONNumber homeIdLowObject = jsonObject.GetJSONNumber("home_id_low");
            LogicJSONNumber homeIdHighObject = jsonObject.GetJSONNumber("home_id_high");

            if (homeIdHighObject != null && homeIdLowObject != null)
            {
                this._currentHomeId = new LogicLong(homeIdHighObject.GetIntValue(), homeIdLowObject.GetIntValue());
            }

            LogicJSONString nameObject = jsonObject.GetJSONString("name");

            if (nameObject != null)
            {
                this._name = nameObject.GetStringValue();
            }

            LogicJSONBoolean nameSetObject = jsonObject.GetJSONBoolean("name_set");

            if (nameSetObject != null)
            {
                this._nameSetByUser = nameSetObject.IsTrue();
            }

            LogicJSONNumber nameChangeStateObject = jsonObject.GetJSONNumber("name_change_state");

            if (nameChangeStateObject != null)
            {
                this._nameChangeState = nameChangeStateObject.GetIntValue();
            }
            
            LogicJSONNumber badgeIdObject = jsonObject.GetJSONNumber("badge_id");

            if (badgeIdObject != null)
            {
                this._badgeId = badgeIdObject.GetIntValue();
            }

            LogicJSONNumber allianceExpLevelObject = jsonObject.GetJSONNumber("alliance_exp_level");

            if (allianceExpLevelObject != null)
            {
                this._allianceExpLevel = allianceExpLevelObject.GetIntValue();
            }

            if (this._badgeId == -1)
            {
                this._allianceId = null;
            }
            else
            {
                LogicJSONNumber allianceIdLowObject = jsonObject.GetJSONNumber("alliance_id_high");
                LogicJSONNumber allianceIdHighObject = jsonObject.GetJSONNumber("alliance_id_low");

                int allIdHigh = -1;
                int allIdLow = -1;

                if (allianceIdHighObject != null && allianceIdLowObject != null)
                {
                    allIdHigh = allianceIdHighObject.GetIntValue();
                    allIdLow = allianceIdLowObject.GetIntValue();
                }

                this._allianceId = new LogicLong(allIdHigh, allIdLow);
                this._allianceName = LogicJSONHelper.GetJSONString(jsonObject, "alliance_name");
            }

            LogicJSONNumber leagueIdLowObject = jsonObject.GetJSONNumber("league_id_low");
            LogicJSONNumber leagueIdHighObject = jsonObject.GetJSONNumber("league_id_high");

            if (leagueIdHighObject != null && leagueIdLowObject != null)
            {
                this._leagueInstanceId = new LogicLong(leagueIdHighObject.GetIntValue(), leagueIdLowObject.GetIntValue());
            }

            this._allianceUnitVisitCapacity = LogicJSONHelper.GetJSONNumber(jsonObject, "alliance_unit_visit_capacity");
            this._allianceUnitSpellVisitCapacity = LogicJSONHelper.GetJSONNumber(jsonObject, "alliance_unit_spell_visit_capacity");
            this._expLevel = LogicJSONHelper.GetJSONNumber(jsonObject, "xp_level");
            this._expPoints = LogicJSONHelper.GetJSONNumber(jsonObject, "xp_points");
            this._diamonds = LogicJSONHelper.GetJSONNumber(jsonObject, "diamonds");
            this._freeDiamonds = LogicJSONHelper.GetJSONNumber(jsonObject, "free_diamonds");

            this._leagueType = LogicJSONHelper.GetJSONNumber(jsonObject, "league_type");
            this._legendaryScore = LogicJSONHelper.GetJSONNumber(jsonObject, "legendary_score");
            this._legendaryScoreVillage2 = LogicJSONHelper.GetJSONNumber(jsonObject, "legendary_score_v2");

            LogicJSONObject legendLeagueEntry = jsonObject.GetJSONObject("legend_league_entry");

            if (legendLeagueEntry != null)
            {
                this._legendLeagueTournamentEntry.ReadFromJSON(legendLeagueEntry);
            }

            LogicJSONObject legendLeagueEntryV2 = jsonObject.GetJSONObject("legend_league_entry_v2");

            if (legendLeagueEntryV2 != null)
            {
                this._legendLeagueTournamentVillage2Entry.ReadFromJSON(legendLeagueEntryV2);
            }

            this.LoadDataSlotArray(jsonObject, "units", this._unitCount);
            this.LoadDataSlotArray(jsonObject, "spells", this._spellCount);
            this.LoadDataSlotArray(jsonObject, "unit_upgrades", this._unitUpgrade);
            this.LoadDataSlotArray(jsonObject, "spell_upgrades", this._spellUpgrade);
            this.LoadDataSlotArray(jsonObject, "resources", this._resourceCount);
            this.LoadDataSlotArray(jsonObject, "resource_caps", this._resourceCap);
            this.LoadUnitSlotArray(jsonObject, "alliance_units", this._allianceUnitCount);
            this.LoadDataSlotArray(jsonObject, "hero_states", this._heroState);
            this.LoadDataSlotArray(jsonObject, "hero_health", this._heroHealth);
            this.LoadDataSlotArray(jsonObject, "hero_upgrade", this._heroUpgrade);
            this.LoadDataSlotArray(jsonObject, "hero_modes", this._heroMode);
            this.LoadDataSlotArray(jsonObject, "variables", this._variables);
            this.LoadDataSlotArray(jsonObject, "units2", this._unitCountVillage2);
            this.LoadDataSlotArray(jsonObject, "units_new2", this._unitCountNewVillage2);
            this.LoadDataSlotArray(jsonObject, "unit_preset1", this._unitPreset1);
            this.LoadDataSlotArray(jsonObject, "unit_preset2", this._unitPreset2);
            this.LoadDataSlotArray(jsonObject, "unit_preset3", this._unitPreset3);
            this.LoadDataSlotArray(jsonObject, "previous_army", this._previousArmy);
            this.LoadDataSlotArray(jsonObject, "event_unit_counter", this._eventUnitCounter);
            this.LoadDataSlotArray(jsonObject, "looted_npc_gold", this._lootedNpcGold);
            this.LoadDataSlotArray(jsonObject, "looted_npc_elixir", this._lootedNpcElixir);
            this.LoadDataSlotArray(jsonObject, "npc_stars", this._npcStars);
            this.LoadDataSlotArray(jsonObject, "achievement_progress", this._achievementProgress);

            LogicJSONArray achievementRewardClaimedArray = jsonObject.GetJSONArray("achievement_rewards");

            if (achievementRewardClaimedArray != null)
            {
                if (achievementRewardClaimedArray.Size() != 0)
                {
                    this._achievementRewardClaimed.EnsureCapacity(achievementRewardClaimedArray.Size());

                    if (this._achievementRewardClaimed.Count != 0)
                    {
                        do
                        {
                            this._achievementRewardClaimed.Remove(0);
                        } while (this._achievementRewardClaimed.Count != 0);
                    }

                    for (int i = 0; i < achievementRewardClaimedArray.Size(); i++)
                    {
                        LogicJSONNumber id = achievementRewardClaimedArray.GetJSONNumber(i);

                        if (id != null)
                        {
                            LogicData data = LogicDataTables.GetDataById(id.GetIntValue());

                            if (data != null)
                            {
                                this._achievementRewardClaimed.Add(data);
                            }
                        }
                    }
                }
            }

            LogicJSONArray missionCompletedArray = jsonObject.GetJSONArray("missions");

            if (missionCompletedArray != null)
            {
                if (missionCompletedArray.Size() != 0)
                {
                    this._missionCompleted.EnsureCapacity(missionCompletedArray.Size());

                    if (this._missionCompleted.Count != 0)
                    {
                        do
                        {
                            this._missionCompleted.Remove(0);
                        } while (this._missionCompleted.Count != 0);
                    }

                    for (int i = 0; i < missionCompletedArray.Size(); i++)
                    {
                        LogicJSONNumber id = missionCompletedArray.GetJSONNumber(i);

                        if (id != null)
                        {
                            LogicData data = LogicDataTables.GetDataById(id.GetIntValue());

                            if (data != null)
                            {
                                this._missionCompleted.Add(data);
                            }
                        }
                    }
                }
            }

            this._allianceCastleLevel = LogicJSONHelper.GetJSONNumber(jsonObject, "castle_lvl");
            this._allianceCastleTotalCapacity = LogicJSONHelper.GetJSONNumber(jsonObject, "castle_total");
            this._allianceCastleUsedCapacity = LogicJSONHelper.GetJSONNumber(jsonObject, "castle_used");
            this._allianceCastleTotalSpellCapacity = LogicJSONHelper.GetJSONNumber(jsonObject, "castle_total_sp");
            this._allianceCastleUsedSpellCapacity = LogicJSONHelper.GetJSONNumber(jsonObject, "castle_used_sp");
            this._townHallLevel = LogicJSONHelper.GetJSONNumber(jsonObject, "town_hall_lvl");
            this._townHallLevelVillage2 = LogicJSONHelper.GetJSONNumber(jsonObject, "th_v2_lvl");
            this._score = LogicJSONHelper.GetJSONNumber(jsonObject, "score");
            this._duelScore = LogicJSONHelper.GetJSONNumber(jsonObject, "duel_score");
            this._warPreference = LogicJSONHelper.GetJSONNumber(jsonObject, "war_preference");
            this._attackRating = LogicJSONHelper.GetJSONNumber(jsonObject, "attack_rating");
            this._attackKFactor = LogicJSONHelper.GetJSONNumber(jsonObject, "atack_kfactor");
            this._attackWinCount = LogicJSONHelper.GetJSONNumber(jsonObject, "attack_win_cnt");
            this._attackLoseCount = LogicJSONHelper.GetJSONNumber(jsonObject, "attack_lose_cnt");
            this._defenseWinCount = LogicJSONHelper.GetJSONNumber(jsonObject, "defense_win_cnt");
            this._defenseLoseCount = LogicJSONHelper.GetJSONNumber(jsonObject, "defense_lose_cnt");
            this._treasuryGoldCount = LogicJSONHelper.GetJSONNumber(jsonObject, "treasury_gold_cnt");
            this._treasuryElixirCount = LogicJSONHelper.GetJSONNumber(jsonObject, "treasury_elixir_cnt");
            this._treasuryDarkElixirCount = LogicJSONHelper.GetJSONNumber(jsonObject, "treasury_dark_elixir_cnt");
            this._redPackageState = LogicJSONHelper.GetJSONNumber(jsonObject, "red_package_state");
        }

        /// <summary>
        ///     Loads this instance for replay.
        /// </summary>
        public void LoadFromReplay(LogicJSONObject jsonObject)
        {
            LogicJSONNumber avatarIdLowObject = jsonObject.GetJSONNumber("avatar_id_low");
            LogicJSONNumber avatarIdHighObject = jsonObject.GetJSONNumber("avatar_id_high");

            if (avatarIdHighObject != null)
            {
                if (avatarIdLowObject != null)
                {
                    this._id = new LogicLong(avatarIdHighObject.GetIntValue(), avatarIdLowObject.GetIntValue());
                }
            }

            LogicJSONString nameObject = jsonObject.GetJSONString("name");

            if (nameObject != null)
            {
                this._name = nameObject.GetStringValue();
            }
            
            LogicJSONNumber badgeIdObject = jsonObject.GetJSONNumber("badge_id");

            if (badgeIdObject != null)
            {
                this._badgeId = badgeIdObject.GetIntValue();
            }

            LogicJSONNumber allianceExpLevelObject = jsonObject.GetJSONNumber("alliance_exp_level");

            if (allianceExpLevelObject != null)
            {
                this._allianceExpLevel = allianceExpLevelObject.GetIntValue();
            }

            if (this._badgeId == -1)
            {
                this._allianceId = null;
            }
            else
            {
                LogicJSONNumber allianceIdLowObject = jsonObject.GetJSONNumber("alliance_id_high");
                LogicJSONNumber allianceIdHighObject = jsonObject.GetJSONNumber("alliance_id_low");

                int allIdHigh = -1;
                int allIdLow = -1;

                if (allianceIdHighObject != null)
                {
                    if (allianceIdLowObject != null)
                    {
                        allIdHigh = allianceIdHighObject.GetIntValue();
                        allIdLow = allianceIdLowObject.GetIntValue();
                    }
                }

                this._allianceId = new LogicLong(allIdHigh, allIdLow);
                this._allianceName = LogicJSONHelper.GetJSONString(jsonObject, "alliance_name");
            }

            this._allianceUnitVisitCapacity = LogicJSONHelper.GetJSONNumber(jsonObject, "alliance_unit_visit_capacity");
            this._allianceUnitSpellVisitCapacity = LogicJSONHelper.GetJSONNumber(jsonObject, "alliance_unit_spell_visit_capacity");
            this._leagueType = LogicJSONHelper.GetJSONNumber(jsonObject, "league_type");
            this._expLevel = LogicJSONHelper.GetJSONNumber(jsonObject, "xp_level");

            this.LoadDataSlotArray(jsonObject, "units", this._unitCount);
            this.LoadDataSlotArray(jsonObject, "spells", this._spellCount);
            this.LoadDataSlotArray(jsonObject, "unit_upgrades", this._unitUpgrade);
            this.LoadDataSlotArray(jsonObject, "spell_upgrades", this._spellUpgrade);
            this.LoadDataSlotArray(jsonObject, "resources", this._resourceCount);
            this.LoadUnitSlotArray(jsonObject, "alliance_units", this._allianceUnitCount);
            this.LoadDataSlotArray(jsonObject, "hero_states", this._heroState);
            this.LoadDataSlotArray(jsonObject, "hero_health", this._heroHealth);
            this.LoadDataSlotArray(jsonObject, "hero_upgrade", this._heroUpgrade);
            this.LoadDataSlotArray(jsonObject, "hero_modes", this._heroMode);
            this.LoadDataSlotArray(jsonObject, "variables", this._variables);
            this.LoadDataSlotArray(jsonObject, "units2", this._unitCountVillage2);

            this._allianceCastleLevel = LogicJSONHelper.GetJSONNumber(jsonObject, "castle_lvl");
            this._allianceCastleTotalCapacity = LogicJSONHelper.GetJSONNumber(jsonObject, "castle_total");
            this._allianceCastleUsedCapacity = LogicJSONHelper.GetJSONNumber(jsonObject, "castle_used");
            this._allianceCastleTotalSpellCapacity = LogicJSONHelper.GetJSONNumber(jsonObject, "castle_total_sp");
            this._allianceCastleUsedSpellCapacity = LogicJSONHelper.GetJSONNumber(jsonObject, "castle_used_sp");
            this._townHallLevel = LogicJSONHelper.GetJSONNumber(jsonObject, "town_hall_lvl");
            this._townHallLevelVillage2 = LogicJSONHelper.GetJSONNumber(jsonObject, "th_v2_lvl");
            this._score = LogicJSONHelper.GetJSONNumber(jsonObject, "score");
            this._duelScore = LogicJSONHelper.GetJSONNumber(jsonObject, "duel_score");
            this._redPackageState = LogicJSONHelper.GetJSONNumber(jsonObject, "red_package_state");
        }

        /// <summary>
        ///     Loads the <see cref="LogicArrayList{T}" /> from json.
        /// </summary>
        private void LoadDataSlotArray(LogicJSONObject jsonObject, string key, LogicArrayList<LogicDataSlot> dataSlotArray)
        {
            this.ClearDataSlotArray(dataSlotArray);

            LogicJSONArray jsonArray = jsonObject.GetJSONArray(key);

            if (jsonArray != null)
            {
                int arraySize = jsonArray.Size();

                if (arraySize != 0)
                {
                    dataSlotArray.EnsureCapacity(arraySize);

                    for (int i = 0; i < arraySize; i++)
                    {
                        LogicJSONObject obj = jsonArray.GetJSONObject(i);

                        if (obj != null)
                        {
                            LogicDataSlot slot = new LogicDataSlot(null, 0);

                            slot.ReadFromJSON(obj);

                            if (slot.GetData() != null)
                            {
                                dataSlotArray.Add(slot);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Loads the <see cref="LogicArrayList{T}" /> from json.
        /// </summary>
        private void LoadUnitSlotArray(LogicJSONObject jsonObject, string key, LogicArrayList<LogicUnitSlot> unitSlotArray)
        {
            this.ClearUnitSlotArray(unitSlotArray);

            LogicJSONArray jsonArray = jsonObject.GetJSONArray(key);

            if (jsonArray != null)
            {
                int arraySize = jsonArray.Size();

                if (arraySize != 0)
                {
                    unitSlotArray.EnsureCapacity(arraySize);

                    for (int i = 0; i < arraySize; i++)
                    {
                        LogicJSONObject obj = jsonArray.GetJSONObject(i);

                        if (obj != null)
                        {
                            LogicUnitSlot slot = new LogicUnitSlot(null, 0, 0);

                            slot.ReadFromJSON(obj);

                            if (slot.GetData() != null)
                            {
                                unitSlotArray.Add(slot);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Saves this instance.
        /// </summary>
        public LogicJSONObject Save()
        {
            LogicJSONObject jsonObject = new LogicJSONObject();

            jsonObject.Put("avatar_id_high", new LogicJSONNumber(this._id.GetHigherInt()));
            jsonObject.Put("avatar_id_low", new LogicJSONNumber(this._id.GetLowerInt()));
            jsonObject.Put("home_id_high", new LogicJSONNumber(this._currentHomeId.GetHigherInt()));
            jsonObject.Put("home_id_low", new LogicJSONNumber(this._currentHomeId.GetLowerInt()));
            jsonObject.Put("name", new LogicJSONString(this._name));
            jsonObject.Put("name_set", new LogicJSONBoolean(this._nameSetByUser));
            jsonObject.Put("name_change_state", new LogicJSONNumber(this._nameChangeState));
            jsonObject.Put("alliance_name", new LogicJSONString(this._allianceName ?? string.Empty));
            jsonObject.Put("xp_level", new LogicJSONNumber(this._expLevel));
            jsonObject.Put("xp_points", new LogicJSONNumber(this._expPoints));
            jsonObject.Put("diamonds", new LogicJSONNumber(this._diamonds));
            jsonObject.Put("free_diamonds", new LogicJSONNumber(this._freeDiamonds));

            if (this._allianceId != null)
            {
                jsonObject.Put("alliance_id_high", new LogicJSONNumber(this._allianceId.GetHigherInt()));
                jsonObject.Put("alliance_id_low", new LogicJSONNumber(this._allianceId.GetLowerInt()));
                jsonObject.Put("badge_id", new LogicJSONNumber(this._badgeId));
                jsonObject.Put("alliance_exp_level", new LogicJSONNumber(this._allianceExpLevel));
                jsonObject.Put("alliance_unit_visit_capacity", new LogicJSONNumber(this._allianceUnitVisitCapacity));
                jsonObject.Put("alliance_unit_spell_visit_capacity", new LogicJSONNumber(this._allianceUnitSpellVisitCapacity));
            }

            if (this._leagueInstanceId != null)
            {
                jsonObject.Put("league_id_high", new LogicJSONNumber(this._leagueInstanceId.GetHigherInt()));
                jsonObject.Put("league_id_low", new LogicJSONNumber(this._leagueInstanceId.GetLowerInt()));
            }

            jsonObject.Put("league_type", new LogicJSONNumber(this._leagueType));
            jsonObject.Put("legendary_score", new LogicJSONNumber(this._legendaryScore));
            jsonObject.Put("legendary_score_v2", new LogicJSONNumber(this._legendaryScoreVillage2));

            LogicJSONObject legendLeagueTournamentEntryObject = new LogicJSONObject();
            this._legendLeagueTournamentEntry.WriteToJSON(legendLeagueTournamentEntryObject);
            jsonObject.Put("legend_league_entry", legendLeagueTournamentEntryObject);

            LogicJSONObject legendLeagueTournamentEntryVillage2Object = new LogicJSONObject();
            this._legendLeagueTournamentVillage2Entry.WriteToJSON(legendLeagueTournamentEntryVillage2Object);
            jsonObject.Put("legend_league_entry_v2", legendLeagueTournamentEntryVillage2Object);

            this.SaveDataSlotArray(jsonObject, "units", this._unitCount);
            this.SaveDataSlotArray(jsonObject, "spells", this._spellCount);
            this.SaveDataSlotArray(jsonObject, "unit_upgrades", this._unitUpgrade);
            this.SaveDataSlotArray(jsonObject, "spell_upgrades", this._spellUpgrade);
            this.SaveDataSlotArray(jsonObject, "resources", this._resourceCount);
            this.SaveDataSlotArray(jsonObject, "resource_caps", this._resourceCap);
            this.SaveUnitSlotArray(jsonObject, "alliance_units", this._allianceUnitCount);
            this.SaveDataSlotArray(jsonObject, "hero_states", this._heroState);
            this.SaveDataSlotArray(jsonObject, "hero_health", this._heroHealth);
            this.SaveDataSlotArray(jsonObject, "hero_upgrade", this._heroUpgrade);
            this.SaveDataSlotArray(jsonObject, "hero_modes", this._heroMode);
            this.SaveDataSlotArray(jsonObject, "variables", this._variables);
            this.SaveDataSlotArray(jsonObject, "units2", this._unitCountVillage2);
            this.SaveDataSlotArray(jsonObject, "units_new2", this._unitCountNewVillage2);
            this.SaveDataSlotArray(jsonObject, "unit_preset1", this._unitPreset1);
            this.SaveDataSlotArray(jsonObject, "unit_preset2", this._unitPreset2);
            this.SaveDataSlotArray(jsonObject, "unit_preset3", this._unitPreset3);
            this.SaveDataSlotArray(jsonObject, "previous_army", this._previousArmy);
            this.SaveDataSlotArray(jsonObject, "event_unit_counter", this._eventUnitCounter);
            this.SaveDataSlotArray(jsonObject, "looted_npc_gold", this._lootedNpcGold);
            this.SaveDataSlotArray(jsonObject, "looted_npc_elixir", this._lootedNpcElixir);
            this.SaveDataSlotArray(jsonObject, "npc_stars", this._npcStars);
            this.SaveDataSlotArray(jsonObject, "achievement_progress", this._achievementProgress);

            LogicJSONArray achievementRewardClaimedArray = new LogicJSONArray();

            for (int i = 0; i < this._achievementRewardClaimed.Count; i++)
            {
                achievementRewardClaimedArray.Add(new LogicJSONNumber(this._achievementRewardClaimed[i].GetGlobalID()));
            }

            jsonObject.Put("achievement_rewards", achievementRewardClaimedArray);

            LogicJSONArray missionCompletedArray = new LogicJSONArray();

            for (int i = 0; i < this._missionCompleted.Count; i++)
            {
                missionCompletedArray.Add(new LogicJSONNumber(this._missionCompleted[i].GetGlobalID()));
            }

            jsonObject.Put("missions", missionCompletedArray);

            jsonObject.Put("castle_lvl", new LogicJSONNumber(this._allianceCastleLevel));
            jsonObject.Put("castle_total", new LogicJSONNumber(this._allianceCastleTotalCapacity));
            jsonObject.Put("castle_used", new LogicJSONNumber(this._allianceCastleUsedCapacity));
            jsonObject.Put("castle_total_sp", new LogicJSONNumber(this._allianceCastleTotalSpellCapacity));
            jsonObject.Put("castle_used_sp", new LogicJSONNumber(this._allianceCastleUsedSpellCapacity));
            jsonObject.Put("town_hall_lvl", new LogicJSONNumber(this._townHallLevel));
            jsonObject.Put("th_v2_lvl", new LogicJSONNumber(this._townHallLevelVillage2));
            jsonObject.Put("score", new LogicJSONNumber(this._score));
            jsonObject.Put("duel_score", new LogicJSONNumber(this._duelScore));
            jsonObject.Put("war_preference", new LogicJSONNumber(this._warPreference));
            jsonObject.Put("attack_rating", new LogicJSONNumber(this._attackRating));
            jsonObject.Put("atack_kfactor", new LogicJSONNumber(this._attackKFactor));
            jsonObject.Put("attack_win_cnt", new LogicJSONNumber(this._attackWinCount));
            jsonObject.Put("attack_lose_cnt", new LogicJSONNumber(this._attackLoseCount));
            jsonObject.Put("defense_win_cnt", new LogicJSONNumber(this._defenseWinCount));
            jsonObject.Put("defense_lose_cnt", new LogicJSONNumber(this._defenseLoseCount));
            jsonObject.Put("treasury_gold_cnt", new LogicJSONNumber(this._treasuryGoldCount));
            jsonObject.Put("treasury_elixir_cnt", new LogicJSONNumber(this._treasuryElixirCount));
            jsonObject.Put("treasury_dark_elixir_cnt", new LogicJSONNumber(this._treasuryDarkElixirCount));

            if (this._redPackageState != 0)
            {
                jsonObject.Put("red_package_state", new LogicJSONNumber(this._redPackageState));
            }

            return jsonObject;
        }

        /// <summary>
        ///     Saves this instance to replay.
        /// </summary>
        public void SaveToReplay(LogicJSONObject jsonObject)
        {
            jsonObject.Put("avatar_id_high", new LogicJSONNumber(this._id.GetHigherInt()));
            jsonObject.Put("avatar_id_low", new LogicJSONNumber(this._id.GetLowerInt()));
            jsonObject.Put("name", new LogicJSONString(this._name));
            jsonObject.Put("alliance_name", new LogicJSONString(this._allianceName ?? string.Empty));
            jsonObject.Put("xp_level", new LogicJSONNumber(this._expLevel));

            if (this._allianceId != null)
            {
                jsonObject.Put("alliance_id_high", new LogicJSONNumber(this._allianceId.GetHigherInt()));
                jsonObject.Put("alliance_id_low", new LogicJSONNumber(this._allianceId.GetLowerInt()));
                jsonObject.Put("badge_id", new LogicJSONNumber(this._badgeId));
                jsonObject.Put("alliance_exp_level", new LogicJSONNumber(this._allianceExpLevel));
                jsonObject.Put("alliance_unit_visit_capacity", new LogicJSONNumber(this._allianceUnitVisitCapacity));
                jsonObject.Put("alliance_unit_spell_visit_capacity", new LogicJSONNumber(this._allianceUnitSpellVisitCapacity));
            }

            jsonObject.Put("league_type", new LogicJSONNumber(this._leagueType));

            this.SaveDataSlotArray(jsonObject, "units", this._unitCount);
            this.SaveDataSlotArray(jsonObject, "spells", this._spellCount);
            this.SaveDataSlotArray(jsonObject, "unit_upgrades", this._unitUpgrade);
            this.SaveDataSlotArray(jsonObject, "spell_upgrades", this._spellUpgrade);
            this.SaveDataSlotArray(jsonObject, "resources", this._resourceCount);
            this.SaveUnitSlotArray(jsonObject, "alliance_units", this._allianceUnitCount);
            this.SaveDataSlotArray(jsonObject, "hero_states", this._heroState);
            this.SaveDataSlotArray(jsonObject, "hero_health", this._heroHealth);
            this.SaveDataSlotArray(jsonObject, "hero_upgrade", this._heroUpgrade);
            this.SaveDataSlotArray(jsonObject, "hero_modes", this._heroMode);
            this.SaveDataSlotArray(jsonObject, "variables", this._variables);
            this.SaveDataSlotArray(jsonObject, "units2", this._unitCountVillage2);

            jsonObject.Put("castle_lvl", new LogicJSONNumber(this._allianceCastleLevel));
            jsonObject.Put("castle_total", new LogicJSONNumber(this._allianceCastleTotalCapacity));
            jsonObject.Put("castle_used", new LogicJSONNumber(this._allianceCastleUsedCapacity));
            jsonObject.Put("castle_total_sp", new LogicJSONNumber(this._allianceCastleTotalSpellCapacity));
            jsonObject.Put("castle_used_sp", new LogicJSONNumber(this._allianceCastleUsedSpellCapacity));
            jsonObject.Put("town_hall_lvl", new LogicJSONNumber(this._townHallLevel));
            jsonObject.Put("th_v2_lvl", new LogicJSONNumber(this._townHallLevelVillage2));
            jsonObject.Put("score", new LogicJSONNumber(this._score));
            jsonObject.Put("duel_score", new LogicJSONNumber(this._duelScore));

            if (this._redPackageState != 0)
            {
                jsonObject.Put("red_package_state", new LogicJSONNumber(this._redPackageState));
            }
        }

        /// <summary>
        ///     Saves the <see cref="LogicArrayList{T}" /> to json.
        /// </summary>
        private void SaveDataSlotArray(LogicJSONObject jsonObject, string key, LogicArrayList<LogicDataSlot> dataSlotArray)
        {
            LogicJSONArray jsonArray = new LogicJSONArray(dataSlotArray.Count);

            for (int i = 0; i < dataSlotArray.Count; i++)
            {
                LogicJSONObject obj = new LogicJSONObject();
                dataSlotArray[i].WriteToJSON(obj);
                jsonArray.Add(obj);
            }

            jsonObject.Put(key, jsonArray);
        }

        /// <summary>
        ///     Saves the <see cref="LogicArrayList{T}" /> to json.
        /// </summary>
        private void SaveUnitSlotArray(LogicJSONObject jsonObject, string key, LogicArrayList<LogicUnitSlot> unitSlotArray)
        {
            LogicJSONArray jsonArray = new LogicJSONArray(unitSlotArray.Count);

            for (int i = 0; i < unitSlotArray.Count; i++)
            {
                LogicJSONObject obj = new LogicJSONObject();
                unitSlotArray[i].WriteToJSON(obj);
                jsonArray.Add(obj);
            }

            jsonObject.Put(key, jsonArray);
        }
    }
}