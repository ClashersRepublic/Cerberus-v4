namespace ClashersRepublic.Magic.Services.Account.Network.Session
{
    using ClashersRepublic.Magic.Services.Core.Network.Session;

    public class NetAccountSession : NetSession
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="NetAccountSession"/> class.
        /// </summary>
        public NetAccountSession(byte[] sessionId, string sessionName) : base(sessionId, sessionName)
        {

        }
    }
}