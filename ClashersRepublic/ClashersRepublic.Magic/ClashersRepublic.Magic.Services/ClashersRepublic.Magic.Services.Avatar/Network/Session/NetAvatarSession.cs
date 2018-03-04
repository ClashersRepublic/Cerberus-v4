namespace ClashersRepublic.Magic.Services.Avatar.Network.Session
{
    using ClashersRepublic.Magic.Services.Avatar.Game;
    using ClashersRepublic.Magic.Services.Core.Network.Session;

    internal class NetAvatarSession : NetSession
    {
        private Avatar _avatar;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetAvatarSession"/> class.
        /// </summary>
        internal NetAvatarSession(Avatar avatar, byte[] sessionId, string sessionName) : base(sessionId, sessionName)
        {
            this._avatar = avatar;
        }
    }
}