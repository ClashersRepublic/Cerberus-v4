namespace ClashersRepublic.Magic.Proxy.Handler
{
    using System;

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
                        string[] parameters = cmd.Substring(1).Split('.');

                        switch (parameters[0])
                        {
                            case "shut":
                            case "shutdown":
                            case "close":
                            case "quit":
                            {
                                CmdHandler.Close();
                                return;
                            }

                            case "test":
                            {
                                CmdHandler.Test();
                                break;
                            }
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

        /// <summary>
        ///     Test function.
        /// </summary>
        private static void Test()
        {
            // Test.
        }
    }
}