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
                Debugger.ErrorEvent(Path.GetFileName(callerFilePath) + ":" + callerLineNumber, new DebugEventArgs(assertionError));
            }

            return assertion;
        }

        /// <summary>
        ///     Logs the specified message.
        /// </summary>
        public static void Log(string log, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            Debugger.LogEvent(Path.GetFileName(callerFilePath) + ":" + callerLineNumber, new DebugEventArgs(log));
        }

        /// <summary>
        ///     Logs the specified warning message.
        /// </summary>
        public static void Warning(string log, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            Debugger.WarningEvent(Path.GetFileName(callerFilePath) + ":" + callerLineNumber, new DebugEventArgs(log));
        }

        /// <summary>
        ///     Logs the specified error message.
        /// </summary>
        public static void Error(string log, [CallerFilePath] string callerFilePath = "", [CallerLineNumber] int callerLineNumber = 0)
        {
            Debugger.ErrorEvent(Path.GetFileName(callerFilePath) + ":" + callerLineNumber, new DebugEventArgs(log));
        }
    }

    public class DebugEventArgs
    {
        public string Text;

        /// <summary>
        ///     Initializes a new instance of the <see cref="DebugEventArgs" /> class.
        /// </summary>
        public DebugEventArgs(string text)
        {
            this.Text = text;
        }
    }
}