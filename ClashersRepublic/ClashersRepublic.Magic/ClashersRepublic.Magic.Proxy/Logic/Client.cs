﻿namespace ClashersRepublic.Magic.Proxy.Logic
{
    using System;
    
    using ClashersRepublic.Magic.Proxy.Message;
    using ClashersRepublic.Magic.Proxy.Network;
    using ClashersRepublic.Magic.Services.Logic.Account;

    internal class Client : IDisposable
    {
        private bool _disposed;

        internal string SessionId;

        internal GameAccount Account;
        internal NetworkToken Token;
        internal MessageManager MessageManager;


        /// <summary>
        ///     Initializes a new instance of the <see cref="Client" /> class.
        /// </summary>
        internal Client(NetworkToken token)
        {
            this.Token = token;
            this.SessionId = Config.ServerId.ToString("X") + Guid.NewGuid().ToString("N").Substring(1);

            ClientManager.AddClient(this);
        }

        /// <summary>
        ///     Disposes this instance.
        /// </summary>
        public void Dispose()
        {
            if (this._disposed)
            {
                return;
            }

            this._disposed = true;

            this.Token = null;
            this.MessageManager = null;

            ClientManager.RemoveClient(this);
        }
    }
}