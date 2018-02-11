namespace ClashersRepublic.Magic.Logic.Message
{
    using ClashersRepublic.Magic.Logic.Message.Account;
    using ClashersRepublic.Magic.Logic.Message.Battle;
    using ClashersRepublic.Magic.Logic.Message.Google;
    using ClashersRepublic.Magic.Logic.Message.Home;
    using ClashersRepublic.Magic.Logic.Message.Security;
    using ClashersRepublic.Magic.Titan.Message;
    using ClashersRepublic.Magic.Titan.Message.Security;

    public static class LogicMagicMessageFactory
    {
        public const string RC4_KEY = "fhsd6f86f67rt8fw78fw789we78r9789wer6re";

        /// <summary>
        ///     Creates a message by type.
        /// </summary>
        public static PiranhaMessage CreateMessageByType(int type)
        {
            PiranhaMessage message = null;

            if (type < 20000)
            {
                switch (type)
                {
                    case 10100:
                    {
                        message = new ClientHelloMessage();
                        break;
                    }

                    case 10101:
                    {
                        message = new LoginMessage();
                        break;
                    }

                    case 10108:
                    {
                        message = new KeepAliveMessage();
                        break;
                    }

                    case 14102:
                    {
                        message = new EndClientTurnMessage();
                        break;
                    }

                    case 14134:
                    {
                        message = new AttackNpcMessage();
                        break;
                    }

                    case 14262:
                    {
                        message = new BindGoogleServiceAccountMessage();
                        break;
                    }
                }
            }
            else
            {
                switch (type)
                {
                    case 20000:
                    {
                        message = new EncryptionMessage();
                        break;
                    }

                    case 20100:
                    {
                        message = new ServerHelloMessage();
                        break;
                    }

                    case 20103:
                    {
                        message = new LoginFailedMessage();
                        break;
                    }

                    case 20104:
                    {
                        message = new LoginOkMessage();
                        break;
                    }

                    case 20108:
                    {
                        message = new KeepAliveServerMessage();
                        break;
                    }

                    case 24101:
                    {
                        message = new OwnHomeDataMessage();
                        break;
                    }

                    case 24104:
                    {
                        message = new OutOfSyncMessage();
                        break;
                    }
                }
            }

            return message;
        }
    }
}