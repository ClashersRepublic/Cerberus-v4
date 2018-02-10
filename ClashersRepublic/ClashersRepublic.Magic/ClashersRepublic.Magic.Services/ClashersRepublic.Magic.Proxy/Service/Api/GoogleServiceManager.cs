namespace ClashersRepublic.Magic.Proxy.Service.Api
{
    using System.IO;
    using System.Threading;

    using Google.Apis.Auth.OAuth2;
    using Google.Apis.Games.v1;
    using Google.Apis.Services;
    using Google.Apis.Util.Store;

    internal static class GoogleServiceManager
    {
        private static UserCredential _credentials;
        private static GamesService _service;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            GoogleServiceManager._credentials = GoogleServiceManager.GetCredentials();
            GoogleServiceManager.Login();
        }

        /// <summary>
        ///     Gets the credentials.
        /// </summary>
        private static UserCredential GetCredentials()
        {
            return GoogleWebAuthorizationBroker.AuthorizeAsync(GoogleServiceManager.GetClientConfiguration().Secrets,
                new[]
                {
                    GamesService.Scope.Games,
                    GamesService.Scope.PlusLogin
                }, "ClashersRepublic.Magic.Services", CancellationToken.None, new FileDataStore("ClashersRepublic.Magic.Services")).Result;
        }

        /// <summary>
        ///     Retrieves the Client Configuration from the server path.
        /// </summary>
        private static GoogleClientSecrets GetClientConfiguration()
        {
            using (var stream = new FileStream("client_secrets.json", FileMode.Open, FileAccess.Read))
            {
                return GoogleClientSecrets.Load(stream);
            }
        }

        /// <summary>
        ///     Logs in the Game Service API's Servers.
        /// </summary>
        private static void Login()
        {
            GoogleServiceManager._service = new GamesService(new BaseClientService.Initializer
            {
                HttpClientInitializer = GoogleServiceManager._credentials,
                ApplicationName = "ClashersRepublic.Magic.Services",
                ApiKey = "fd03592da8d3d53e5e96b708932324ceefee004b"
            });
        }
    }
}