namespace ClashersRepublic.Magic.Proxy.User
{
    using System;

    using ClashersRepublic.Magic.Proxy.Network;
    using ClashersRepublic.Magic.Proxy.Session;

    internal class Client : IDisposable
    {
        internal NetworkToken NetworkToken { get; private set; }
        internal GameSession GameSession { get; set; }
        internal ClientDefines Defines { get; set; }

        internal int State { get; set; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Client" /> class.
        /// </summary>
        internal Client(NetworkToken token)
        {
            this.NetworkToken = token;
            this.State = 1;
        }

        /// <summary>
        ///     Disposes this instance.
        /// </summary>
        public void Dispose()
        {
            this.State = 0;

            if (this.GameSession != null)
            {
                GameSessionManager.CloseSession(this.GameSession);
            }

            this.GameSession = null;
            this.NetworkToken = null;
        }
    }
}