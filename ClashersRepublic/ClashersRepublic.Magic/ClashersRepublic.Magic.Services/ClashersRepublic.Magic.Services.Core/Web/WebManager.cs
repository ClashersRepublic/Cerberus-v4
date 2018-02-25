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