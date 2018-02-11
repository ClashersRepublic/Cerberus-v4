﻿namespace ClashersRepublic.Magic.Services.Home
{
    using System;
    using System.Drawing;
    using System.Reflection;

    using ClashersRepublic.Magic.Services.Home.Debug;
    using ClashersRepublic.Magic.Services.Home.Handler;
    using ClashersRepublic.Magic.Services.Home.Player;
    using ClashersRepublic.Magic.Services.Home.Session;
    using ClashersRepublic.Magic.Services.Logic;

    internal class Program
    {
        private const int Width = 120;
        private const int Height = 30;

        private static void Main(string[] args)
        {
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

            Logging.Initialize();
            Config.Initialize();
            
            if (args.Length > 0)
            {
                Config.ServerId = int.Parse(args[0]);
            }

            Resources.Initialize();
            ExitHandler.Initialize();

            Console.WriteLine(Assembly.GetExecutingAssembly().GetName().Name + " is now starting..." + Environment.NewLine);
            Program.UpdateConsoleTitle();
            CmdHandler.Initialize();
        }

        /// <summary>
        ///     Updates the console title.
        /// </summary>
        internal static void UpdateConsoleTitle()
        {
            Console.Title = "Clashers Republic - " + Assembly.GetExecutingAssembly().GetName().Name + " - ServerID : " + Config.ServerId + " - Sessions : " + GameSessionManager.TotalSessions + " - Players : " + GamePlayerManager.TotalPlayers;
        }
    }
}