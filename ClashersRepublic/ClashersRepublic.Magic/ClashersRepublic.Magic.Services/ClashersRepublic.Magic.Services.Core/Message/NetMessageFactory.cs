namespace ClashersRepublic.Magic.Services.Core.Message
{
    using ClashersRepublic.Magic.Services.Core.Message.Account;
    using ClashersRepublic.Magic.Services.Core.Message.Avatar;
    using ClashersRepublic.Magic.Services.Core.Message.Session;

    public class NetMessageFactory
    {
        /// <summary>
        ///     Creates a new <see cref="NetMessage" /> instance by type.
        /// </summary>
        public static NetMessage CreateMessageByType(int messageType)
        {
            if (messageType < 20000)
            {
                switch (messageType)
                {
                    case 10102: return new LoginClientMessage();
                    case 10103: return new CreateAccountMessage();
                    case 10104: return new CreateAccountBanMessage();
                    case 10105: return new RevokeAccountBanMessage();

                    case 10200: return new CreateAvatarMessage();
                    case 10201: return new AskForAvatarMessage();
                    case 10202: return new SetAvatarDataMessage();

                    case 10300: return new RemoveSessionSocketMessage();
                    case 10301: return new SetSessionSocketMessage();
                }
            }
            else
            {
                switch (messageType)
                {
                    case 20101: return new CreateAccountOkMessage();
                    case 20102: return new CreateAccountFailedMessage();
                    case 20103: return new LoginClientFailedMessage();
                    case 20104: return new LoginClientOkMessage();
                    case 20105: return new AccoutBanCreatedMessage();
                    case 20106: return new AccountBanRevokedMessage();

                    case 20201: return new CreateAvatarOkMessage();
                    case 20202: return new CreateAvatarFailedMessage();
                    case 20203: return new AvatarDataMessage();

                    case 20300: return new UpdateSessionSocketListMessage();
                    case 20301: return new UpdateSessionSocketMessage();
                }
            }

            return null;
        }
    }
}