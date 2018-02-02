﻿namespace ClashersRepublic.Magic.Proxy
{
    using System;
    using System.Drawing;
    using System.Reflection;

    using ClashersRepublic.Magic.Proxy.Account;
    using ClashersRepublic.Magic.Proxy.Debug;
    using ClashersRepublic.Magic.Proxy.Handler;
    using ClashersRepublic.Magic.Services.Logic;

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
            Config.Initialize();

            Console.WriteLine('1' + '1');

            if (args.Length > 0)
            {
                Config.ServerId = int.Parse(args[0]);
            }

            Resources.Initialize();

            while (true)
            {
                string cmd = Console.ReadLine();

                if (cmd.StartsWith("/"))
                {
                    switch (cmd.Substring(1))
                    {
                        case "close":
                        case "stop":
                        case "shutdown":
                        {
                            ExitHandler.OnQuit();
                            return;
                        }

                        case "test":
                        {
                            for (int i = 0; i < 1000; i++)
                            {
                                GameAccountManager.CreateAccount();
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
            Console.Title = "Clashers Republic - " + Assembly.GetExecutingAssembly().GetName().Name + " - " + Config.ServerId;
        }
    }
}