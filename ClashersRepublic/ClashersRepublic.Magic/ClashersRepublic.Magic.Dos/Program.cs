namespace ClashersRepublic.Magic.Dos
{
    using System;
    using System.Drawing;
    using System.Reflection;
    using System.Threading;
    using ClashersRepublic.Magic.Dos.Bot;
    using ClashersRepublic.Magic.Dos.Debug;

    using ClashersRepublic.Magic.Logic.Data;

    internal class Program
    {
        private const int Width = 120;
        private const int Height = 30;
        private static Client _latency;

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

                if (cmd != null)
                {
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

                            case "load100":
                            {
                                for (int i = 0; i < 100; i++)
                                {
                                    ClientManager.Create();
                                }

                                break;
                            }

                            case "load1000":
                            {
                                for (int i = 0; i < 1000; i++)
                                {
                                    ClientManager.Create();
                                }

                                break;
                            }

                            case "load5000":
                            {
                                for (int i = 0; i < 5000; i++)
                                {
                                    ClientManager.Create();
                                }

                                break;
                            }

                            case "test":
                            {
                                ClientManager.ForEeach(client => { client.MessageManager.SendKeepAliveMessage(); });

                                break;
                            }

                            case "hard":
                            {
                                for (int i = 0; i < 14; i++)
                                {
                                    ClientManager.DisconnectAll();

                                    for (int j = 0; j < 1000; j++)
                                    {
                                        ClientManager.Create();
                                    }

                                    Thread.Sleep(2000);
                                }

                                break;
                            }

                            case "simulation":
                            {
                                for (int i = 0; i < 50; i++)
                                {
                                    for (int j = 0; j < 200; j++)
                                    {
                                        ClientManager.Create();
                                    }

                                    Thread.Sleep(1000);
                                }
                                break;
                            }

                            case "ping":
                            {
                                int cnt = 0;
                                int ping = 0;

                                ClientManager.ForEeach(client =>
                                {
                                    int tmp = client.MessageManager.GetPing();

                                    if (tmp != -1)
                                    {
                                        cnt += 1;
                                        ping += tmp;
                                    }
                                });

                                if (cnt > 0)
                                {
                                    Console.WriteLine("Latency: " + ping / cnt);
                                }

                                break;
                            }
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