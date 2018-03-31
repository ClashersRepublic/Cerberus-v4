namespace ClashersRepublic.Magic.Services.Chat
{
    using System;
    using System.Drawing;
    using System.Reflection;
    
    using ClashersRepublic.Magic.Services.Core;
    using ClashersRepublic.Magic.Services.Chat.Game;
    using ClashersRepublic.Magic.Services.Chat.Handler;
    using ClashersRepublic.Magic.Services.Chat.Network.Session;

    internal class Program
    {
        private const int Width = 120;
        private const int Height = 30;

        private static void Main(string[] args)
        {
            Console.SetOut(new ConsoleOut());
            Console.SetWindowSize(Program.Width, Program.Height);

            Core.Libs.Colorful.Console.WriteWithGradient(@"
     _________.__                 .__                          __________                   ___.   .__.__        
     \_   ___ \|  | _____    _____|  |__   ___________  ______ \______   \ ____ ______  __ _\_ |__ |  | |__| ____  
     /    \  \/|  | \__  \  /  ___/  |  \_/ __ \_  __ \/  ___/  |       _// __ \\____ \|  |  \ __ \|  | |  |/ ___\ 
     \     \___|  |__/ __ \_\___ \|   Y  \  ___/|  | \/\___ \   |    |   \  ___/|  |_> >  |  / \_\ \  |_|  \  \___ 
      \______  /____(____  /____  >___|  /\___  >__|  /____  >  |____|_  /\___  >   __/|____/|___  /____/__|\___  >
             \/          \/     \/     \/     \/           \/          \/     \/|__|             \/             \/ 
            ", Color.OrangeRed, Color.LimeGreen, 14);

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine(Environment.NewLine);
            ServiceGlobalChat.Initialize(args);
            Console.WriteLine(Assembly.GetExecutingAssembly().GetName().Name + " is now starting..." + Environment.NewLine);
            CmdHandler.Initialize();
        }

        /// <summary>
        ///     Updates the console title.
        /// </summary>
        internal static void UpdateConsoleTitle()
        {
            Console.Title = "Clashers Republic - " + Assembly.GetExecutingAssembly().GetName().Name + " - ServerID: " + ServiceCore.ServiceNodeId + " - Sessions: " + NetGlobalChatSessionManager.TotalSessions + " - Rooms: " + RoomManager.TotalRooms;
        }
    }
}