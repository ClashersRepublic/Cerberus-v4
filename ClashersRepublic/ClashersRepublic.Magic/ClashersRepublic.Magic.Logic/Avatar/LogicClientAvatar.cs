namespace ClashersRepublic.Magic.Logic.Avatar
{
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Logic.League.Entry;
    using ClashersRepublic.Magic.Logic.Utils;
    using ClashersRepublic.Magic.Titan.DataStream;
    using ClashersRepublic.Magic.Titan.Debug;
    using ClashersRepublic.Magic.Titan.Json;
    using ClashersRepublic.Magic.Titan.Math;
    using ClashersRepublic.Magic.Titan.Util;
    using Supercell.Magic.Logic;

    public sealed class LogicClientAvatar : LogicAvatar
    {
        private LogicLong _id;
        private LogicLong _homeId;
        private LogicLong _allianceId;
        private LogicLong _leagueInstanceId;

        private LogicLegendLeagueTournamentEntry _legendLeagueTournamentEntry;
        private LogicLegendLeagueTournamentEntry _legendLeagueTournamentVillage2Entry;

        private bool _nameSetByUser;

        private int _allianceBadgeId;
        private int _allianceRole;
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
        private int _nameChangeState;

        private string _facebookId;
        private string _allianceName;
        private string _name;
        
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicClientAvatar"/> class.
        /// </summary>
        public LogicClientAvatar()
        {
            this._expLevel = 1;
            this._nameChangeState = -1;
            this._attackRating = 1200;
            this._attackKFactor = 60;
            this._warPreference = 1;

            this._name = string.Empty;
            this._allianceName = string.Empty;
            
            this._legendLeagueTournamentEntry = new LogicLegendLeagueTournamentEntry();
            this._legendLeagueTournamentVillage2Entry = new LogicLegendLeagueTournamentEntry();
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

            if (this._allianceId != 0)
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
            LogicGlobals globalsInstance = LogicDataTables.GetGlobalsInstance();

            defaultAvatar._diamonds = globalsInstance.GetStartingDiamonds();
            defaultAvatar._freeDiamonds = globalsInstance.GetStartingDiamonds();

            defaultAvatar.SetResourceCount(LogicDataTables.GetGoldData(), globalsInstance.GetStartingGold());
            defaultAvatar.SetResourceCount(LogicDataTables.GetGold2Data(), globalsInstance.GetStartingGold2());
            defaultAvatar.SetResourceCount(LogicDataTables.GetElixirData(), globalsInstance.GetStartingElixir());
            defaultAvatar.SetResourceCount(LogicDataTables.GetElixir2Data(), globalsInstance.GetStartingElixir2());

            return defaultAvatar;
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
        ///     Gets a value indicating whether the avatar is in alliance.
        /// </summary>
        /// <returns></returns>
        public override bool IsInAlliance()
        {
            return this._allianceId != 0;
        }

        /// <summary>
        ///     Gets the experience level.
        /// </summary>
        public override int GetExpLevel()
        {
            return this._expLevel;
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
        ///     Gets the home id.
        /// </summary>
        public LogicLong GetHomeId()
        {
            return this._homeId;
        }

        /// <summary>
        ///     Sets the home id.
        /// </summary>
        public void SetHomeId(LogicLong id)
        {
            this._homeId = id;
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
        ///     Gets the league type.
        /// </summary>
        public int GetLeagueType()
        {
            return this._leagueType;
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
        ///     Encodes this instance.
        /// </summary>
        public void Encode(ChecksumEncoder encoder)
        {
            encoder.WriteLong(this._id);
            encoder.WriteLong(this._homeId);

            if (this._allianceId != 0)
            {
                encoder.WriteBoolean(true);
                encoder.WriteLong(this._allianceId);
                encoder.WriteString(this._allianceName);
                encoder.WriteInt(this._allianceBadgeId);
                encoder.WriteInt(this._allianceRole);
            }
            else
            {
                encoder.WriteBoolean(false);
            }

            if (this._leagueInstanceId != 0)
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
            encoder.WriteInt(this._allianceCastleUsedCapacity);
            encoder.WriteInt(this._allianceCastleTotalCapacity);
            encoder.WriteInt(this._allianceCastleSpellUsedCapacity);
            encoder.WriteInt(this._allianceCastleSpellTotalCapacity);

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
            encoder.WriteInt(0);
            encoder.WriteInt(this._warPreference);
            encoder.WriteInt(0);
            encoder.WriteInt(0);

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

            encoder.WriteInt(0);
            encoder.WriteInt(0);

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
        public void Load(LogicJSONObject json)
        {
            this._id = new LogicLong(LogicJSONHelper.GetJSONNumber(json, "id_high"), LogicJSONHelper.GetJSONNumber(json, "id_low"));
            this._homeId = new LogicLong(LogicJSONHelper.GetJSONNumber(json, "home_id_high"), LogicJSONHelper.GetJSONNumber(json, "home_id_low"));
            this._allianceId = new LogicLong(LogicJSONHelper.GetJSONNumber(json, "alliance_id_high"), LogicJSONHelper.GetJSONNumber(json, "alliance_id_low"));
            this._leagueInstanceId = new LogicLong(LogicJSONHelper.GetJSONNumber(json, "league_id_high"), LogicJSONHelper.GetJSONNumber(json, "league_id_low"));
            
            this._name = LogicJSONHelper.GetJSONString(json, "name");
            this._allianceName = LogicJSONHelper.GetJSONString(json, "alliance_name");
            this._facebookId = LogicJSONHelper.GetJSONString(json, "facebook_id");

            LogicJSONObject prvSeasonRankingJsonObject = json.GetJSONObject("previous_season_ranking");
            LogicJSONObject bestSeasonRankingJsonObject = json.GetJSONObject("best_season_ranking");

            if (prvSeasonRankingJsonObject != null)
            {
                this._legendLeagueTournamentEntry.ReadFromJSON(prvSeasonRankingJsonObject);
            }

            if (bestSeasonRankingJsonObject != null)
            {
                this._legendLeagueTournamentVillage2Entry.ReadFromJSON(bestSeasonRankingJsonObject);
            }

            this._allianceBadgeId = LogicJSONHelper.GetJSONNumber(json, "badge_id");
            this._leagueType = LogicJSONHelper.GetJSONNumber(json, "league_type");
            this._diamonds = LogicJSONHelper.GetJSONNumber(json, "diamonds");
            this._freeDiamonds = LogicJSONHelper.GetJSONNumber(json, "free_diamonds");
            this._expLevel = LogicJSONHelper.GetJSONNumber(json, "exp_level");
            this._expPoints = LogicJSONHelper.GetJSONNumber(json, "exp_points");
            this._score = LogicJSONHelper.GetJSONNumber(json, "score");
            this._duelScore = LogicJSONHelper.GetJSONNumber(json, "duel_score");
            this._townHallLevel = LogicJSONHelper.GetJSONNumber(json, "town_hall_level");
            this._townHallLevelVillage2 = LogicJSONHelper.GetJSONNumber(json, "town_hall_level_village2");
            this._treasuryGoldCount = LogicJSONHelper.GetJSONNumber(json, "treasury_gold_cnt");
            this._treasuryElixirCount = LogicJSONHelper.GetJSONNumber(json, "treasury_elixir_cnt");
            this._treasuryDarkElixirCount = LogicJSONHelper.GetJSONNumber(json, "treasury_dark_elixir_cnt");

            this.LoadDataSlotArrayList(this._resourceCap, json, "resource_caps");
            this.LoadDataSlotArrayList(this._resourceCount, json, "resources");
            this.LoadDataSlotArrayList(this._unitCount, json, "units");
            this.LoadDataSlotArrayList(this._spellCount, json, "spells");
            this.LoadDataSlotArrayList(this._unitUpgrade, json, "unit_upgrades");
            this.LoadDataSlotArrayList(this._spellUpgrade, json, "spell_upgrades");
            this.LoadUnitSlotArrayList(this._allianceUnitCount, json, "alliance_units");
            this.LoadDataSlotArrayList(this._achievementProgress, json, "achievements");
            this.LoadDataArrayList(this._achievementRewardClaimed, json, "achievements_claimed");
            this.LoadDataArrayList(this._missionCompleted, json, "missions");
            this.LoadDataSlotArrayList(this._npcStars, json, "npc_stars");
            this.LoadDataSlotArrayList(this._lootedNpcGold, json, "npc_looted_gold");
            this.LoadDataSlotArrayList(this._lootedNpcElixir, json, "npc_looted_elixir");
        }

        /// <summary>
        ///     Loads the specified data slot array list.
        /// </summary>
        private void LoadDataArrayList(LogicArrayList<LogicData> dataArrayList, LogicJSONObject jsonObject, string key)
        {
            if (dataArrayList.Count != 0)
            {
                do
                {
                    dataArrayList.Remove(0);
                } while (dataArrayList.Count != 0);
            }

            LogicJSONArray jsonArray = jsonObject.GetJSONArray(key);

            if (jsonArray != null)
            {
                dataArrayList.EnsureCapacity(jsonArray.Size());

                for (int i = 0; i < jsonArray.Size(); i++)
                {
                    LogicJSONNumber id = (LogicJSONNumber) jsonArray[i];

                    if (id != null && id.GetIntValue() != 0)
                    {
                        LogicData data = LogicDataTables.GetDataById(id.GetIntValue());

                        if (data != null)
                        {
                            dataArrayList.Add(data);
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Loads the specified data slot array list.
        /// </summary>
        private void LoadDataSlotArrayList(LogicArrayList<LogicDataSlot> dataSlotArrayList, LogicJSONObject jsonObject, string key)
        {
            if (dataSlotArrayList.Count != 0)
            {
                do
                {
                    dataSlotArrayList.Remove(0);
                } while (dataSlotArrayList.Count != 0);
            }

            LogicJSONArray jsonArray = jsonObject.GetJSONArray(key);

            if (jsonArray != null)
            {
                dataSlotArrayList.EnsureCapacity(jsonArray.Size());

                for (int i = 0; i < jsonArray.Size(); i++)
                {
                    LogicDataSlot slot = new LogicDataSlot(null, 0);
                    slot.ReadFromJSON((LogicJSONObject) jsonArray[i]);

                    if (slot.GetData() != null)
                    {
                        dataSlotArrayList.Add(slot);
                    }
                }
            }
        }

        /// <summary>
        ///     Loads the specified unit slot array list.
        /// </summary>
        private void LoadUnitSlotArrayList(LogicArrayList<LogicUnitSlot> unitSlotArrayList, LogicJSONObject jsonObject, string key)
        {
            if (unitSlotArrayList.Count != 0)
            {
                do
                {
                    unitSlotArrayList.Remove(0);
                } while (unitSlotArrayList.Count != 0);
            }

            LogicJSONArray jsonArray = jsonObject.GetJSONArray(key);

            if (jsonArray != null)
            {
                unitSlotArrayList.EnsureCapacity(jsonArray.Size());

                for (int i = 0; i < jsonArray.Size(); i++)
                {
                    LogicUnitSlot slot = new LogicUnitSlot(null, 0, 0);
                    slot.ReadFromJSON((LogicJSONObject)jsonArray[i]);

                    if (slot.GetData() != null)
                    {
                        unitSlotArrayList.Add(slot);
                    }
                }
            }
        }

        /// <summary>
        ///     Saves this instance to json.
        /// </summary>
        public LogicJSONObject Save()
        {
            LogicJSONObject jsonObject = new LogicJSONObject();

            jsonObject.Put("id_high", new LogicJSONNumber(this._id.GetHigherInt()));
            jsonObject.Put("id_low", new LogicJSONNumber(this._id.GetLowerInt()));

            jsonObject.Put("home_id_high", new LogicJSONNumber(this._homeId.GetHigherInt()));
            jsonObject.Put("home_id_low", new LogicJSONNumber(this._homeId.GetLowerInt()));

            jsonObject.Put("alliance_id_high", new LogicJSONNumber(this._allianceId.GetHigherInt()));
            jsonObject.Put("alliance_id_low", new LogicJSONNumber(this._allianceId.GetLowerInt()));

            jsonObject.Put("league_id_high", new LogicJSONNumber(this._leagueInstanceId.GetHigherInt()));
            jsonObject.Put("league_id_low", new LogicJSONNumber(this._leagueInstanceId.GetLowerInt()));

            jsonObject.Put("name", new LogicJSONString(this._name));
            jsonObject.Put("alliance_name", new LogicJSONString(this._allianceName));
            jsonObject.Put("facebook_id", new LogicJSONString(this._facebookId));

            LogicJSONObject legendLeagueTournamentEntry = new LogicJSONObject();
            LogicJSONObject legendLeagueTournamentEntryVillage2 = new LogicJSONObject();

            this._legendLeagueTournamentEntry.WriteToJSON(legendLeagueTournamentEntry);
            this._legendLeagueTournamentVillage2Entry.WriteToJSON(legendLeagueTournamentEntryVillage2);

            jsonObject.Put("legend_league_tournament", legendLeagueTournamentEntry);
            jsonObject.Put("legend_league_tournament_village2", legendLeagueTournamentEntryVillage2);

            jsonObject.Put("badge_id", new LogicJSONNumber(this._allianceBadgeId));
            jsonObject.Put("league_type", new LogicJSONNumber(this._leagueType));
            jsonObject.Put("diamonds", new LogicJSONNumber(this._diamonds));
            jsonObject.Put("free_diamonds", new LogicJSONNumber(this._freeDiamonds));
            jsonObject.Put("exp_level", new LogicJSONNumber(this._expLevel));
            jsonObject.Put("exp_points", new LogicJSONNumber(this._expPoints));
            jsonObject.Put("score", new LogicJSONNumber(this._score));
            jsonObject.Put("duel_score", new LogicJSONNumber(this._duelScore));
            jsonObject.Put("town_hall_level", new LogicJSONNumber(this._townHallLevel));
            jsonObject.Put("town_hall_level_village2", new LogicJSONNumber(this._townHallLevelVillage2));

            this.SaveDataSlotArrayList(this._resourceCap, jsonObject, "resource_caps");
            this.SaveDataSlotArrayList(this._resourceCount, jsonObject, "resources");
            this.SaveDataSlotArrayList(this._unitCount, jsonObject, "units");
            this.SaveDataSlotArrayList(this._spellCount, jsonObject, "spells");
            this.SaveDataSlotArrayList(this._unitUpgrade, jsonObject, "unit_upgrades");
            this.SaveDataSlotArrayList(this._spellUpgrade, jsonObject, "spell_upgrades");
            this.SaveUnitSlotArrayList(this._allianceUnitCount, jsonObject, "alliance_units");
            this.SaveDataSlotArrayList(this._achievementProgress, jsonObject, "achievements");
            this.SaveDataArrayList(this._achievementRewardClaimed, jsonObject, "achievements_claimed");
            this.SaveDataArrayList(this._missionCompleted, jsonObject, "missions");
            this.SaveDataSlotArrayList(this._npcStars, jsonObject, "npc_stars");
            this.SaveDataSlotArrayList(this._lootedNpcGold, jsonObject, "npc_looted_gold");
            this.SaveDataSlotArrayList(this._lootedNpcElixir, jsonObject, "npc_looted_elixir");

            return jsonObject;
        }

        /// <summary>
        ///     Saves the specified data slot array list to json.
        /// </summary>
        private void SaveDataSlotArrayList(LogicArrayList<LogicDataSlot> dataSlotArrayList, LogicJSONObject jsonObject, string key)
        {
            LogicJSONArray jsonArray = new LogicJSONArray(dataSlotArrayList.Count);

            for (int i = 0; i < dataSlotArrayList.Count; i++)
            {
                LogicJSONObject slotObject = new LogicJSONObject();
                dataSlotArrayList[i].WriteToJSON(slotObject);
                jsonArray.Add(slotObject);
            }

            jsonObject.Put(key, jsonArray);
        }

        /// <summary>
        ///     Saves the specified unit slot array list to json.
        /// </summary>
        private void SaveUnitSlotArrayList(LogicArrayList<LogicUnitSlot> unitSlotArrayList, LogicJSONObject jsonObject, string key)
        {
            LogicJSONArray jsonArray = new LogicJSONArray(unitSlotArrayList.Count);

            for (int i = 0; i < unitSlotArrayList.Count; i++)
            {
                LogicJSONObject slotObject = new LogicJSONObject();
                unitSlotArrayList[i].WriteToJSON(slotObject);
                jsonArray.Add(slotObject);
            }

            jsonObject.Put(key, jsonArray);
        }

        /// <summary>
        ///     Saves the specified data array list to json.
        /// </summary>
        private void SaveDataArrayList(LogicArrayList<LogicData> dataArrayList, LogicJSONObject jsonObject, string key)
        {
            LogicJSONArray jsonArray = new LogicJSONArray(dataArrayList.Count);

            for (int i = 0; i < dataArrayList.Count; i++)
            {
                if (dataArrayList[i] != null)
                {
                    jsonArray.Add(new LogicJSONNumber(dataArrayList[i].GetGlobalID()));
                }
            }

            jsonObject.Put(key, jsonArray);
        }
    }
}