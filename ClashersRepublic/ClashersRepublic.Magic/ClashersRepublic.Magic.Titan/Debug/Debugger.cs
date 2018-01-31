namespace ClashersRepublic.Magic.Titan.Debug
{
    using System.IO;
    using System.Runtime.CompilerServices;

    public static class Debugger
    {
        public delegate void DebugEventHandler(object sender, DebugEventArgs args);

        public static event DebugEventHandler LogEvent;
        public static event DebugEventHandler WarningEvent;
        public static event DebugEventHandler ErrorEvent;

        /// <summary>
        ///     Do assert the specified message.
        /// </summary>
        public static bool DoAssert(bool assertion, string assertionError, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            if (!assertion)
            {
                Debugger.ErrorEvent(typeof(Debugger), new DebugEventArgs(assertionError, Path.GetFileName(callerFilePath), callerLineNumber));
            }

            return assertion;
        }

        /// <summary>
        ///     Logs the specified message.
        /// </summary>
        public static void Log(string log, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            Debugger.LogEvent(typeof(Debugger), new DebugEventArgs(log, Path.GetFileName(callerFilePath), callerLineNumber));
        }

        /// <summary>
        ///     Logs the specified warning message.
        /// </summary>
        public static void Warning(string log, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            Debugger.WarningEvent(typeof(Debugger), new DebugEventArgs(log, Path.GetFileName(callerFilePath), callerLineNumber));
        }

        /// <summary>
        ///     Logs the specified error message.
        /// </summary>
        public static void Error(string log, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            Debugger.ErrorEvent(typeof(Debugger), new DebugEventArgs(log, Path.GetFileName(callerFilePath), callerLineNumber));
        }
    }

    public class DebugEventArgs
    {
        public string Text;
        public string FileName;
        public int Line;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DebugEventArgs" /> class.
        /// </summary>
        public DebugEventArgs(string text, string fileName, int line)
        {
            this.Text = text;
            this.FileName = fileName;
            this.Line = line;
        }
    }
}