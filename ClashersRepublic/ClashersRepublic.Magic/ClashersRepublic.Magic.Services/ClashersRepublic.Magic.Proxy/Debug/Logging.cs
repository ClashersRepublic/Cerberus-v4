namespace ClashersRepublic.Magic.Proxy.Debug
{
    using System;
    using System.Diagnostics;
    using ClashersRepublic.Magic.Titan.Debug;
    using Debugger = ClashersRepublic.Magic.Titan.Debug.Debugger;

    internal static class Logging
    {
        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            Debugger.LogEvent += Logging.Log;
            Debugger.WarningEvent += Logging.Warning;
            Debugger.ErrorEvent += Logging.Error;
        }

        /// <summary>
        ///     Logs the specified message.
        /// </summary>
        private static void Log(object sender, DebugEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("[DEBUG] {0}:{1}: {2}", args.FileName, args.Line, args.Text);
        }

        /// <summary>
        ///     Logs the specified message.
        /// </summary>
        private static void Warning(object sender, DebugEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("[WARNING] {0}:{1}: {2}", args.FileName, args.Line, args.Text);
        }

        /// <summary>
        ///     Logs the specified message.
        /// </summary>
        private static void Error(object sender, DebugEventArgs args)
        {
            System.Diagnostics.Debug.WriteLine("[ERROR] {0}:{1}: {2}", args.FileName, args.Line, args.Text);
        }

        /// <summary>
        ///     Logs the specified debug message.
        /// </summary>
        [Conditional("DEBUG")]
        internal static void Debug(object sender, string message)
        {
            System.Diagnostics.Debug.WriteLine("[DEBUG] " + sender.GetType().Name + ": " + message);
        }

        /// <summary>
        ///     Logs the specified debug message.
        /// </summary>
        [Conditional("DEBUG")]
        internal static void Debug(Type sender, string message)
        {
            System.Diagnostics.Debug.WriteLine("[DEBUG] " + sender.Name + ": " + message);
        }

        /// <summary>
        ///     Logs the specified message.
        /// </summary>
        [Conditional("DEBUG")]
        internal static void Log(object sender, string message)
        {
            System.Diagnostics.Debug.WriteLine("[LOG] " + sender.GetType().Name + ": " + message);
        }

        /// <summary>
        ///     Logs the specified message.
        /// </summary>
        [Conditional("DEBUG")]
        internal static void Log(Type sender, string message)
        {
            System.Diagnostics.Debug.WriteLine("[LOG] " + sender.Name + ": " + message);
        }

        /// <summary>
        ///     Logs the specified warning message.
        /// </summary>
        internal static void Warning(object sender, string message)
        {
            System.Diagnostics.Debug.WriteLine("[WARNING] " + sender.GetType().Name + ": " + message);
        }

        /// <summary>
        ///     Logs the specified warning message.
        /// </summary>
        internal static void Warning(Type sender, string message)
        {
            System.Diagnostics.Debug.WriteLine("[WARNING] " + sender.Name + ": " + message);
        }

        /// <summary>
        ///     Logs the specified error message.
        /// </summary>
        internal static void Error(object sender, string message)
        {
            System.Diagnostics.Debug.WriteLine("[ERROR] " + sender.GetType().Name + ": " + message);
        }

        /// <summary>
        ///     Logs the specified error message.
        /// </summary>
        internal static void Error(Type sender, string message)
        {
            System.Diagnostics.Debug.WriteLine("[ERROR] " + sender.Name + ": " + message);
        }

        /// <summary>
        ///     Do assert the specified message.
        /// </summary>
        internal static void DoAssert(object sender, bool assertCondition, string assertionError)
        {
            if (!assertCondition)
            {
                System.Diagnostics.Debug.WriteLine("[ASSERT] " + sender.GetType().Name + ": " + assertionError);
            }
        }

        /// <summary>
        ///     Do assert the specified message.
        /// </summary>
        internal static void DoAssert(Type sender, bool assertCondition, string assertionError)
        {
            if (!assertCondition)
            {
                System.Diagnostics.Debug.WriteLine("[ASSERT] " + sender.Name + ": " + assertionError);
            }
        }
    }
}