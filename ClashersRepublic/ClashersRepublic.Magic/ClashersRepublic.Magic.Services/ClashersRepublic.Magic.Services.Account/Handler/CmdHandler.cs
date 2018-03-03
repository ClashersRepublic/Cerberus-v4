namespace ClashersRepublic.Magic.Services.Account.Handler
{
    using System;
    using System.Diagnostics;

    using ClashersRepublic.Magic.Services.Account.Game;

    internal static class CmdHandler
    {
        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            while (true)
            {
                string cmd = Console.ReadLine();

                if (!string.IsNullOrEmpty(cmd))
                {
                    if (cmd[0] == '/')
                    {
                        string[] parameters = cmd.Substring(1).Split(' ');

                        switch (parameters[0])
                        {
                            case "shutdown":
                            case "close":
                            case "quit":
                                CmdHandler.Close();
                                break;
                            case "test":
                                Stopwatch watch = new Stopwatch();
                                watch.Start();

                                for (int i = 0; i < 1_000_000; i++)
                                {
                                    AccountManager.TryCreateAccount(out _, out _);
                                }

                                watch.Stop();
                                Console.WriteLine(watch.Elapsed.TotalMilliseconds);
                                break;
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Closes the program.
        /// </summary>
        private static void Close()
        {
            ExitHandler.OnQuit();
        }
    }
}