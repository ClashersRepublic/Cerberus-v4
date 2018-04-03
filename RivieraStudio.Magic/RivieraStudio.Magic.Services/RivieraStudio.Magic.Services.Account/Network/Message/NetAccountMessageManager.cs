namespace RivieraStudio.Magic.Services.Account.Network.Message
{
    using RivieraStudio.Magic.Services.Account.Game;
    using RivieraStudio.Magic.Services.Account.Network.Session;

    using RivieraStudio.Magic.Services.Core;
    using RivieraStudio.Magic.Services.Core.Message;
    using RivieraStudio.Magic.Services.Core.Message.Account;
    using RivieraStudio.Magic.Services.Core.Message.Admin;
    using RivieraStudio.Magic.Services.Core.Message.Session;
    using RivieraStudio.Magic.Services.Core.Network;
    using RivieraStudio.Magic.Services.Core.Utils;
    using RivieraStudio.Magic.Titan.Math;
    using RivieraStudio.Magic.Titan.Util;

    internal class NetAccountMessageManager : NetMessageManager
    {
        /// <summary>
        ///     Receives the specicified <see cref="NetMessage"/>.
        /// </summary>
        public override void ReceiveMessage(NetMessage message)
        {
            if (message.GetSessionId() == null)
            {
                Logging.Warning("NetAccountMessageManager::receiveMessage session id is not defined");
                return;
            }

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

                case 10600:
                    this.ExecuteAdminCommandMessageReceived((ExecuteAdminCommandMessage) message);
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
            byte[] sessionId = message.RemoveSessionId();

            if (!accountId.IsZero())
            {
                string passToken = message.RemovePassToken();

                if (passToken != null)
                {
                    if (AccountManager.TryGet(accountId, out Account account))
                    {
                        if (!account.IsBanned())
                        {
                            this.StartSession(account, message.GetIPAddress(), message.GetDeviceModel(), sessionId, message.GetServiceNodeId());
                        }
                        else
                        {
                            this.SendLoginClientFailedMessage(1, sessionId, NetManager.GetServiceNodeEndPoint(1, message.GetServiceNodeId()), account.CurrentBan.BanReason, account.GetRemainingBanTime());
                        }
                    }
                    else
                    {
                        this.SendLoginClientFailedMessage(2, sessionId, NetManager.GetServiceNodeEndPoint(1, message.GetServiceNodeId()));
                    }
                }
                else
                {
                    this.SendLoginClientFailedMessage(2, sessionId, NetManager.GetServiceNodeEndPoint(1, message.GetServiceNodeId()));
                }
            }
            else
            {
                string passToken = message.RemovePassToken();

                if (passToken == null)
                {
                    this.StartSession(AccountManager.CreateAccount(), message.GetIPAddress(), message.GetDeviceModel(), sessionId, message.GetServiceNodeId());
                }
                else
                {
                    this.SendLoginClientFailedMessage(2, sessionId, NetManager.GetServiceNodeEndPoint(1, message.GetServiceNodeId()));
                }
            }
        }

        /// <summary>
        ///     Handle the login of client.
        /// </summary>
        internal void StartSession(Account account, string ipAddress, string deviceModel, byte[] sessionId, int proxyId)
        {
            if (account.Session != null)
            {
                NetAccountSessionManager.TryRemove(account.Session.SessionId);

                account.Session.SendMessage(NetUtils.SERVICE_NODE_TYPE_PROXY_CONTAINER, new UnbindServerMessage());
                account.Session.Destruct();
                account.SetSession(null);
            }

            NetAccountSession session = NetAccountSessionManager.Create(account, sessionId);

            account.SetSession(session);
            account.SessionStarted(LogicTimeUtil.GetTimestamp(), ipAddress, deviceModel);
            session.SetServiceNodeId(NetUtils.SERVICE_NODE_TYPE_PROXY_CONTAINER, proxyId);

            LoginClientOkMessage loginClientOkMessage = new LoginClientOkMessage();

            loginClientOkMessage.SetAccountId(account.Id);
            loginClientOkMessage.SetHomeId(account.Id);
            loginClientOkMessage.SetPassToken(account.PassToken);
            loginClientOkMessage.SetAccountCreatedDate(account.AccountCreatedDate);
            loginClientOkMessage.SetSessionCount(account.TotalSessions);
            loginClientOkMessage.SetPlayTimeSeconds(account.PlayTimeSecs);
            
            session.SendMessage(NetUtils.SERVICE_NODE_TYPE_PROXY_CONTAINER, loginClientOkMessage);
        }

        /// <summary>
        ///     Called when the <see cref="CreateAccountBanMessage"/> is received.
        /// </summary>
        internal void CreateAccountBanMessageReceived(CreateAccountBanMessage message)
        {
            LogicLong accountId = message.RemoveAccountId();

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

        /// <summary>
        ///     Called when the <see cref="RevokeAccountBanMessage"/> is received.
        /// </summary>
        internal void RevokeAccountBanMessageReceived(RevokeAccountBanMessage message)
        {
            LogicLong accountId = message.RemoveAccountId();

            if (AccountManager.TryGet(accountId, out Account account))
            {
                if (account.RevokeBan())
                {
                    NetAccountMessageManager.SendResponseMessage(message, new AccountBanRevokedMessage());
                }
            }
        }

        /// <summary>
        ///     Called when a <see cref="ServerUnboundMessage"/> is received.
        /// </summary>
        internal void ServerUnboundMessageReceived(ServerUnboundMessage message)
        {
            byte[] sessionId = message.GetSessionId();

            if (NetAccountSessionManager.TryRemove(sessionId, out NetAccountSession session))
            {
                session.Account.EndSession();
                session.Account.SetSession(null);
                session.SaveAccount();
                session.Destruct();
            }
        }

        /// <summary>
        ///     Sends a <see cref="LoginClientFailedMessage"/> to the specified server.
        /// </summary>
        internal void SendLoginClientFailedMessage(int errorCode, byte[] sessionId, NetSocket socket, string reason = null, int remainingTime = -1)
        {
            LoginClientFailedMessage message = new LoginClientFailedMessage();
            message.SetErrorCode(errorCode);
            message.SetReason(reason);
            message.SetRemainingTime(remainingTime);
            NetMessageManager.SendMessage(socket, sessionId, message);
        }

        /// <summary>
        ///     Called when a <see cref="ExecuteAdminCommandMessage"/> is received.
        /// </summary>
        private void ExecuteAdminCommandMessageReceived(ExecuteAdminCommandMessage message)
        {
            byte[] sessionId = message.GetSessionId();

            if (NetAccountSessionManager.TryGet(sessionId, out NetAccountSession session))
            {
                AdminCommand adminCommand = message.RemoveDebugCommand();

                if (adminCommand.GetServiceNodeType() == ServiceCore.ServiceNodeType)
                {
                    switch (adminCommand.GetCommandType())
                    {
                        default:
                            Logging.Debug(string.Format("executeAdminCommandMessageReceived unknown debug command received ({0})", adminCommand.GetCommandType()));
                            break;
                    }
                }
                else
                {
                    Logging.Debug(string.Format("executeAdminCommandMessageReceived invalid debug command received ({0})", adminCommand.GetCommandType()));
                }
            }
        }
    }
}