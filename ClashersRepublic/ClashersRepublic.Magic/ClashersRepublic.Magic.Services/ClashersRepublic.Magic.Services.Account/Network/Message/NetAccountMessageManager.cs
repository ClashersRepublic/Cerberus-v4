namespace ClashersRepublic.Magic.Services.Account.Network.Message
{
    using System;
    using ClashersRepublic.Magic.Services.Account.Database;
    using ClashersRepublic.Magic.Services.Account.Game;
    using ClashersRepublic.Magic.Services.Account.Network.Session;
    using ClashersRepublic.Magic.Services.Core;
    using ClashersRepublic.Magic.Services.Core.Message;
    using ClashersRepublic.Magic.Services.Core.Message.Account;
    using ClashersRepublic.Magic.Services.Core.Message.Avatar;
    using ClashersRepublic.Magic.Services.Core.Message.Session;
    using ClashersRepublic.Magic.Services.Core.Network;

    using ClashersRepublic.Magic.Titan.Math;
    using ClashersRepublic.Magic.Titan.Util;

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

                case 10301:
                    this.ServerUnboundMessageReceived((ServerUnboundMessage) message);
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
                if (AccountManager.TryGetAccount(accountId, out Account account))
                {
                    if (account.PassToken.Equals(message.RemovePassToken()))
                    {
                        if (!account.IsBanned())
                        {
                            if (NetAccountSessionManager.TryCreate(account, message.GetSessionId(), out NetAccountSession session))
                            {
                                if (account.Session != null)
                                {
                                    if (NetAccountSessionManager.TryRemove(account.Session.SessionName, out NetAccountSession oldSession))
                                    {
                                        NetSocket proxy = oldSession.GetServiceNodeEndPoint(1);
                                        Logging.DoAssert(this, proxy != null, "NetAccountMessageManager::loginClientMessageReceived pProxy->NULL");
                                        NetMessageManager.SendMessage(proxy, oldSession.SessionId, oldSession.SessionId.Length, new UnbindServerMessage());

                                        oldSession.Account.EndSession();
                                        oldSession.SaveAccount();
                                        oldSession.Destruct();
                                    }
                                }

                                account.SetSession(session);
                                account.SessionStarted(LogicTimeUtil.GetTimestamp(), message.GetIPAddress(), message.GetDeviceModel());

                                LoginClientOkMessage loginClientOkMessage = new LoginClientOkMessage();

                                loginClientOkMessage.SetAccountId(account.Id);
                                loginClientOkMessage.SetHomeId(account.Id);
                                loginClientOkMessage.SetPassToken(account.PassToken);
                                loginClientOkMessage.SetAccountCreatedDate(account.AccountCreatedDate);

                                NetAccountMessageManager.SendResponseMessage(message, loginClientOkMessage);
                            }
                        }
                        else
                        {
                            LoginClientFailedMessage banMessage = new LoginClientFailedMessage();
                            banMessage.SetErrorCode(2);
                            NetAccountMessageManager.SendResponseMessage(message, banMessage);
                        }

                        return;
                    }
                }
            }

            LoginClientFailedMessage responseMessage = new LoginClientFailedMessage();
            responseMessage.SetErrorCode(1);
            NetAccountMessageManager.SendResponseMessage(message, responseMessage);
        }

        /// <summary>
        ///     Called when the <see cref="CreateAccountMessage"/> is received.
        /// </summary>
        internal void CreateAccountMessageReceived(CreateAccountMessage message)
        {
            if (AccountManager.TryCreateAccount(out LogicLong accountId, out Account account))
            {
                CreateHomeMessage createHomeMessage = new CreateHomeMessage();
                createHomeMessage.SetAccountId(accountId);
                NetMessageManager.SendMessage(10, NetManager.GetDocumentOwnerId(10, accountId), createHomeMessage);

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
                    if (!account.IsBanned())
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
                    if (account.RevokeBan())
                    {
                        NetAccountMessageManager.SendResponseMessage(message, new AccountBanRevokedMessage());
                    }
                }
            }
        }

        /// <summary>
        ///     Called when a <see cref="ServerUnboundMessage"/> is received.
        /// </summary>
        internal void ServerUnboundMessageReceived(ServerUnboundMessage message)
        {
            byte[] sessionId = message.GetSessionId();

            if (sessionId != null)
            {
                if (NetAccountSessionManager.TryRemove(sessionId, out NetAccountSession session))
                {
                    session.Account.EndSession();
                    session.SaveAccount();
                    session.Destruct();
                }
            }
        }
    }
}