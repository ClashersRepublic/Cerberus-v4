namespace ClashersRepublic.Magic.Services.Account.Handler
{
    using System;
    using System.Runtime.InteropServices;

    internal class ExitHandler
    {
        public const int SC_CLOSE = 0xF060;

        private static bool _initialized;

        private static EventHandler _exitHandler;

        [DllImport("user32.dll")]
        public static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

        [DllImport("user32.dll")]
        private static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);

        [DllImport("Kernel32")]
        private static extern bool SetConsoleCtrlHandler(EventHandler handler, bool enabled);

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            if (ExitHandler._initialized)
            {
                return;
            }

            ExitHandler._initialized = true;
            ExitHandler._exitHandler += ExitHandler.OnQuit;
            ExitHandler.SetConsoleCtrlHandler(ExitHandler._exitHandler, true);
            ExitHandler.DeleteMenu(ExitHandler.GetSystemMenu(ExitHandler.GetConsoleWindow(), false), ExitHandler.SC_CLOSE, 0);
        }

        /// <summary>
        ///     Called when the program closes.
        /// </summary>
        private static void OnQuit()
        {
            Environment.Exit(0);
        }

        private delegate void EventHandler();
    }
}