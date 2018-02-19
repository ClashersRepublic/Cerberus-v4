﻿namespace ClashersRepublic.Magic.Proxy.Session
{
    using System;
    using ClashersRepublic.Magic.Proxy.Account;
    using ClashersRepublic.Magic.Proxy.Service;
    using ClashersRepublic.Magic.Proxy.User;

    using ClashersRepublic.Magic.Services.Logic.Log;
    using ClashersRepublic.Magic.Services.Logic.Message.Messaging;
    using ClashersRepublic.Magic.Services.Logic.Message.Session;

    using ClashersRepublic.Magic.Titan.Message;

    using NetMQ;

    internal class GameSession
    {
        private readonly ServerEndPoint[] _serverEndPoints;

        internal string SessionId { get; }

        internal Client Client { get; }
        internal GameAccount Account { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GameSession" /> class.
        /// </summary>
        internal GameSession(string sessionId, Client client, GameAccount account)
        {
            this.SessionId = sessionId;
            this.Client = client;
            this.Account = account;

            this._serverEndPoints = new ServerEndPoint[28];

            for (int i = 0; i < 28; i++)
            {
                this._serverEndPoints[i] = new ServerEndPoint();
            }
        }

        /// <summary>
        ///     Binds the specified service node.
        /// </summary>
        internal void BindServiceNode(int serviceNodeType, int serverId)
        {
            if (serviceNodeType < 0 || serviceNodeType >= this._serverEndPoints.Length)
            {
                Logging.Error(this, "GameSession::bindServiceNode serviceNodeType out of bands " + serviceNodeType + "/" + this._serverEndPoints.Length);
            }
            else
            {
                ServerEndPoint currentEndPoint = this._serverEndPoints[serviceNodeType];

                if (serverId == -1)
                {
                    if (currentEndPoint.Socket != null)
                    {
                        if (currentEndPoint.IsBound)
                        {
                            ServiceMessageManager.SendMessage(new ServiceNodeUnboundToSessionMessage(), this.SessionId, currentEndPoint.Socket);
                        }

                        currentEndPoint.SetData(null, false);
                    }
                }
                else
                {
                    NetMQSocket newServerEndPoint = ServiceManager.GetServiceSocket(serviceNodeType, serverId);

                    if (newServerEndPoint == null)
                    {
                        Logging.Error(this, "GameSession::bindServiceNode specified service node cannot exist");
                    }
                    else
                    {
                        if (currentEndPoint.Socket != null)
                        {
                            if (currentEndPoint.Socket != newServerEndPoint)
                            {
                                if (currentEndPoint.IsBound)
                                {
                                    ServiceMessageManager.SendMessage(new ServiceNodeUnboundToSessionMessage(), this.SessionId, currentEndPoint.Socket);
                                }
                            }
                        }

                        currentEndPoint.SetData(newServerEndPoint, true);

                        ServiceMessageManager.SendMessage(new ServiceNodeBoundToSessionMessage
                        {
                            AccountId = this.Account.Id
                        }, this.SessionId, newServerEndPoint);
                    }
                }
            }
        }

        /// <summary>
        ///     Sets the specified service node.
        /// </summary>
        internal void SetServiceNode(int serviceNodeType, int serverId)
        {
            if (serviceNodeType < 0 || serviceNodeType >= this._serverEndPoints.Length)
            {
                Logging.Error(this, "GameSession::setServiceNode serviceNodeType out of bands " + serviceNodeType + "/" + this._serverEndPoints.Length);
            }
            else
            {
                ServerEndPoint currentEndPoint = this._serverEndPoints[serviceNodeType];

                if (serverId == -1)
                {
                    if (currentEndPoint.Socket != null)
                    {
                        if (currentEndPoint.IsBound)
                        {
                            ServiceMessageManager.SendMessage(new ServiceNodeUnboundToSessionMessage(), this.SessionId, currentEndPoint.Socket);
                        }

                        currentEndPoint.SetData(null, false);
                    }
                }
                else
                {
                    NetMQSocket newServerEndPoint = ServiceManager.GetServiceSocket(serviceNodeType, serverId);

                    if (newServerEndPoint == null)
                    {
                        Logging.Error(this, "GameSession::setServiceNode specified service node cannot exist");
                    }
                    else
                    {
                        if (currentEndPoint.Socket != null)
                        {
                            if (currentEndPoint.Socket == newServerEndPoint)
                            {
                                return;
                            }
                            else
                            {
                                if (currentEndPoint.IsBound)
                                {
                                    ServiceMessageManager.SendMessage(new ServiceNodeUnboundToSessionMessage(), this.SessionId, currentEndPoint.Socket);
                                }
                            }
                        }

                        currentEndPoint.SetData(newServerEndPoint, false);
                    }
                }
            }
        }

        /// <summary>
        ///     Removes the specified service node.
        /// </summary>
        internal void RemoveServiceNode(int serviceNodeType)
        {
            if (serviceNodeType < 0 || serviceNodeType >= this._serverEndPoints.Length)
            {
                Logging.Error(this, "GameSession::setServiceNode serviceNodeType out of bands " + serviceNodeType + "/" + this._serverEndPoints.Length);
            }
            else
            {
                ServerEndPoint currentEndPoint = this._serverEndPoints[serviceNodeType];

                if (currentEndPoint.Socket != null)
                {
                    if (currentEndPoint.IsBound)
                    {
                        ServiceMessageManager.SendMessage(new ServiceNodeUnboundToSessionMessage(), this.SessionId, currentEndPoint.Socket);
                    }

                    currentEndPoint.SetData(null, false);
                }
            }
        }

        /// <summary>
        ///     Removes all service nodes.
        /// </summary>
        internal void RemoveServiceNodes()
        {
            for (int i = 0; i < this._serverEndPoints.Length; i++)
            {
                this.RemoveServiceNode(i);
            }
        }

        /// <summary>
        ///     Forwards the specified <see cref="PiranhaMessage"/> to the right server.
        /// </summary>
        internal void ForwardPiranhaMessage(PiranhaMessage message)
        {
            this.ForwardPiranhaMessage(message, message.GetServiceNodeType());
        }

        /// <summary>
        ///     Forwards the specified <see cref="PiranhaMessage"/> to the specified service node type.
        /// </summary>
        internal void ForwardPiranhaMessage(PiranhaMessage message, int serviceNodeType)
        {
            if (serviceNodeType > -1 && serviceNodeType < this._serverEndPoints.Length)
            {
                ServerEndPoint serverEndPoint = this._serverEndPoints[serviceNodeType];

                if (serverEndPoint != null)
                {
                    ServiceMessageManager.SendMessage(new ForwardPiranhaMessage
                    {
                        PiranhaMessage = message
                    }, this.SessionId, serverEndPoint.Socket);
                }
            }
            else
            {
                Logging.Error(this, "GameSession::forwardPiranhaMessage service node type out of bands " + serviceNodeType + "/" + this._serverEndPoints.Length);
            }
        }

        private class ServerEndPoint
        {
            internal NetMQSocket Socket { get; private set; }
            internal bool IsBound { get; private set; }

            /// <summary>
            ///     Initializes a new instance of the <see cref="ServerEndPoint"/> class.
            /// </summary>
            internal ServerEndPoint()
            {
                // ServerEndPoint.
            }

            /// <summary>
            ///     Sets the instance data.
            /// </summary>
            internal void SetData(NetMQSocket socket, bool isBound)
            {
                this.Socket = socket;
                this.IsBound = isBound;
            }
        }
    }
}