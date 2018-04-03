namespace LineageSoft.Magic.Services.Proxy
{
    using System.Timers;
    using LineageSoft.Magic.Logic.Data;

    using LineageSoft.Magic.Services.Core;
    using LineageSoft.Magic.Services.Core.Utils;
    using LineageSoft.Magic.Services.Proxy.Network;
    using LineageSoft.Magic.Services.Proxy.Network.Message;
    using LineageSoft.Magic.Services.Proxy.Network.Session;

    using LineageSoft.Magic.Titan.Math;
    using LineageSoft.Magic.Titan.Util;

    internal static class ServiceProxy
    {
        internal static Timer TitleTimer;
        internal static LogicRandom Random;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize(string[] args)
        {
            ServiceCore.Initialize(NetUtils.SERVICE_NODE_TYPE_PROXY_CONTAINER, new NetProxyMessageManager(), args);

            ServiceProxy.InitLogic();
            ServiceProxy.InitNetwork();

            ServiceProxy.Random = new LogicRandom(LogicTimeUtil.GetTimestamp());

            ServiceProxy.TitleTimer = new Timer(200);
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
            NetworkMessagingManager.Initialize();
        }
    }
}