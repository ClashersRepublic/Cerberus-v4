﻿namespace RivieraStudio.Magic.Services.Account
{
    using System;
    using System.Drawing;
    using System.Reflection;
    using RivieraStudio.Magic.Services.Account.Game;
    using RivieraStudio.Magic.Services.Account.Handler;
    using RivieraStudio.Magic.Services.Account.Network.Session;
    using RivieraStudio.Magic.Services.Core;

    internal class Program
    {
        private const int Width = 120;
        private const int Height = 30;

        private static void Main(string[] args)
        {
            Console.SetOut(new ConsoleOut());
            Console.SetWindowSize(Program.Width, Program.Height);

            Core.Libs.Colorful.Console.WriteWithGradient(@"
                   __________.__      .__                      _________ __            .___.__        
                   \______   \__|__  _|__| ________________   /   _____//  |_ __ __  __| _/|__| ____  
                    |       _/  \  \/ /  |/ __ \_  __ \__  \  \_____  \\   __\  |  \/ __ | |  |/  _ \ 
                    |    |   \  |\   /|  \  ___/|  | \// __ \_/        \|  | |  |  / /_/ | |  (  <_> )
                    |____|_  /__| \_/ |__|\___  >__|  (____  /_______  /|__| |____/\____ | |__|\____/ 
                           \/                 \/           \/        \/                 \/            
            ", Color.OrangeRed, Color.LimeGreen, 14);

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(Environment.NewLine);
            ServiceAccount.Initialize(args);
            Console.WriteLine(Assembly.GetExecutingAssembly().GetName().Name + " is now starting..." + Environment.NewLine);
            CmdHandler.Initialize();
        }

        /// <summary>
        ///     Updates the console title.
        /// </summary>
        internal static void UpdateConsoleTitle()
        {
            Console.Title = "Clashers Republic - " + Assembly.GetExecutingAssembly().GetName().Name + " - ServerID: " + ServiceCore.ServiceNodeId + " - Sessions: " + NetAccountSessionManager.TotalSessions + " - Accounts: " + AccountManager.TotalAccounts;
        }
    }
}