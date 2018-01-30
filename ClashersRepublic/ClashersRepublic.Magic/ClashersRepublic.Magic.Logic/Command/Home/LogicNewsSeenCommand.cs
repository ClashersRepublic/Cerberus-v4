namespace ClashersRepublic.Magic.Logic.Command.Home
{
    using ClashersRepublic.Magic.Titan.DataStream;

    public sealed class LogicNewsSeenCommand : LogicCommand
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicNewsSeenCommand"/> class.
        /// </summary>
        public LogicNewsSeenCommand()
        {
            // LogicNewsSeenCommand.
        }

        /// <summary>
        ///     Decodes this instnace.
        /// </summary>
        public override void Decode(ByteStream stream)
        {
            stream.ReadInt();
            base.Decode(stream);
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode(ChecksumEncoder encoder)
        {
            encoder.WriteInt(0);
            base.Encode(encoder);
        }

        /// <summary>
        ///     Gets the command type of this instance.
        /// </summary>
        public override int GetCommandType()
        {
            return 539;
        }
    }
}