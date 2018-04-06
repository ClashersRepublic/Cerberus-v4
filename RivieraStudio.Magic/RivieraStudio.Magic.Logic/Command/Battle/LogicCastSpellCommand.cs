namespace RivieraStudio.Magic.Logic.Command.Battle
{
    using System;
    using RivieraStudio.Magic.Logic.Avatar;
    using RivieraStudio.Magic.Logic.Data;
    using RivieraStudio.Magic.Logic.GameObject;
    using RivieraStudio.Magic.Logic.Helper;
    using RivieraStudio.Magic.Logic.Level;
    using RivieraStudio.Magic.Titan.DataStream;
    using RivieraStudio.Magic.Titan.Debug;
    using RivieraStudio.Magic.Titan.Math;

    public sealed class LogicCastSpellCommand : LogicCommand
    {
        private int _x;
        private int _y;
        private int _upgLevel;
        private bool _allianceSpell;

        private LogicSpellData _data;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicCastSpellCommand" /> class.
        /// </summary>
        public LogicCastSpellCommand()
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
            this._data = (LogicSpellData) stream.ReadDataReference(25);
            this._allianceSpell = stream.ReadBoolean();
            this._upgLevel = stream.ReadInt();

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
            encoder.WriteBoolean(this._allianceSpell);
            encoder.WriteInt(this._upgLevel);

            base.Encode(encoder);
        }

        /// <summary>
        ///     Gets the command type of this instance.
        /// </summary>
        public override int GetCommandType()
        {
            return 704;
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
                    if (level.GetVillageType() == 0)
                    {
                        LogicClientAvatar playerAvatar = level.GetPlayerAvatar();

                        if (playerAvatar != null)
                        {
                            int unitCount = this._allianceSpell ? playerAvatar.GetAllianceUnitCount(this._data, this._upgLevel) : playerAvatar.GetUnitCount(this._data);

                            if (unitCount > 0)
                            {
                                if(level.GetBattleLog() != null)
                                {
                                    // ?
                                }

                                if (level.GetGameMode().IsInAttackPreparationMode())
                                {
                                    level.GetGameMode().EndAttackPreparation();
                                }

                                LogicCastSpellCommand.CastSpell(playerAvatar, this._data, this._allianceSpell, this._upgLevel, level, this._x, this._y);

                                return 0;
                            }
                        }

                        return -3;
                    }
                    else
                    {
                        Debugger.Error("not available for village");
                    }

                    return -23;
                }

                return -3;
            }

            return -1;
        }

        /// <summary>
        ///     Casts the specified spell.
        /// </summary>
        public static LogicSpell CastSpell(LogicAvatar avatar, LogicSpellData spellData, bool allianceSpell, int upgLevel, LogicLevel level, int x, int y)
        {
            if (allianceSpell)
            {
                avatar.RemoveAllianceUnit(spellData, upgLevel);
            }
            else
            {
                avatar.CommodityCountChangeHelper(0, spellData, -1);
            }

            LogicSpell spell = (LogicSpell) LogicGameObjectFactory.CreateGameObject(spellData, level, level.GetVillageType());
            spell.SetUpgradeLevel(upgLevel);
            return spell;
        }
    }
}