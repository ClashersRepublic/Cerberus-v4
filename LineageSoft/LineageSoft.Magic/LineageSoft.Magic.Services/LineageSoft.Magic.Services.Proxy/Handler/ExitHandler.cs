namespace LineageSoft.Magic.Services.Proxy.Handler
{
    using System;
    using System.Runtime.InteropServices;
    using LineageSoft.Magic.Services.Proxy.Network;

    internal class ExitHandler
    {
        private static EventHandler _exitHandler;

        [DllImport("user32.dll")]
        private static extern int DeleteMenu(IntPtr hMenu, int nPosition, int wFlags);

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
            ExitHandler._exitHandler += ExitHandler.OnQuit;
            ExitHandler.SetConsoleCtrlHandler(ExitHandler._exitHandler, true);
            ExitHandler.DeleteMenu(ExitHandler.GetSystemMenu(ExitHandler.GetConsoleWindow(), false), 0xF060, 0);
        }

        /// <summary>
        ///     Called when the program closes.
        /// </summary>
        internal static void OnQuit()
        {
            NetworkMessagingManager.Destruct();
            Environment.Exit(0);
        }

        private delegate void EventHandler();
    }
}