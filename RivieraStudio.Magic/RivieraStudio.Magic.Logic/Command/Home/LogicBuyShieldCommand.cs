namespace RivieraStudio.Magic.Logic.Command.Home
{
    using RivieraStudio.Magic.Logic.Avatar;
    using RivieraStudio.Magic.Logic.Data;
    using RivieraStudio.Magic.Logic.Helper;
    using RivieraStudio.Magic.Logic.Level;
    using RivieraStudio.Magic.Logic.Mode;

    using RivieraStudio.Magic.Titan.DataStream;
    using RivieraStudio.Magic.Titan.Math;

    public sealed class LogicBuyShieldCommand : LogicCommand
    {
        private LogicShieldData _shieldData;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicBuyShieldCommand" /> class.
        /// </summary>
        public LogicBuyShieldCommand()
        {
            // LogicBuyShieldCommand.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicBuyShieldCommand" /> class.
        /// </summary>
        public LogicBuyShieldCommand(LogicShieldData data)
        {
            this._shieldData = data;
        }

        /// <summary>
        ///     Decodes this instnace.
        /// </summary>
        public override void Decode(ByteStream stream)
        {
            this._shieldData = (LogicShieldData) stream.ReadDataReference(19);
            base.Decode(stream);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode(ChecksumEncoder encoder)
        {
            encoder.WriteDataReference(this._shieldData);
            base.Encode(encoder);
        }

        /// <summary>
        ///     Gets the command type of this instance.
        /// </summary>
        public override int GetCommandType()
        {
            return 522;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();
            this._shieldData = null;
        }

        /// <summary>
        ///     Executes this command.
        /// </summary>
        public override int Execute(LogicLevel level)
        {
            if (this._shieldData != null)
            {
                int cooldownSecs = level.GetCooldownManager().GetCooldownSeconds(this._shieldData.GetGlobalID());

                if (cooldownSecs <= 0)
                {
                    LogicClientAvatar playerAvatar = level.GetPlayerAvatar();

                    if (this._shieldData.GetScoreLimit() > playerAvatar.GetScore() || this._shieldData.GetScoreLimit() <= 0)
                    {
                        if (playerAvatar.HasEnoughDiamonds(this._shieldData.GetDiamondsCost(), true, level))
                        {
                            LogicGameMode gameMode = level.GetGameMode();

                            playerAvatar.UseDiamonds(this._shieldData.GetDiamondsCost());

                            int shieldTime = gameMode.GetShieldRemainingSeconds() + this._shieldData.TimeH * 3600;
                            int guardTime = gameMode.GetGuardRemainingSeconds();
                            int maintenanceTime = 0;

                            if (this._shieldData.TimeH <= 0)
                            {
                                if (shieldTime > 0)
                                {
                                    return -2;
                                }

                                guardTime += this._shieldData.GuardTimeH * 3600;
                            }
                            else
                            {
                                LogicLeagueData leagueData = playerAvatar.GetLeagueTypeData();

                                if (playerAvatar.GetShieldCostAmount() != 0)
                                {
                                    playerAvatar.SetShieldCostAmount(0);
                                }

                                guardTime += leagueData.VillageGuardInMins * 60;
                            }

                            if (shieldTime <= 0)
                            {
                                maintenanceTime = LogicMath.Min(LogicDataTables.GetGlobals().GetPersonalBreakLimitSeconds() + this._shieldData.GuardTimeH * 3600,
                                                                gameMode.GetMaintenanceRemainingSeconds() + this._shieldData.GuardTimeH * 3600);
                            }
                            else
                            {
                                maintenanceTime = shieldTime + LogicDataTables.GetGlobals().GetPersonalBreakLimitSeconds();
                            }

                            gameMode.SetMaintenanceRemainingSeconds(maintenanceTime);
                            gameMode.SetShieldRemainingSeconds(shieldTime);
                            gameMode.SetGuardRemainingSeconds(guardTime);

                            level.GetCooldownManager().AddCooldown(this._shieldData.GetGlobalID(), this._shieldData.GetCooldownSecs());

                            return 0;
                        }
                    }
                }
            }

            return -1;
        }
    }
}