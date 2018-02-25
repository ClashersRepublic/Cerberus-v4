namespace ClashersRepublic.Magic.Services.Proxy
{
    using System.Timers;
    using ClashersRepublic.Magic.Logic.Data;

    using ClashersRepublic.Magic.Services.Core;
    using ClashersRepublic.Magic.Services.Core.Network;

    using ClashersRepublic.Magic.Services.Proxy.Network;
    using ClashersRepublic.Magic.Services.Proxy.Network.Handler;
    using ClashersRepublic.Magic.Services.Proxy.Network.Message;

    using ClashersRepublic.Magic.Titan.Math;
    using ClashersRepublic.Magic.Titan.Util;

    internal static class ServiceProxy
    {
        private const int ServiceNodeType = 1;

        internal static LogicRandom Random;
        internal static Timer TitleTimer;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize(string[] args)
        {
            ServiceCore.Initialize(ServiceProxy.ServiceNodeType, new NetMessageManager(), args);
            ServiceProxy.InitLogic();
            ServiceProxy.InitNetwork();

            ServiceProxy.TitleTimer = new Timer(50);
            ServiceProxy.TitleTimer.Elapsed += (sender, eventArgs) => Program.UpdateConsoleTitle();
            ServiceProxy.TitleTimer.Start();
        }

        /// <summary>
        ///     Initializes the logic part.
        /// </summary>
        internal static void InitLogic()
        {
            LogicDataTables.Initialize();
            ServiceProxy.Random = new LogicRandom(LogicTimeUtil.GetTimestamp());
        }

        /// <summary>
        ///     Initializes the network part.
        /// </summary>
        internal static void InitNetwork()
        {
            NetworkManager.Initialize();
            MessageHandler.Initialize();
        }
    }
}