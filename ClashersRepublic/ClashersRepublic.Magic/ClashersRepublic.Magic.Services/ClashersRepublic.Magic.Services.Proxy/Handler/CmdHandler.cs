namespace ClashersRepublic.Magic.Services.Proxy.Handler
{
    using System;
    using ClashersRepublic.Magic.Services.Core.Message;
    using ClashersRepublic.Magic.Services.Core.Message.Account;
    using ClashersRepublic.Magic.Services.Core.Network;

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
                                for (int i = 0; i < 10000; i++)
                                {
                                    NetSocket socket = NetManager.GetRandomEndPoint(2);

                                    if (socket != null)
                                    {
                                        NetMessageManager.SendMessage(socket, new byte[10], 10, new CreateAccountMessage());
                                    }
                                }
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