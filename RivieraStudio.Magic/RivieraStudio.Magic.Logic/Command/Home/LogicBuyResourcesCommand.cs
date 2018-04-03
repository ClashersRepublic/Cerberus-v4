namespace RivieraStudio.Magic.Logic.Command.Home
{
    using RivieraStudio.Magic.Logic.Avatar;
    using RivieraStudio.Magic.Logic.Data;
    using RivieraStudio.Magic.Logic.Helper;
    using RivieraStudio.Magic.Logic.Level;
    using RivieraStudio.Magic.Logic.Util;
    using RivieraStudio.Magic.Titan.DataStream;
    using RivieraStudio.Magic.Titan.Debug;

    public sealed class LogicBuyResourcesCommand : LogicCommand
    {
        private LogicCommand _command;
        private LogicResourceData _resourceData;
        private LogicResourceData _resource2Data;
        private int _resourceCount;
        private int _resource2Count;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicBuyResourcesCommand" /> class.
        /// </summary>
        public LogicBuyResourcesCommand()
        {
            // LogicBuyResourcesCommand.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicBuyResourcesCommand" /> class.
        /// </summary>
        public LogicBuyResourcesCommand(LogicResourceData data, int resourceCount, LogicResourceData resource2Data, int resource2Count, LogicCommand resourceCommand)
        {
            this._resourceData = data;
            this._resource2Data = resource2Data;
            this._command = resourceCommand;
            this._resourceCount = resourceCount;
            this._resource2Count = resource2Count;
        }

        /// <summary>
        ///     Decodes this instnace.
        /// </summary>
        public override void Decode(ByteStream stream)
        {
            this._resourceCount = stream.ReadInt();
            this._resourceData = (LogicResourceData) stream.ReadDataReference(2);
            this._resource2Count = stream.ReadInt();

            if (this._resource2Count > 0)
            {
                this._resource2Data = (LogicResourceData) stream.ReadDataReference(2);
            }

            if (stream.ReadBoolean())
            {
                this._command = LogicCommandManager.DecodeCommand(stream);
            }

            base.Decode(stream);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode(ChecksumEncoder encoder)
        {
            encoder.WriteInt(this._resourceCount);
            encoder.WriteDataReference(this._resourceData);
            encoder.WriteInt(this._resource2Count);

            if (this._resource2Count > 0)
            {
                encoder.WriteDataReference(this._resource2Data);
            }

            if (this._command != null)
            {
                encoder.WriteBoolean(true);
                LogicCommandManager.EncodeCommand(encoder, this._command);
            }
            else
            {
                encoder.WriteBoolean(false);
            }

            base.Encode(encoder);
        }

        /// <summary>
        ///     Gets the command type of this instance.
        /// </summary>
        public override int GetCommandType()
        {
            return 518;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();

            if (this._command != null)
            {
                this._command.Destruct();
                this._command = null;
            }

            this._resourceData = null;
            this._resource2Data = null;
        }

        /// <summary>
        ///     Executes this command.
        /// </summary>
        public override int Execute(LogicLevel level)
        {
            if (this._resourceData != null && this._resourceCount > 0 && !this._resourceData.PremiumCurrency)
            {
                LogicClientAvatar playerAvatar = level.GetPlayerAvatar();

                if (this._resource2Data != null && this._resource2Count > 0)
                {
                    int resourceCost = LogicGamePlayUtil.GetResourceDiamondCost(this._resourceCount, this._resourceData);
                    int resourceCost2 = LogicGamePlayUtil.GetResourceDiamondCost(this._resource2Count, this._resource2Data);

                    if (playerAvatar.GetUnusedResourceCap(this._resourceData) >= this._resourceCount &&
                        playerAvatar.GetUnusedResourceCap(this._resource2Data) >= this._resource2Count)
                    {
                        if (playerAvatar.HasEnoughDiamonds(resourceCost + resourceCost2, true, level))
                        {
                            playerAvatar.UseDiamonds(resourceCost + resourceCost2);
                            playerAvatar.CommodityCountChangeHelper(0, this._resourceData, this._resourceCount);
                            playerAvatar.CommodityCountChangeHelper(0, this._resource2Data, this._resource2Count);

                            Debugger.Log("LogicBuyResourcesCommand::execute buy resources: " + (resourceCost + resourceCost2));
                            
                            if (this._command != null)
                            {
                                int cmdType = this._command.GetCommandType();

                                if (cmdType < 1000)
                                {
                                    if (cmdType >= 500 && cmdType < 700)
                                    {
                                        this._command.Execute(level);
                                    }
                                }
                            }

                            return 0;
                        }
                    }
                }
                else
                {
                    int resourceCost = LogicGamePlayUtil.GetResourceDiamondCost(this._resourceCount, this._resourceData);

                    if (playerAvatar.GetUnusedResourceCap(this._resourceData) >= this._resourceCount)
                    {
                        if (playerAvatar.HasEnoughDiamonds(resourceCost, true, level))
                        {
                            playerAvatar.UseDiamonds(resourceCost);
                            playerAvatar.CommodityCountChangeHelper(0, this._resourceData, this._resourceCount);

                            Debugger.Log("LogicBuyResourcesCommand::execute buy resources: " + resourceCost);

                            if (this._command != null)
                            {
                                int cmdType = this._command.GetCommandType();

                                if (cmdType < 1000)
                                {
                                    if (cmdType >= 500 && cmdType < 700)
                                    {
                                        this._command.Execute(level);
                                    }
                                }
                            }

                            return 0;
                        }
                    }
                }
            }

            return -1;
        }
    }
}