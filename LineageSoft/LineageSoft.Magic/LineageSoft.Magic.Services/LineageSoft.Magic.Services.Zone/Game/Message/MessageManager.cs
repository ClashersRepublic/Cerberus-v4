namespace LineageSoft.Magic.Services.Zone.Game.Message
{
    using LineageSoft.Magic.Logic.Data;
    using LineageSoft.Magic.Logic.Message.Home;

    using LineageSoft.Magic.Services.Zone.Game.Mode;

    using LineageSoft.Magic.Titan.Message;

    internal class MessageManager
    {
        private GameMode _gameMode;

        /// <summary>
        ///     Initializes a new instance of the <see cref="MessageManager"/> class.
        /// </summary>
        internal MessageManager(GameMode gameMode)
        {
            this._gameMode = gameMode;
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        internal void Destruct()
        {
            this._gameMode = null;
        }

        /// <summary>
        ///     Receives the specicified <see cref="PiranhaMessage"/>.
        /// </summary>
        internal void ReceiveMessage(PiranhaMessage message)
        {
            switch (message.GetMessageType())
            {
                case 14101:
                    this.GoHomeMessageReceived((GoHomeMessage) message);
                    break;
                case 14102:
                    this.EndClientTurnMessageReceived((EndClientTurnMessage) message);
                    break;
                case 14134:
                    this.AttackNpcMessageReceived((AttackNpcMessage) message);        
                    break;
            }
        }

        /// <summary>
        ///     Called when a <see cref="GoHomeMessage"/> is received.
        /// </summary>
        internal void GoHomeMessageReceived(GoHomeMessage message)
        {
            this._gameMode.SetHomeState();
        }

        /// <summary>
        ///     Called when a <see cref="EndClientTurnMessage"/> is received.
        /// </summary>
        internal void EndClientTurnMessageReceived(EndClientTurnMessage message)
        {
            this._gameMode.ClientTurnReceived(message.GetSubTick(), message.GetChecksum(), message.GetCommands());
        }

        /// <summary>
        ///     Called when a <see cref="AttackNpcMessage"/> is received.
        /// </summary>
        internal void AttackNpcMessageReceived(AttackNpcMessage message)
        {
            LogicNpcData npcData = message.GetNpcData();

            if (npcData != null)
            {
                this._gameMode.SetNpcAttackState(npcData);
            }
        }
    }
}