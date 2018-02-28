namespace ClashersRepublic.Magic.Services.Avatar
{
    using System;
    using System.Reflection;

    using ClashersRepublic.Magic.Services.Core;

    internal class Program
    {
        private const int Width = 120;
        private const int Height = 30;

        private static void Main(string[] args)
        {
        }

        /// <summary>
        ///     Updates the console title.
        /// </summary>
        internal static void UpdateConsoleTitle()
        {
            Console.Title = "Clashers Republic - " + Assembly.GetExecutingAssembly().GetName().Name + " - ServerID: " + ServiceCore.ServiceNodeId;
        }
    }
}