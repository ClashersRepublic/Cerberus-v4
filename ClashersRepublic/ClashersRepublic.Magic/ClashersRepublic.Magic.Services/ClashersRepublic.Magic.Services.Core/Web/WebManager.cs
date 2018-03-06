namespace ClashersRepublic.Magic.Services.Core.Web
{
    using System;
    using System.Net;

    public class WebManager
    {
        private static WebClient _webClient;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        public static void Initialize()
        {
            WebManager._webClient = new WebClient();
        }

        /// <summary>
        ///     Downloads the string to the specified url.
        /// </summary>
        public static string DownloadString(string url)
        {
            try
            {
                return WebManager._webClient.DownloadString(url);
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        ///     Downloads the specified config file.
        /// </summary>
        public static string DownloadFileFromConfigServer(string path)
        {
            if (path.Length > 0)
            {
                if (path[0] != '/')
                {
                    path = "/" + path;
                }
            }

            return WebManager.DownloadString(Constants.CONFIG_SERVER + path);
        }

        /// <summary>
        ///     Downloads the specified config file.
        /// </summary>
        public static byte[] DownloadDataFromConfigServer(string path)
        {
            if (path.Length > 0)
            {
                if (path[0] != '/')
                {
                    path = "/" + path;
                }
            }

            return WebManager.DownloadData(Constants.CONFIG_SERVER + path);
        }

        /// <summary>
        ///     Downloads the string to the specified url.
        /// </summary>
        public static byte[] DownloadData(string url)
        {
            try
            {
                return WebManager._webClient.DownloadData(url);
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}