namespace ClashersRepublic.Magic.Client.Game
{
    using ClashersRepublic.Magic.Client.Game.Network;
    using ClashersRepublic.Magic.Logic.Data;

    internal class GameMain
    {
        internal ServerConnection ServerConnection;

        /// <summary>
        ///     Initializes a new instance of the <see cref="GameMain" /> class.
        /// </summary>
        internal GameMain()
        {
            this.ServerConnection = new ServerConnection();
        }

        /// <summary>
        ///     Updates the client.
        /// </summary>
        internal void Update(float time)
        {
            this.ServerConnection.Update(time);
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