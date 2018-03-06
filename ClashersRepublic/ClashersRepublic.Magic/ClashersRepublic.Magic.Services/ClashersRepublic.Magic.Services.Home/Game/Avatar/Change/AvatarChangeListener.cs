namespace ClashersRepublic.Magic.Services.Home.Game.Avatar.Change
{
    using ClashersRepublic.Magic.Logic.Avatar.Change;
    using ClashersRepublic.Magic.Services.Home.Network.Session;

    internal class AvatarChangeListener : LogicAvatarChangeListener
    {
        private NetHomeSession _session;

        /// <summary>
        ///     Initializes a new instance of the <see cref="AvatarChangeListener"/> class.
        /// </summary>
        internal AvatarChangeListener(NetHomeSession session)
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