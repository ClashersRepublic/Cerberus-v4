namespace ClashersRepublic.Magic.Services.Proxy.Handler
{
    using System;
    using ClashersRepublic.Magic.Services.Proxy.Network;

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
                            case "kill":
                                NetworkMessagingManager.Destruct();
                                break;
                            case "test":
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