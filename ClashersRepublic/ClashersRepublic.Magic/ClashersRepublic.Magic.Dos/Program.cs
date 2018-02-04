namespace ClashersRepublic.Magic.Dos
{
    using System;
    using System.Drawing;
    using System.Reflection;
    using System.Threading;
    using ClashersRepublic.Magic.Dos.Bot;
    using ClashersRepublic.Magic.Dos.Debug;
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.Message.Account;

    internal class Program
    {
        private const int Width = 120;
        private const int Height = 30;

        private static void Main(string[] args)
        {
            Program.UpdateConsoleTitle();

            Console.SetOut(new ConsoleOut());
            Console.SetWindowSize(Program.Width, Program.Height);

            Colorful.Console.WriteWithGradient(@"
     _________.__                 .__                          __________                   ___.   .__.__        
     \_   ___ \|  | _____    _____|  |__   ___________  ______ \______   \ ____ ______  __ _\_ |__ |  | |__| ____  
     /    \  \/|  | \__  \  /  ___/  |  \_/ __ \_  __ \/  ___/  |       _// __ \\____ \|  |  \ __ \|  | |  |/ ___\ 
     \     \___|  |__/ __ \_\___ \|   Y  \  ___/|  | \/\___ \   |    |   \  ___/|  |_> >  |  / \_\ \  |_|  \  \___ 
      \______  /____(____  /____  >___|  /\___  >__|  /____  >  |____|_  /\___  >   __/|____/|___  /____/__|\___  >
             \/          \/     \/     \/     \/           \/          \/     \/|__|             \/             \/ 
            ", Color.OrangeRed, Color.LimeGreen, 14);

            Console.ForegroundColor = ConsoleColor.White;

            Console.WriteLine(Environment.NewLine);
            Console.WriteLine(Assembly.GetExecutingAssembly().GetName().Name + " is now starting..." + Environment.NewLine);

            Logging.Initialize();
            LogicDataTables.Initialize();
            ClientManager.Initialize();

            while (true)
            {
                string cmd = Console.ReadLine();

                if (cmd.StartsWith("/"))
                {
                    switch (cmd.Substring(1))
                    {
                        case "create":
                        {
                            for (int i = 0; i < 1; i++)
                            {
                                ClientManager.Create();
                            }

                            break;
                        }
                            
                        case "dos":
                        {
                            for (int j = 0; j < 14000; j++)
                            {
                                ClientManager.Create();
                            }

                            break;
                        }

                        case "test":
                        {
                            ClientManager.ForEeach(client =>
                            {
                                client.MessageManager.SendKeepAliveMessage();
                            });

                            break;
                        }

                        case "ping":
                        {
                            Client client = ClientManager.Get();

                            if (client != null)
                            {
                                client.MessageManager.Ping();
                            }
                            else
                            {
                                Console.WriteLine("No client is connected to the server");
                            }

                            break;
                        }
                    }
                }
            }
        }

        /// <summary>
        ///     Updates the console title.
        /// </summary>
        internal static void UpdateConsoleTitle()
        {
            Console.Title = "Clashers Republic - " + Assembly.GetExecutingAssembly().GetName().Name;
        }
    }
}