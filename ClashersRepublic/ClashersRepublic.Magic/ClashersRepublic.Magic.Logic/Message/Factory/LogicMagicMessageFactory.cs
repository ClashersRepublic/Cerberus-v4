namespace ClashersRepublic.Magic.Logic.Message.Factory
{
    using ClashersRepublic.Magic.Logic.Message.Account;

    public sealed class LogicMagicMessageFactory : LogicMessageFactory
    {
        public static readonly LogicMessageFactory Instance = new LogicMagicMessageFactory();

        /// <summary>
        ///     Creates a message by type.
        /// </summary>
        public override PiranhaMessage CreateMessageByType(int type)
        {
            PiranhaMessage message = null;

            if (type < 20000)
            {
                switch (type)
                {
                    case 10101:
                    {
                        message = new LoginMessage();
                        break;
                    }
                }
            }
            else
            {
                switch (type)
                {
                    case 20103:
                    {
                        message = new LoginFailedMessage();
                        break;
                    }
                }
            }

            return message;
        }
    }
}