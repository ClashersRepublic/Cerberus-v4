namespace ClashersRepublic.Magic.Services.Home
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using Debugger = ClashersRepublic.Magic.Titan.Debug.Debugger;

    public static class Logging
    {
        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            Debugger.LogEvent += (sender, args) =>
            {
                string[] fileInfo = sender.ToString().Split(':');

                Debug.WriteLine("[DEBUG] " + args.Text + " " + Path.GetFileName(fileInfo[0]) + ":" + fileInfo[1]);
            };

            Debugger.WarningEvent += (sender, args) =>
            {
                string[] fileInfo = sender.ToString().Split(':');

                Debug.WriteLine("[WARNING] " + args.Text + " " + Path.GetFileName(fileInfo[0]) + ":" + fileInfo[1]);
            };

            Debugger.ErrorEvent += (sender, args) =>
            {
                string[] fileInfo = sender.ToString().Split(':');

                Debug.WriteLine("[ERROR] " + args.Text + " " + Path.GetFileName(fileInfo[0]) + ":" + fileInfo[1]);
            };
        }

        /// <summary>
        ///     Logs the specified informative message.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="message">The message.</param>
        public static void Info(Type type, string message)
        {
            Debug.WriteLine("[INFO] " + type.Name + " : " + message);
        }

        /// <summary>
        ///     Logs the specified warning message.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="message">The message.</param>
        public static void Warning(Type type, string message)
        {
            Debug.WriteLine("[WARNING] " + type.Name + " : " + message);
        }

        /// <summary>
        ///     Logs the specified error message.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <param name="message">The message.</param>
        public static void Error(Type type, string message)
        {
            Debug.WriteLine("[ERROR] " + type.Name + " : " + message);
        }
    }
}