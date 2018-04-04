namespace RivieraStudio.Magic.Services.Core.Web
{
    using System;
    using System.Net;
    using System.Threading.Tasks;

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
        ///     Downloads the specified config string.
        /// </summary>
        public static string DownloadConfigString(string path)
        {
            if (path.Length > 0)
            {
                if (path[0] != '/')
                {
                    path = "/" + path;
                }
            }

            return WebManager.DownloadString(ServiceCore.ConfigurationServer + "/" + ServiceCore.ServiceEnvironment + path);
        }

        /// <summary>
        ///     Downloads the specified asset data.
        /// </summary>
        public static byte[] DownloadAssetData(string sha, string path)
        {
            if (path.Length > 0)
            {
                if (path[0] != '/')
                {
                    path = "/" + path;
                }
            }

            return WebManager.DownloadData(ServiceCore.ConfigurationServer + "/assets/" + sha + "/" + path);
        }
        
        /// <summary>
        ///     Downloads the specified asset string.
        /// </summary>
        public static string DownloadAssetString(string sha, string path)
        {
            if (path.Length > 0)
            {
                if (path[0] != '/')
                {
                    path = "/" + path;
                }
            }

            return WebManager.DownloadString(ServiceCore.ConfigurationServer + "/assets/" + sha + "/" + path);
        }
        
        /// <summary>
        ///     Downloads the data to the specified url.
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