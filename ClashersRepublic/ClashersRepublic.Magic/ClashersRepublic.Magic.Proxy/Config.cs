namespace ClashersRepublic.Magic.Proxy
{
    internal class Config
    {
        internal const int ServerId = 0;

        internal const int BufferSize = 8192;
        internal const int MaxPlayers = 1000;

        internal const string ServerVersion = "1.0.0";
        internal const string ServerEnvironment = "stage";

        internal const string GameDatabaseUrl = "mongodb://127.0.0.1:27017";
        internal const string GameDatabaseName = "ClashersRepublic-Magic";
        internal const string GameDatabaseCollection = "Accounts";
    }
}