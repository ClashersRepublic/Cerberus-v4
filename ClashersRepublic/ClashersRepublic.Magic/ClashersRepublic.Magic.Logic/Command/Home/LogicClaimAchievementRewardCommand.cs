namespace ClashersRepublic.Magic.Logic.Command.Home
{
    using ClashersRepublic.Magic.Logic.Avatar;
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.GameObject;
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Logic.Level;

    using ClashersRepublic.Magic.Titan.DataStream;

    public sealed class LogicClaimAchievementRewardCommand : LogicCommand
    {
        private LogicAchievementData _achievementData;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicClaimAchievementRewardCommand" /> class.
        /// </summary>
        public LogicClaimAchievementRewardCommand()
        {
            // LogicClaimAchievementRewardCommand.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicClaimAchievementRewardCommand" /> class.
        /// </summary>
        public LogicClaimAchievementRewardCommand(LogicAchievementData achievementData)
        {
            this._achievementData = achievementData;
        }

        /// <summary>
        ///     Decodes this instnace.
        /// </summary>
        public override void Decode(ByteStream stream)
        {
            this._achievementData = (LogicAchievementData) stream.ReadDataReference(22);
            base.Decode(stream);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode(ChecksumEncoder encoder)
        {
            encoder.WriteDataReference(this._achievementData);
            base.Encode(encoder);
        }

        /// <summary>
        ///     Gets the command type of this instance.
        /// </summary>
        public override int GetCommandType()
        {
            return 523;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();
            this._achievementData = null;
        }

        /// <summary>
        ///     Executes this command.
        /// </summary>
        public override int Execute(LogicLevel level)
        {
            LogicClientAvatar playerAvatar = level.GetPlayerAvatar();

            if (playerAvatar != null && this._achievementData != null)
            {
                if (playerAvatar.IsAchievementCompleted(this._achievementData) && !playerAvatar.IsAchievementRewardClaimed(this._achievementData))
                {
                    playerAvatar.XpGainHelper(this._achievementData.GetExpReward());

                    if (this._achievementData.GetDiamondReward() > 0)
                    {
                        playerAvatar.SetDiamonds(playerAvatar.GetDiamonds() + this._achievementData.GetDiamondReward());
                        playerAvatar.SetFreeDiamonds(playerAvatar.GetFreeDiamonds() + this._achievementData.GetDiamondReward());
                    }

                    playerAvatar.SetAchievementRewardClaimed(this._achievementData, true);
                    playerAvatar.GetChangeListener().CommodityCountChanged(1, this._achievementData, 1);

                    return 0;
                }
            }

            return -1;
        }
    }
}