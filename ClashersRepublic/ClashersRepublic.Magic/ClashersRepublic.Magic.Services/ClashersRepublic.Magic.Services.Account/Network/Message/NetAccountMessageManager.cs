namespace ClashersRepublic.Magic.Services.Account.Network.Message
{
    using ClashersRepublic.Magic.Services.Account.Game;
    using ClashersRepublic.Magic.Services.Account.Network.Session;
    using ClashersRepublic.Magic.Services.Core;
    using ClashersRepublic.Magic.Services.Core.Message;
    using ClashersRepublic.Magic.Services.Core.Message.Account;
    using ClashersRepublic.Magic.Services.Core.Message.Session;
    using ClashersRepublic.Magic.Services.Core.Network;
    using ClashersRepublic.Magic.Titan.Math;

    internal class NetAccountMessageManager : NetMessageManager
    {
        /// <summary>
        ///     Receives the specicified <see cref="NetMessage"/>.
        /// </summary>
        public override void ReceiveMessage(NetMessage message)
        {
            switch (message.GetMessageType())
            {
                case 10102:
                    this.LoginClientMessageReceived((LoginClientMessage) message);
                    break;
                case 10103:
                    this.CreateAccountMessageReceived((CreateAccountMessage) message);
                    break;
                case 10104:
                    this.CreateAccountBanMessageReceived((CreateAccountBanMessage) message);
                    break;
                case 10105:
                    this.RevokeAccountBanMessageReceived((RevokeAccountBanMessage) message);
                    break;
            }
        }

        /// <summary>
        ///     Sends the response <see cref="NetMessage"/> to the requester.
        /// </summary>
        internal static void SendResponseMessage(NetMessage requestMessage, NetMessage responseMessage)
        {
            NetMessageManager.SendMessage(requestMessage.GetServiceNodeType(), requestMessage.GetServiceNodeId(), requestMessage.GetSessionId(), requestMessage.GetSessionIdLength(), responseMessage);
        }

        /// <summary>
        ///     Called when the <see cref="LoginClientMessage"/> is received.
        /// </summary>
        internal void LoginClientMessageReceived(LoginClientMessage message)
        {
            LogicLong accountId = message.RemoveAccountId();

            if (!accountId.IsZero())
            {
                if (accountId.GetHigherInt() == ServiceCore.ServiceNodeId)
                {
                    if (AccountManager.TryGetAccount(accountId, out Account account))
                    {
                        if (account.PassToken.Equals(message.RemovePassToken()))
                        {
                            NetAccountSession session = NetAccountSessionManager.TryCreate(account, message.GetSessionId());

                            if (session != null)
                            {
                                if (account.Session != null)
                                {
                                    // Abort
                                }

                                session.SetServiceNodeId(1, message.GetServiceNodeId(), true);
                                session.SetServiceNodeId(2, ServiceCore.ServiceNodeId, true);
                                session.SetServiceNodeId(3, account.Id.GetHigherInt(), true);

                                account.SetSession(session);

                                LoginClientOkMessage loginClientOkMessage = new LoginClientOkMessage();

                                loginClientOkMessage.SetAccountId(account.Id);
                                loginClientOkMessage.SetHomeId(account.Id);
                                loginClientOkMessage.SetPassToken(account.PassToken);
                                loginClientOkMessage.SetAccountCreatedDate(account.AccountCreatedDate);

                                NetAccountMessageManager.SendResponseMessage(message, loginClientOkMessage);

                                return;
                            }
                        }
                    }
                }
            }

            LoginClientFailedMessage responseMessage = new LoginClientFailedMessage();
            responseMessage.SetErrorCode(1);
            NetAccountMessageManager.SendResponseMessage(message, responseMessage);
        }

        /// <summary>
        ///     Called when the <see cref="CreateAccountMessageReceived"/> is received.
        /// </summary>
        internal void CreateAccountMessageReceived(CreateAccountMessage message)
        {
            if (AccountManager.TryCreateAccount(out LogicLong accountId, out Account account))
            {
                LoginClientOkMessage createAccountOkMessage = new LoginClientOkMessage();

                createAccountOkMessage.SetAccountId(accountId);
                createAccountOkMessage.SetHomeId(accountId);
                createAccountOkMessage.SetPassToken(account.PassToken);

                NetAccountMessageManager.SendResponseMessage(message, createAccountOkMessage);
            }
            else
            {
                CreateAccountFailedMessage createAccountFailedMessage = new CreateAccountFailedMessage();
                createAccountFailedMessage.SetErrorCode(1);
                NetAccountMessageManager.SendResponseMessage(message, createAccountFailedMessage);
            }
        }

        /// <summary>
        ///     Called when the <see cref="CreateAccountBanMessage"/> is received.
        /// </summary>
        internal void CreateAccountBanMessageReceived(CreateAccountBanMessage message)
        {
            LogicLong accountId = message.RemoveAccountId();

            if (accountId != null)
            {
                if (AccountManager.TryGetAccount(accountId, out Account account))
                {
                    if (account.CurrentBan == null)
                    {
                        if (message.GetEndTime() >= -1)
                        {
                            if (account.CreateBan(message.GetReason(), message.GetEndTime()))
                            {
                                NetAccountMessageManager.SendResponseMessage(message, new AccoutBanCreatedMessage());
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Called when the <see cref="RevokeAccountBanMessage"/> is received.
        /// </summary>
        internal void RevokeAccountBanMessageReceived(RevokeAccountBanMessage message)
        {
            LogicLong accountId = message.RemoveAccountId();

            if (accountId != null)
            {
                if (AccountManager.TryGetAccount(accountId, out Account account))
                {
                    if (account.CurrentBan != null)
                    {
                        if (account.RevokeBan())
                        {
                            NetAccountMessageManager.SendResponseMessage(message, new AccountBanRevokedMessage());
                        }
                    }
                }
            }
        }
    }
}