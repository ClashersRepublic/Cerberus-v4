namespace ClashersRepublic.Magic.Client.Game
{
    using ClashersRepublic.Magic.Client.Game.Network;
    using ClashersRepublic.Magic.Logic.Data;

    internal class GameMain
    {
        internal ServerConnection _serverConnection;

        /// <summary>
        ///     Initializes a new instance of the <see cref="GameMain"/> class.
        /// </summary>
        internal GameMain()
        {
            this._serverConnection = new ServerConnection();
        }

        /// <summary>
        ///     Initializes the game.
        /// </summary>
        internal static void Initialize()
        {
            ResourceManager.Initialize();
            LogicDataTables.Initialize();
        }
    }
}