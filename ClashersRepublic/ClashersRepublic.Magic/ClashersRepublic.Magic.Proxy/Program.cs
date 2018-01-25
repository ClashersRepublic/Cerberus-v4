﻿namespace ClashersRepublic.Magic.Proxy
{
    using System;
    using System.Drawing;
    using System.Reflection;
    using System.Threading;

    using ClashersRepublic.Magic.Logic.Data;

    using ClashersRepublic.Magic.Proxy.Handler;
    using ClashersRepublic.Magic.Proxy.Network;
    using ClashersRepublic.Magic.Proxy.Service;

    using ClashersRepublic.Magic.Services.Logic;
    using ClashersRepublic.Magic.Services.Logic.Message.Account;
    using ClashersRepublic.Magic.Services.Logic.Resource;
    using ClashersRepublic.Magic.Services.Logic.Util;

    internal class Program
    {
        private const int Width = 120;
        private const int Height = 30;

        private static void Main(string[] args)
        {
            Console.SetOut(new ConsoleWriter());
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
            ConsoleWriter.WriteMiddle = false;

            Logging.Initialize();
            Config.Initialize();

            if (args.Length > 0)
            {
                Config.ServerId = int.Parse(args[0]);
            }

            LogicDataTables.Initialize();
            ResourceManager.Initialize();
            ServiceProcessor.Initialize();
            ServiceMessaging.Initialize();
            ServiceConnection.Initialize();
            NetworkProcessor.Initialize();
            NetworkGateway.Initialize();
            ExitHandler.Initialize();

            Program.UpdateConsoleTitle();
            Program.Test();

            Thread.Sleep(-1);
        }

        /// <summary>
        ///     Updates the console title.
        /// </summary>
        internal static void UpdateConsoleTitle()
        {
            Console.Title = "Clashers Republic - " + Assembly.GetExecutingAssembly().GetName().Name + " - " + Config.ServerId;
        }

        /// <summary>
        ///     Test part.
        /// </summary>
        internal static void Test()
        {
            for (int i = 0; i < 150000; i++)
            {
                ServiceMessaging.SendMessage(new CreateAccountMessage
                {
                    ProxySessionId = SessionUtil.CreateSessionId(Config.ServerId, i),
                    StartSession = false
                }, string.Empty, ServiceExchangeName.ACCOUNT_COMMON_QUEUE);
            }
        }
    }
}