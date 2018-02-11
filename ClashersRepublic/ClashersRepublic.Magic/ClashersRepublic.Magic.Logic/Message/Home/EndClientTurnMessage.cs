namespace ClashersRepublic.Magic.Logic.Message.Home
{
    using ClashersRepublic.Magic.Logic.Command;
    using ClashersRepublic.Magic.Titan.Message;
    using ClashersRepublic.Magic.Titan.Util;

    public class EndClientTurnMessage : PiranhaMessage
    {
        public int Subtick;
        public int Checksum;

        public LogicArrayList<LogicCommand> Commands;

        /// <summary>
        ///     Initializes a new instance of the <see cref="EndClientTurnMessage" /> class.
        /// </summary>
        public EndClientTurnMessage() : this(0)
        {
            // EndClientTurnMessage.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="EndClientTurnMessage" /> class.
        /// </summary>
        public EndClientTurnMessage(short messageVersion) : base(messageVersion)
        {
            // EndClientTurnMessage.
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();

            this.Subtick = this.Stream.ReadInt();
            this.Checksum = this.Stream.ReadInt();

            int arraySize = this.Stream.ReadInt();

            if (arraySize <= 512)
            {
                if (arraySize > 0)
                {
                    this.Commands = new LogicArrayList<LogicCommand>(arraySize);

                    do
                    {
                        LogicCommand command = LogicCommandManager.DecodeCommand(this.Stream);

                        if (command == null)
                        {
                            break;
                        }

                        this.Commands.Add(command);
                    } while (--arraySize != 0);
                }
            }
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();

            this.Stream.WriteInt(this.Subtick);
            this.Stream.WriteInt(this.Checksum);

            if (this.Commands != null)
            {
                this.Stream.WriteInt(this.Commands.Count);

                for (int i = 0; i < this.Commands.Count; i++)
                {
                    LogicCommandManager.EncodeCommand(this.Stream, this.Commands[i]);
                }
            }
            else
            {
                this.Stream.WriteInt(-1);
            }
        }

        /// <summary>
        ///     Gets the message type of this message.
        /// </summary>
        public override short GetMessageType()
        {
            return 14102;
        }

        /// <summary>
        ///     Gets the service node type of this message.
        /// </summary>
        public override int GetServiceNodeType()
        {
            return 10;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            this.Commands = null;
        }
    }
}