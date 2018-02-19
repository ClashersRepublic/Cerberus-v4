namespace ClashersRepublic.Magic.Logic.Message.Home
{
    using ClashersRepublic.Magic.Logic.Command;
    using ClashersRepublic.Magic.Titan.Message;
    using ClashersRepublic.Magic.Titan.Util;

    public class EndClientTurnMessage : PiranhaMessage
    {
        private int _subTick;
        private int _checksum;

        private LogicArrayList<LogicCommand> _commands;

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

            this._subTick = this.Stream.ReadInt();
            this._checksum = this.Stream.ReadInt();

            int arraySize = this.Stream.ReadInt();

            if (arraySize <= 512)
            {
                if (arraySize > 0)
                {
                    this._commands = new LogicArrayList<LogicCommand>(arraySize);

                    do
                    {
                        LogicCommand command = LogicCommandManager.DecodeCommand(this.Stream);

                        if (command == null)
                        {
                            break;
                        }

                        this._commands.Add(command);
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

            this.Stream.WriteInt(this._subTick);
            this.Stream.WriteInt(this._checksum);

            if (this._commands != null)
            {
                this.Stream.WriteInt(this._commands.Count);

                for (int i = 0; i < this._commands.Count; i++)
                {
                    LogicCommandManager.EncodeCommand(this.Stream, this._commands[i]);
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
            base.Destruct();
            this._commands = null;
        }

        /// <summary>
        ///     Gets the subtick.
        /// </summary>
        public int GetSubTick()
        {
            return this._subTick;
        }

        /// <summary>
        ///     Sets the subtick.
        /// </summary>
        public void SetSubTick(int value)
        {
            this._subTick = value;
        }

        /// <summary>
        ///     Gets the checksum.
        /// </summary>
        public int GetChecksum()
        {
            return this._checksum;
        }

        /// <summary>
        ///     Sets the checksum.
        /// </summary>
        public void SetChecksum(int value)
        {
            this._checksum = value;
        }

        /// <summary>
        ///     Gets all commands.
        /// </summary>
        public LogicArrayList<LogicCommand> GetCommands()
        {
            return this._commands;
        }

        /// <summary>
        ///     Sets all commands.
        /// </summary>
        public void SetCommands(LogicArrayList<LogicCommand> commands)
        {
            this._commands = commands;
        }
    }
}