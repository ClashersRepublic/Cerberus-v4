namespace ClashersRepublic.Magic.Logic.Command.Battle
{
    using System;
    using ClashersRepublic.Magic.Logic.Avatar;
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.GameObject;
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Logic.Level;
    using ClashersRepublic.Magic.Titan.DataStream;
    using ClashersRepublic.Magic.Titan.Math;

    public sealed class LogicPlaceAttackerCommand : LogicCommand
    {
        private int _x;
        private int _y;
        private LogicCharacterData _data;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicPlaceAttackerCommand" /> class.
        /// </summary>
        public LogicPlaceAttackerCommand()
        {
            // LogicPlaceAttackerCommand.
        }
        
        /// <summary>
        ///     Decodes this instnace.
        /// </summary>
        public override void Decode(ByteStream stream)
        {
            this._x = stream.ReadInt();
            this._y = stream.ReadInt();
            this._data = (LogicCharacterData) stream.ReadDataReference(3);

            base.Decode(stream);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode(ChecksumEncoder encoder)
        {
            encoder.WriteInt(this._x);
            encoder.WriteInt(this._y);
            encoder.WriteDataReference(this._data);

            base.Encode(encoder);
        }

        /// <summary>
        ///     Gets the command type of this instance.
        /// </summary>
        public override int GetCommandType()
        {
            return 700;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();
            this._data = null;
        }

        /// <summary>
        ///     Executes this instance.
        /// </summary>
        public override int Execute(LogicLevel level)
        {
            if (true)
            {
                int tileX = this._x >> 9;
                int tileY = this._y >> 9;

                if (level.GetTileMap().GetTile(tileX, tileY) != null)
                {
                    if (level.GetTileMap().IsPassablePathFinder(this._x >> 8, this._y >> 8))
                    {
                        if (level.GetTileMap().IsValidAttackPos(tileX, tileY))
                        {
                            LogicClientAvatar playerAvatar = level.GetPlayerAvatar();

                            if (playerAvatar != null)
                            {
                                int unitCount = level.GetVillageType() == 1 ? playerAvatar.GetUnitCountVillage2(this._data) : playerAvatar.GetUnitCount(this._data);

                                if (unitCount > 0)
                                {
                                    if (level.GetBattleLog() != null)
                                    {
                                        // ?
                                    }

                                    if (level.GetGameMode().IsInAttackPreparationMode())
                                    {
                                        level.GetGameMode().EndAttackPreparation();
                                    }

                                    LogicCharacter character = LogicPlaceAttackerCommand.PlaceAttacker(playerAvatar, this._data, level, this._x, this._y);

                                    if (character != null)
                                    {
                                        // TODO: Finish this.
                                    }

                                    return 0;
                                }

                                return -7;
                            }

                            return -5;
                        }

                        return -4;
                    }

                    return -2;
                }

                return -3;
            }

            return -1;
        }

        /// <summary>
        ///     Places the specified attacker.
        /// </summary>
        public static LogicCharacter PlaceAttacker(LogicAvatar avatar, LogicCharacterData characterData, LogicLevel level, int x, int y)
        {
            if (level.GetVillageType() == 1)
            {
                avatar.CommodityCountChangeHelper(7, characterData, -1);
            }
            else
            {
                avatar.CommodityCountChangeHelper(0, characterData, -1);
            }

            LogicCharacter character = (LogicCharacter) LogicGameObjectFactory.CreateGameObject(characterData, level, level.GetVillageType());
            Int32 upgradeLevel = avatar.GetUnitUpgradeLevel(characterData);

            if (level.GetMissionManager().GetMissionByCategory(2) != null && level.GetVillageType() == 1 && level.GetHomeOwnerAvatar() != null)
            {
                LogicAvatar homeOwnerAvatar = level.GetHomeOwnerAvatar();

                if (homeOwnerAvatar.IsNpcAvatar())
                {
                    upgradeLevel = LogicMath.Clamp(LogicDataTables.GetGlobals().GetVillage2StartUnitLevel(), 0, characterData.GetUpgradeLevelCount());
                }
            }

            character.SetUpgradeLevel(upgradeLevel);
            character.SetInitialPosition(x, y);

            return character;
        }
    }
}