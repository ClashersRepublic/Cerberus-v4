namespace ClashersRepublic.Magic.Services.Logic.Message.Factory
{
    using ClashersRepublic.Magic.Logic.Message;
    using ClashersRepublic.Magic.Logic.Message.Factory;
    using ClashersRepublic.Magic.Services.Logic.Message.Account;

    public sealed class MagicServiceMessageFactory : LogicMessageFactory
    {
        public static readonly LogicMessageFactory Instance = new MagicServiceMessageFactory();

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
                    case 10200:
                    {
                        message = new StartSessionMessage();
                        break;
                    }

                    case 10210:
                    {
                        message = new StopSessionMessage();
                        break;
                    }
                }
            }
            else
            {
                switch (type)
                {
                    case 20200:
                    {
                        message = new StartSessionOkMessage();
                        break;
                    }

                    case 20201:
                    {
                        message = new StartSessionFailedMessage();
                        break;
                    }

                    case 20250:
                    {
                        message = new DisconnectMessage();
                        break;
                    }
                }
            }

            return message;
        }
    }
}