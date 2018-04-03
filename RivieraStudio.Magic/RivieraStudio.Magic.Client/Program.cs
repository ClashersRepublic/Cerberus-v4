namespace ClashersRepublic.Magic.Client
{
    using System;
    using System.Drawing;
    using System.Reflection;
    using ClashersRepublic.Magic.Client.Debug;
    using ClashersRepublic.Magic.Client.Game;
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
            ClientManager.Initialize();
            GameMain.Initialize();

            while (true)
            {
                string cmd = Console.ReadLine();

                if (cmd.StartsWith("/"))
                {
                    switch (cmd.Substring(1))
                    {
                        case "create":
                        {
                            for (int i = 0; i < 5000; i++)
                            {
                                ClientManager.Create();
                            }

                            break;
                        }

                        case "test":
                        {
                            ClientManager.ForEeach(main => main.ServerConnection.MessageManager.SendMessage(new KeepAliveMessage()));
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