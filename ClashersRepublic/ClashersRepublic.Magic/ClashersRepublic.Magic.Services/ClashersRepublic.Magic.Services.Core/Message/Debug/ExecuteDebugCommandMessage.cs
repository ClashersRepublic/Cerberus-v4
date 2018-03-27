namespace ClashersRepublic.Magic.Services.Core.Message.Debug
{
    public class ExecuteDebugCommandMessage : NetMessage
    {
        private DebugCommand _debugCommand;

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();

            if (this._debugCommand != null)
            {
                this._debugCommand.Destruct();
                this._debugCommand = null;
            }
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
            this.Stream.WriteVInt(this._debugCommand.GetCommandType());
            this._debugCommand.Encode(this.Stream);
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
            this._debugCommand = DebugCommandFactory.CreateDebugCommandByType(this.Stream.ReadVInt());

            if (this._debugCommand != null)
            {
                this._debugCommand.Decode(this.Stream);
            }
        }

        /// <summary>
        ///     Gets the message type of this instance.
        /// </summary>
        public override int GetMessageType()
        {
            return 10600;
        }

        /// <summary>
        ///     Removes the <see cref="DebugCommand"/> instance.
        /// </summary>
        public DebugCommand RemoveDebugCommand()
        {
            DebugCommand tmp = this._debugCommand;
            this._debugCommand = null;
            return tmp;
        }

        /// <summary>
        ///     Sets the <see cref="DebugCommand"/> instance.
        /// </summary>
        public void SetDebugCommand(DebugCommand command)
        {
            this._debugCommand = command;
        }
    }
}