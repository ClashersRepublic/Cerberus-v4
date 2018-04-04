namespace RivieraStudio.Magic.Services.Core
{
    using System;
    using System.Runtime.CompilerServices;

    using RivieraStudio.Magic.Services.Core.Exception;
    using RivieraStudio.Magic.Titan.Debug;

    using MSDebug = System.Diagnostics.Debug;
    using LogicDebugger = RivieraStudio.Magic.Titan.Debug.Debugger;

    public static class Logging
    {
        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        public static void Initialize()
        {
            LogicDebugger.PrintEvent += Logging.Print;
            LogicDebugger.WarningEvent += Logging.Warning;
            LogicDebugger.ErrorEvent += Logging.Error;
        }

        /// <summary>
        ///     Print the specified message.
        /// </summary>
        private static void Print(object sender, DebugEventArgs args)
        {
            MSDebug.WriteLine("[DEBUG] " + args.Text);
        }

        /// <summary>
        ///     Print the specified message.
        /// </summary>
        public static void Print(string message)
        {
            MSDebug.WriteLine("[DEBUG] " + message);
        }
        
        /// <summary>
        ///     Logs the specified warning message.
        /// </summary>
        private static void Warning(object sender, DebugEventArgs args)
        {
            MSDebug.WriteLine("[WARNING] " + args.Text);
        }

        /// <summary>
        ///     Logs the specified warning message.
        /// </summary>
        public static void Warning(string message)
        {
            MSDebug.WriteLine("[WARNING] " + message);
        }

        /// <summary>
        ///     Logs the specified error message.
        /// </summary>
        private static void Error(object sender, DebugEventArgs args)
        {
            MSDebug.WriteLine("[ERROR] " + args.Text);
        }

        /// <summary>
        ///     Logs the specified error message.
        /// </summary>
        public static void Error(string message)
        {
            MSDebug.WriteLine("[ERROR] " + message);
        }

        /// <summary>
        ///     Do assert the specified condition.
        /// </summary>
        public static void DoAssert(bool assertCondition, string assertionError, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            if (!assertCondition)
            {
                throw new AssertionException(file, line, assertionError);
            }
        }
    }
}