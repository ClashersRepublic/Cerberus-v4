namespace ClashersRepublic.Magic.Services.Core.Message
{
    using ClashersRepublic.Magic.Services.Core.Message.Account;
    using ClashersRepublic.Magic.Services.Core.Message.Admin;
    using ClashersRepublic.Magic.Services.Core.Message.Avatar;
    using ClashersRepublic.Magic.Services.Core.Message.Avatar.Stream;
    using ClashersRepublic.Magic.Services.Core.Message.Home;
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
                    case 10100: return new LoginClientMessage();
                    case 10101: return new CreateAccountBanMessage();
                    case 10102: return new RevokeAccountBanMessage();
                        
                    case 10300: return new BindServerMessage();
                    case 10301: return new ServerUnboundMessage();
                    case 10302: return new ServerBoundMessage();
                    case 10303: return new UpdateServerEndPointMessage();
                    case 10304: return new UnbindServerMessage();
                    case 10305: return new AskForBindServerMessage();

                    case 10400: return new ForwardPiranhaMessage();

                    case 10511: return new AllowServerCommandMessage();
                    case 10520: return new AskForAvatarProfileFullEntryMessage();

                    case 10600: return new ExecuteAdminCommandMessage();

                    case 11500: return new AddAvatarStreamEntryMessage();
                }
            }
            else
            {
                switch (messageType)
                {
                    case 20100: return new LoginClientFailedMessage();
                    case 20101: return new LoginClientOkMessage();
                    case 20102: return new AccountBanCreatedMessage();
                    case 20103: return new AccountBanRevokedMessage();
                        
                    case 20211: return new AvatarEntryMessage();

                    case 20520: return new AvatarProfileFullEntryMessage();
                }
            }

            return null;
        }
    }
}