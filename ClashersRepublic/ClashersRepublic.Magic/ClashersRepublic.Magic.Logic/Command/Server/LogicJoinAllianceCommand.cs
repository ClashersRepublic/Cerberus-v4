namespace ClashersRepublic.Magic.Logic.Command.Server
{
    using ClashersRepublic.Magic.Logic.Avatar;
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.Level;
    using ClashersRepublic.Magic.Logic.Mode;
    using ClashersRepublic.Magic.Titan.DataStream;
    using ClashersRepublic.Magic.Titan.Math;

    public class LogicJoinAllianceCommand : LogicServerCommand
    {
        private LogicLong _allianceId;
        private string _allianceName;
        private int _allianceBadgeId;
        private int _allianceExpLevel;
        private bool _allianceCreate;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicJoinAllianceCommand"/> class.
        /// </summary>
        public LogicJoinAllianceCommand()
        {
            // LogicJoinAllianceCommand.
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();

            this._allianceId = null;
            this._allianceName = null;
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode(ByteStream stream)
        {
            this._allianceId = stream.ReadLong();
            this._allianceName = stream.ReadString(900000);
            this._allianceBadgeId = stream.ReadInt();
            this._allianceCreate = stream.ReadBoolean();
            this._allianceExpLevel = stream.ReadInt();

            base.Decode(stream);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode(ChecksumEncoder encoder)
        {
            encoder.WriteLong(this._allianceId);
            encoder.WriteString(this._allianceName);
            encoder.WriteInt(this._allianceBadgeId);
            encoder.WriteBoolean(this._allianceCreate);
            encoder.WriteInt(this._allianceExpLevel);

            base.Encode(encoder);
        }

        /// <summary>
        ///     Executes this command.
        /// </summary>
        public override int Execute(LogicLevel level)
        {
            LogicClientAvatar playerAvatar = (LogicClientAvatar) level.GetPlayerAvatar();

            if (playerAvatar != null)
            {
                if (this._allianceCreate)
                {
                    LogicGlobals globals = LogicDataTables.GetGlobals();
                    LogicResourceData resource = globals.GetAllianceCreateResourceData();

                    int removeCount = LogicMath.Min(globals.GetAllianceCreateCost(), playerAvatar.GetResourceCount(resource));
                    
                    playerAvatar.CommodityCountChangeHelper(0, resource, -removeCount);
                }

                playerAvatar.SetAllianceId(this._allianceId.Clone());
                playerAvatar.SetAllianceName(this._allianceName);
                playerAvatar.SetAllianceBadge(this._allianceBadgeId);
                playerAvatar.SetAllianceExpLevel(this._allianceExpLevel);
                playerAvatar.SetAllianceRole(this._allianceCreate ? 2 : 1);

                LogicGameListener gameListener = level.GetGameListener();

                if (gameListener != null)
                {
                    if (this._allianceCreate)
                    {
                        gameListener.AllianceCreated();
                    }
                    else
                    {
                        gameListener.AllianceJoined();
                    }
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
            return 1;
        }
    }
}