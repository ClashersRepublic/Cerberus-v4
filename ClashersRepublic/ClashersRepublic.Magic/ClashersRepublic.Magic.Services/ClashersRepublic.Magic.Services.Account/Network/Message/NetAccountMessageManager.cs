﻿namespace ClashersRepublic.Magic.Services.Account.Network.Message
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
                case 10100:
                    this.LoginClientMessageReceived((LoginClientMessage) message);
                    break;
                case 10101:
                    this.CreateAccountBanMessageReceived((CreateAccountBanMessage) message);
                    break;
                case 10102:
                    this.RevokeAccountBanMessageReceived((RevokeAccountBanMessage) message);
                    break;

                case 10301:
                    this.ServerUnboundMessageReceived((ServerUnboundMessage) message);
                    break;

                default:
                    Logging.Warning("NetAccountMessageManager::receiveMessage no case for message type " + message.GetType().Name);
                    break;
            }
        }

        /// <summary>
        ///     Sends the response <see cref="NetMessage"/> to the requester.
        /// </summary>
        internal static void SendResponseMessage(NetMessage requestMessage, NetMessage responseMessage)
        {
            NetMessageManager.SendMessage(requestMessage.GetServiceNodeType(), requestMessage.GetServiceNodeId(), requestMessage.GetSessionId(), responseMessage);
        }

        /// <summary>
        ///     Called when the <see cref="LoginClientMessage"/> is received.
        /// </summary>
        internal void LoginClientMessageReceived(LoginClientMessage message)
        {
            LogicLong accountId = message.RemoveAccountId();

            if (!accountId.IsZero())
            {
                string passToken = message.RemovePassToken();

                if (passToken != null)
                {
                    if (AccountManager.TryGet(accountId, out Account account))
                    {
                        if (!account.IsBanned())
                        {
                            this.StartSession(account, message.RemoveSessionId(), message.GetServiceNodeId());
                        }
                        else
                        {
                            this.SendLoginClientFailedMessage(1, NetManager.GetServiceNodeEndPoint(1, message.GetServiceNodeId()));
                        }
                    }
                    else
                    {
                        this.SendLoginClientFailedMessage(2, NetManager.GetServiceNodeEndPoint(1, message.GetServiceNodeId()));
                    }
                }
                else
                {
                    this.SendLoginClientFailedMessage(2, NetManager.GetServiceNodeEndPoint(1, message.GetServiceNodeId()));
                }
            }
            else
            {
                string passToken = message.RemovePassToken();

                if (passToken == null)
                {
                    this.StartSession(AccountManager.CreateAccount(), message.RemoveSessionId(), message.GetServiceNodeId());
                }
                else
                {
                    this.SendLoginClientFailedMessage(2, NetManager.GetServiceNodeEndPoint(1, message.GetServiceNodeId()));
                }
            }
        }

        /// <summary>
        ///     Handle the login of client.
        /// </summary>
        internal void StartSession(Account account, byte[] sessionId, int proxyId)
        {
            if (account.Session != null)
            {
                NetAccountSessionManager.TryRemove(sessionId);

                account.Session.SendMessage(1, new UnbindServerMessage());
                account.Session.Destruct();
                account.SetSession(null);
            }

            NetAccountSession session = NetAccountSessionManager.Create(account, sessionId);

            account.SetSession(session);
            session.SetServiceNodeId(1, proxyId);

            LoginClientOkMessage loginClientOkMessage = new LoginClientOkMessage();

            loginClientOkMessage.SetAccountId(account.Id);
            loginClientOkMessage.SetHomeId(account.Id);
            loginClientOkMessage.SetPassToken(account.PassToken);
            loginClientOkMessage.SetAccountCreatedDate(account.AccountCreatedDate);
            loginClientOkMessage.SetSessionCount(account.TotalSessions);
            loginClientOkMessage.SetPlayTimeSeconds(account.PlayTimeSecs);
            
            session.SendMessage(1, loginClientOkMessage);
        }

        /// <summary>
        ///     Called when the <see cref="CreateAccountBanMessage"/> is received.
        /// </summary>
        internal void CreateAccountBanMessageReceived(CreateAccountBanMessage message)
        {
            LogicLong accountId = message.RemoveAccountId();

            if (accountId != null)
            {
                if (AccountManager.TryGet(accountId, out Account account))
                {
                    if (!account.IsBanned())
                    {
                        if (message.GetEndTime() >= -1)
                        {
                            if (account.CreateBan(message.GetReason(), message.GetEndTime()))
                            {
                                NetAccountMessageManager.SendResponseMessage(message, new AccountBanCreatedMessage());
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
                if (AccountManager.TryGet(accountId, out Account account))
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

        /// <summary>
        ///     Sends a <see cref="LoginClientFailedMessage"/> to the specified server.
        /// </summary>
        internal void SendLoginClientFailedMessage(int errorCode, NetSocket socket)
        {
            LoginClientFailedMessage message = new LoginClientFailedMessage();
            message.SetErrorCode(errorCode);
            NetMessageManager.SendMessage(socket, message);
        }
    }
}