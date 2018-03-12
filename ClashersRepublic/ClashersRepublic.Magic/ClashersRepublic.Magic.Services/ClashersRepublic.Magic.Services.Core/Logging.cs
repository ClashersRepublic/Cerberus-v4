namespace ClashersRepublic.Magic.Services.Core
{
    using System;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.CompilerServices;

    using ClashersRepublic.Magic.Services.Core.Exception;
    using ClashersRepublic.Magic.Titan.Debug;

    using CDebug = System.Diagnostics.Debug;
    using LogicDebugger = ClashersRepublic.Magic.Titan.Debug.Debugger;

    public static class Logging
    {
        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        public static void Initialize()
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
        public static void Debug(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            CDebug.WriteLine("[DEBUG] {0}:{1}: {2}", Path.GetFileNameWithoutExtension(file), line, message);
        }
        
        /// <summary>
        ///     Logs the specified message.
        /// </summary>
        public static void Log(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            CDebug.WriteLine("[LOG] {0}:{1}: {2}", Path.GetFileNameWithoutExtension(file), line, message);
        }
        
        /// <summary>
        ///     Logs the specified warning message.
        /// </summary>
        public static void Warning(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            CDebug.WriteLine("[WARNING] {0}:{1}: {2}", Path.GetFileNameWithoutExtension(file), line, message);
        }
        
        /// <summary>
        ///     Logs the specified error message.
        /// </summary>
        public static void Error(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            CDebug.WriteLine("[ERROR] {0}:{1}: {2}", Path.GetFileNameWithoutExtension(file), line, message);
        }
        
        /// <summary>
        ///     Do assert the specified condition.
        /// </summary>
        public static void DoAssert(object sender, bool assertCondition, string assertionError, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            if (!assertCondition)
            {
                throw new AssertionException(file, line, assertionError);
            }
        }

        /// <summary>
        ///     Do assert the specified message.
        /// </summary>
        internal static void DoAssert(Type sender, bool assertCondition, string assertionError, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        {
            if (!assertCondition)
            {
                throw new AssertionException(file, line, assertionError);
            }
        }
    }
}