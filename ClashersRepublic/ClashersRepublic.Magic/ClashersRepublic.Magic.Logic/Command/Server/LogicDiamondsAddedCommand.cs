namespace ClashersRepublic.Magic.Logic.Command.Server
{
    using ClashersRepublic.Magic.Logic.Avatar;
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.Level;
    using ClashersRepublic.Magic.Titan.DataStream;

    public class LogicDiamondsAddedCommand : LogicServerCommand
    {
        private bool _freeDiamonds;
        private bool _bundlePackage;

        private int _source;
        private int _diamondsCount;
        private int _billingPackageId;

        private string _transactionId;

        /// <summary>
        ///     Sets the command data.
        /// </summary>
        public void SetData(bool free, int diamondCount, int billingPackage, bool bundlePackage, int source, string transactionId)
        {
            this._freeDiamonds = free;
            this._diamondsCount = diamondCount;
            this._billingPackageId = billingPackage;
            this._bundlePackage = bundlePackage;
            this._source = source;
            this._transactionId = transactionId;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();
            this._transactionId = null;
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode(ByteStream stream)
        {
            this._freeDiamonds = stream.ReadBoolean();
            this._diamondsCount = stream.ReadInt();
            this._billingPackageId = stream.ReadInt();
            this._bundlePackage = stream.ReadBoolean();
            this._source = stream.ReadInt();
            this._transactionId = stream.ReadString(900000);

            base.Decode(stream);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode(ChecksumEncoder encoder)
        {
            encoder.WriteBoolean(this._freeDiamonds);
            encoder.WriteInt(this._diamondsCount);
            encoder.WriteInt(this._billingPackageId);
            encoder.WriteBoolean(this._bundlePackage);
            encoder.WriteInt(this._source);
            encoder.WriteString(this._transactionId);

            base.Encode(encoder);
        }

        /// <summary>
        ///     Executes this command.
        /// </summary>
        public override int Execute(LogicLevel level)
        {
            LogicClientAvatar playerAvatar = level.GetPlayerAvatar();

            if (playerAvatar != null)
            {
                if (this._source == 1)
                {
                    // listener.
                }

                playerAvatar.SetDiamonds(playerAvatar.GetDiamonds() + this._diamondsCount);

                if (this._freeDiamonds)
                {
                    int freeDiamonds = playerAvatar.GetFreeDiamonds();

                    if (this._diamondsCount < 0)
                    {
                        if (freeDiamonds - this._diamondsCount >= 0 && playerAvatar.GetDiamonds() != freeDiamonds)
                        {
                            playerAvatar.SetFreeDiamonds(freeDiamonds + this._diamondsCount);
                        }
                    }
                }
                else
                {
                    if (this._billingPackageId > 0)
                    {
                        LogicBillingPackageData billingPackageData = (LogicBillingPackageData) LogicDataTables.GetDataById(this._billingPackageId, 21);

                        if (billingPackageData != null)
                        {
                            if (billingPackageData.RED && !this._bundlePackage)
                            {
                                int redPackageState = playerAvatar.GetRedPackageState();
                                int newRedPackageState = redPackageState | 0x10;

                                if ((redPackageState & 3) != 3)
                                {
                                    newRedPackageState = (int) (newRedPackageState & 0xFFFFFFFC);
                                }

                                playerAvatar.SetRedPackageState(newRedPackageState);
                            }
                        }
                    }

                    level.GetGameListener().DiamondsBought();
                    playerAvatar.AddCumulativePurchasedDiamonds(this._diamondsCount);
                }

                return 0;
            }

            return -1;
        }

        /// <summary>
        ///     Gets the command type.
        /// </summary>
        public override int GetCommandType()
        {
            return 7;
        }
    }
}