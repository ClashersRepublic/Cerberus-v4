namespace RivieraStudio.Magic.Services.Zone.Game.Avatar.Change
{
    using RivieraStudio.Magic.Logic.Avatar.Change;
    using RivieraStudio.Magic.Services.Zone.Network.Session;

    internal class AvatarChangeListener : LogicAvatarChangeListener
    {
        private NetZoneSession _session;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AvatarChangeListener"/> class.
        /// </summary>
        internal AvatarChangeListener(NetZoneSession session)
        {
            this._session = session;
        }

        /// <summary>
        ///     Called when exp level is gained.
        /// </summary>
        public override void ExpLevelGained(int count)
        {
        }

        /// <summary>
        ///     Sends the specified <see cref="LogicAvatarChange"/> to servers.
        /// </summary>
        public void SendAvatarChange(LogicAvatarChange entry)
        {
        }
    }
}