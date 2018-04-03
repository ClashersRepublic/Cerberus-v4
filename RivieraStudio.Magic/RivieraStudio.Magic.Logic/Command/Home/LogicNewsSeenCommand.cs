namespace RivieraStudio.Magic.Logic.Command.Home
{
    using RivieraStudio.Magic.Logic.Level;
    using RivieraStudio.Magic.Titan.DataStream;

    public sealed class LogicNewsSeenCommand : LogicCommand
    {
        private int _lastSeenNews;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicNewsSeenCommand"/> class.
        /// </summary>
        public LogicNewsSeenCommand()
        {
            // LogicNewsSeenCommand.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicNewsSeenCommand"/> class.
        /// </summary>
        public LogicNewsSeenCommand(int lastSeenNews)
        {
            this._lastSeenNews = lastSeenNews;
        }

        /// <summary>
        ///     Decodes this instnace.
        /// </summary>
        public override void Decode(ByteStream stream)
        {
            this._lastSeenNews = stream.ReadInt();
            base.Decode(stream);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode(ChecksumEncoder encoder)
        {
            encoder.WriteInt(this._lastSeenNews);
            base.Encode(encoder);
        }

        /// <summary>
        ///     Gets the command type of this instance.
        /// </summary>
        public override int GetCommandType()
        {
            return 539;
        }

        /// <summary>
        ///     Executes this instance.
        /// </summary>
        public override int Execute(LogicLevel level)
        {
            level.SetLastSeenNews(this._lastSeenNews);
            return 0;
        }
    }
}