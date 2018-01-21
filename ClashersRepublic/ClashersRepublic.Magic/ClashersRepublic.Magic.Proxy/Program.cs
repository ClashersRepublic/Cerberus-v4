namespace ClashersRepublic.Magic.Proxy
{
    using System;
    using System.Drawing;
    using System.Reflection;
    using System.Threading;

    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Proxy.Handler;
    using ClashersRepublic.Magic.Proxy.Network;
    using ClashersRepublic.Magic.Proxy.Service;

    internal class Program
    {
        private const int Width = 120;
        private const int Height = 30;

        private static void Main(string[] args)
        {
            Console.Title = "Clashers Republic - " + Assembly.GetExecutingAssembly().GetName().Name;
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
            LogicDataTables.Initialize();
            ServiceConnectionProcessor.Initialize();
            ServiceConnection.Initialize();
            NetworkProcessor.Initialize();
            NetworkGateway.Initialize();
            ExitHandler.Initialize();
            
            Thread.Sleep(-1);
        }
    }
}