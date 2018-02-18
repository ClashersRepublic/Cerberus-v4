namespace ClashersRepublic.Magic.Services.Home.Debug
{
    using System;
    using System.Diagnostics;
    using ClashersRepublic.Magic.Titan.Debug;

    using CDebug = System.Diagnostics.Debug;
    using LogicDebugger = ClashersRepublic.Magic.Titan.Debug.Debugger;

    internal static class Logging
    {
        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            LogicDebugger.LogEvent += Logging.Log;
            LogicDebugger.WarningEvent += Logging.Warning;
            LogicDebugger.ErrorEvent += Logging.Error;
        }

        /// <summary>
        ///     Logs the specified message.
        /// </summary>
        private static void Log(object sender, DebugEventArgs args)
        {
            CDebug.WriteLine("[DEBUG] {0}:{1}: {2}", args.FileName, args.Line, args.Text);
        }

        /// <summary>
        ///     Logs the specified message.
        /// </summary>
        private static void Warning(object sender, DebugEventArgs args)
        {
            CDebug.WriteLine("[WARNING] {0}:{1}: {2}", args.FileName, args.Line, args.Text);
        }

        /// <summary>
        ///     Logs the specified message.
        /// </summary>
        private static void Error(object sender, DebugEventArgs args)
        {
            CDebug.WriteLine("[ERROR] {0}:{1}: {2}", args.FileName, args.Line, args.Text);
        }

        /// <summary>
        ///     Logs the specified debug message.
        /// </summary>
        [Conditional("DEBUG")]
        internal static void Debug(object sender, string message)
        {
            CDebug.WriteLine("[DEBUG] {0}: {1}", sender.GetType().Name, message);
        }

        /// <summary>
        ///     Logs the specified debug message.
        /// </summary>
        [Conditional("DEBUG")]
        internal static void Debug(Type sender, string message)
        {
            CDebug.WriteLine("[DEBUG] {0}: {1}", sender.Name, message);
        }

        /// <summary>
        ///     Logs the specified message.
        /// </summary>
        [Conditional("DEBUG")]
        internal static void Log(object sender, string message)
        {
            CDebug.WriteLine("[LOG] {0}: {1}", sender.GetType().Name, message);
        }

        /// <summary>
        ///     Logs the specified message.
        /// </summary>
        [Conditional("DEBUG")]
        internal static void Log(Type sender, string message)
        {
            CDebug.WriteLine("[LOG] {0}: {1}", sender.Name, message);
        }

        /// <summary>
        ///     Logs the specified warning message.
        /// </summary>
        internal static void Warning(object sender, string message)
        {
            CDebug.WriteLine("[WARNING] {0}: {1}", sender.GetType().Name, message);
        }

        /// <summary>
        ///     Logs the specified warning message.
        /// </summary>
        internal static void Warning(Type sender, string message)
        {
            CDebug.WriteLine("[WARNING] {0}: {1}", sender.Name, message);
        }

        /// <summary>
        ///     Logs the specified error message.
        /// </summary>
        internal static void Error(object sender, string message)
        {
            CDebug.WriteLine("[ERROR] {0}: {1}", sender.GetType().Name, message);
        }

        /// <summary>
        ///     Logs the specified error message.
        /// </summary>
        internal static void Error(Type sender, string message)
        {
            CDebug.WriteLine("[ERROR] {0}: {1}", sender.Name, message);
        }

        /// <summary>
        ///     Do assert the specified message.
        /// </summary>
        internal static void DoAssert(object sender, bool assertCondition, string assertionError)
        {
            if (!assertCondition)
            {
                CDebug.WriteLine("[ASSERT] {0}: {1}", sender.GetType().Name, assertionError);
            }
        }

        /// <summary>
        ///     Do assert the specified message.
        /// </summary>
        internal static void DoAssert(Type sender, bool assertCondition, string assertionError)
        {
            if (!assertCondition)
            {
                CDebug.WriteLine("[ASSERT] {0}: {1}", sender.Name, assertionError);
            }
        }
    }
}