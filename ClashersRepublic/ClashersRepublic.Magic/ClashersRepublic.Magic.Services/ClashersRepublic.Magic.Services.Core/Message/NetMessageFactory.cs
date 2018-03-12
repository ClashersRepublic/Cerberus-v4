namespace ClashersRepublic.Magic.Services.Core.Message
{
    using ClashersRepublic.Magic.Services.Core.Message.Account;
    using ClashersRepublic.Magic.Services.Core.Message.Avatar;
    using ClashersRepublic.Magic.Services.Core.Message.Network;
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

                    case 10200: return new CreateHomeMessage();
                    case 10201: return new AskForAvatarMessage();

                    case 10300: return new BindServerMessage();
                    case 10301: return new ServerUnboundMessage();
                    case 10302: return new ServerBoundMessage();
                    case 10303: return new UpdateServerEndPointMessage();
                    case 10304: return new UpdateServerEndPointMessage();

                    case 10400: return new ForwardPiranhaMessage();
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
                        
                    case 20203: return new AvatarDataMessage();
                    case 20210: return new AvatarChangeMessage();
                }
            }

            return null;
        }
    }
}