namespace ClashersRepublic.Magic.Logic.Message
{
    using ClashersRepublic.Magic.Logic.Message.Account;
    using ClashersRepublic.Magic.Logic.Message.Avatar;
    using ClashersRepublic.Magic.Logic.Message.Avatar.Stream;
    using ClashersRepublic.Magic.Logic.Message.Chat;
    using ClashersRepublic.Magic.Logic.Message.Google;
    using ClashersRepublic.Magic.Logic.Message.Home;
    using ClashersRepublic.Magic.Logic.Message.Security;
    using ClashersRepublic.Magic.Titan.Message;
    using ClashersRepublic.Magic.Titan.Message.Security;

    public class LogicMagicMessageFactory : LogicMessageFactory
    {
        public static readonly LogicMessageFactory Instance;

        /// <summary>
        ///     Initializes static members.
        /// </summary>
        static LogicMagicMessageFactory()
        {
            LogicMagicMessageFactory.Instance = new LogicMagicMessageFactory();
        }

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

                    case 10113:
                    {
                        message = new SetDeviceTokenMessage();
                        break;
                    }

                    case 10116:
                    {
                        message = new ResetAccountMessage();
                        break;
                    }

                    case 10117:
                    {
                        message = new ReportUserMessage();
                        break;
                    }

                    case 10118:
                    {
                        message = new AccountSwitchedMessage();
                        break;
                    }

                    case 10150:
                    {
                        message = new AppleBillingRequestMessage();
                        break;
                    }

                    case 10212:
                    {
                        message = new ChangeAvatarNameMessage();
                        break;
                    }

                    case 14101:
                    {
                        message = new GoHomeMessage();
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

                    case 14325:
                    {
                        message = new AskForAvatarProfileMessage();
                        break;
                    }

                    case 14715:
                    {
                        message = new SendGlobalChatLineMessage();
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
                        message = new ExtendedSetEncryptionMessage();
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

                    case 20117:
                    {
                        message = new ReportUserStatusMessage();
                        break;
                    }

                    case 20151:
                    {
                        message = new AppleBillingProcessedByServerMessage();
                        break;
                    }

                    case 20161:
                    {
                        message = new ShutdownStartedMessage();
                        break;
                    }

                    case 20171:
                    {
                        message = new PersonalBreakStartedMessage();
                        break;
                    }

                    case 20261:
                    {
                        message = new GoogleServiceAccountBoundMessage();
                        break;
                    }

                    case 20262:
                    {
                        message = new GoogleServiceAccountAlreadyBoundMessage();
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

                    case 24111:
                    {
                        message = new AvailableServerCommand();
                        break;
                    }

                    case 24112:
                    {
                        message = new WaitingToGoHomeMessage();
                        break;
                    }

                    case 24115:
                    {
                        message = new ServerErrorMessage();
                        break;
                    }

                    case 24133:
                    {
                        message = new NpcDataMessage();
                        break;
                    }

                    case 24334:
                    {
                        message = new AvatarProfileMessage();
                        break;
                    }

                    case 24411:
                    {
                        message = new AvatarStreamMessage();
                        break;
                    }

                    case 24715:
                    {
                        message = new GlobalChatLineMessage();
                        break;
                    }
                }
            }

            return message;
        }
    }
}