namespace LineageSoft.Magic.Services.Core.Message.Admin
{
    public class ExecuteAdminCommandMessage : NetMessage
    {
        private AdminCommand _adminCommand;

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();

            if (this._adminCommand != null)
            {
                this._adminCommand.Destruct();
                this._adminCommand = null;
            }
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();
            this.Stream.WriteVInt(this._adminCommand.GetCommandType());
            this._adminCommand.Encode(this.Stream);
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();
            this._adminCommand = AdminCommandFactory.CreateDebugCommandByType(this.Stream.ReadVInt());

            if (this._adminCommand != null)
            {
                this._adminCommand.Decode(this.Stream);
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
        ///     Removes the <see cref="AdminCommand"/> instance.
        /// </summary>
        public AdminCommand RemoveDebugCommand()
        {
            AdminCommand tmp = this._adminCommand;
            this._adminCommand = null;
            return tmp;
        }

        /// <summary>
        ///     Sets the <see cref="AdminCommand"/> instance.
        /// </summary>
        public void SetDebugCommand(AdminCommand command)
        {
            this._adminCommand = command;
        }
    }
}