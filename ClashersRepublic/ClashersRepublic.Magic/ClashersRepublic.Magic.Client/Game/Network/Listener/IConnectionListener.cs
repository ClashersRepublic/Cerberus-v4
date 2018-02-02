namespace ClashersRepublic.Magic.Client.Game.Network.Listener
{
    internal interface IConnectionListener
    {
        void OnConnectionFailed(Connection connection);
        void OnStart(Connection connection);
        void OnConnect(Connection connection);
        void OnDisconnect(Connection connection);
        void OnWakeup(Connection connection);
        void OnReceive(Connection connection);
    }
}