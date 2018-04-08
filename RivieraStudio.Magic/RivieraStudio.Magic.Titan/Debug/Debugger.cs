namespace RivieraStudio.Magic.Titan.Debug
{
    public static class Debugger
    {
        public delegate void DebugEventHandler(object sender, DebugEventArgs args);

        public static event DebugEventHandler HudPrintEvent;
        public static event DebugEventHandler PrintEvent;
        public static event DebugEventHandler WarningEvent;
        public static event DebugEventHandler ErrorEvent;

        /// <summary>
        ///     Do assert the specified message.
        /// </summary>
        public static bool DoAssert(bool assertion, string assertionError)
        {
            if (!assertion)
            {
                Debugger.ErrorEvent(typeof(Debugger), new DebugEventArgs(assertionError));
            }

            return assertion;
        }

        /// <summary>
        ///     Print the specified message to hud.
        /// </summary>
        public static void HudPrint(string log)
        {
            Debugger.HudPrintEvent(typeof(Debugger), new DebugEventArgs(log));
        }

        /// <summary>
        ///     Print the specified message.
        /// </summary>
        public static void Print(string log)
        {
            Debugger.PrintEvent(typeof(Debugger), new DebugEventArgs(log));
        }

        /// <summary>
        ///     Logs the specified warning message.
        /// </summary>
        public static void Warning(string log)
        {
            Debugger.WarningEvent(typeof(Debugger), new DebugEventArgs(log));
        }

        /// <summary>
        ///     Logs the specified error message.
        /// </summary>
        public static void Error(string log)
        {
            Debugger.ErrorEvent(typeof(Debugger), new DebugEventArgs(log));
        }
    }

    public class DebugEventArgs
    {
        public string Text { get; }

        /// <summary>
        ///     Initializes a new instance of the <see cref="DebugEventArgs" /> class.
        /// </summary>
        public DebugEventArgs(string text)
        {
            this.Text = text;
        }
    }
}