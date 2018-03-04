namespace ClashersRepublic.Magic.Services.Proxy
{
    using System.Timers;
    using ClashersRepublic.Magic.Logic.Data;

    using ClashersRepublic.Magic.Services.Core;
    using ClashersRepublic.Magic.Services.Proxy.Network;
    using ClashersRepublic.Magic.Services.Proxy.Network.Handler;
    using ClashersRepublic.Magic.Services.Proxy.Network.Message;
    using ClashersRepublic.Magic.Services.Proxy.Network.Session;

    using ClashersRepublic.Magic.Titan.Math;
    using ClashersRepublic.Magic.Titan.Util;

    internal static class ServiceProxy
    {
        private const int ServiceNodeType = 1;

        internal static Timer TitleTimer;
        internal static LogicRandom Random;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize(string[] args)
        {
            ServiceCore.Initialize(ServiceProxy.ServiceNodeType, new NetProxyMessageManager(), args);

            ServiceProxy.InitLogic();
            ServiceProxy.InitNetwork();

            ServiceProxy.Random = new LogicRandom(LogicTimeUtil.GetTimestamp());
            ServiceProxy.TitleTimer = new Timer(50);
            ServiceProxy.TitleTimer.Elapsed += (sender, eventArgs) => Program.UpdateConsoleTitle();
            ServiceProxy.TitleTimer.Start();

            ServiceCore.Start();
        }

        /// <summary>
        ///     Initializes the logic part.
        /// </summary>
        internal static void InitLogic()
        {
            LogicDataTables.Initialize();
        }
        
        /// <summary>
        ///     Initializes the network part.
        /// </summary>
        internal static void InitNetwork()
        {
            NetProxySessionManager.Initialize();
            NetworkManager.Initialize();
            MessageHandler.Initialize();
        }
    }
}