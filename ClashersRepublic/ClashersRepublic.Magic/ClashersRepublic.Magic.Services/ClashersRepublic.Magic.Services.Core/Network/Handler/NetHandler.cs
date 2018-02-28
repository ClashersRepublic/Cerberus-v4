namespace ClashersRepublic.Magic.Services.Core.Network.Handler
{
    using System.Collections.Concurrent;
    using System.Threading;
    using ClashersRepublic.Magic.Services.Core.Message;
    using ClashersRepublic.Magic.Titan.Util;

    public class NetHandler
    {
        private readonly Thread _sendThread;
        private readonly Thread _receiveThread;
        private readonly ConcurrentQueue<NetPacket> _receiveQueue;
        private readonly NetSocket[] _sockets;

        private INetMessageManager _messageManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetHandler" /> class.
        /// </summary>
        public NetHandler()
        {
            this._sendThread = new Thread(this.SendTask);
            this._receiveThread = new Thread(this.ReceiveTask);
            this._receiveQueue = new ConcurrentQueue<NetPacket>();
            this._sockets = NetManager.Get();

            this._sendThread.Start();
            this._receiveThread.Start();
        }

        /// <summary>
        ///     Task for the receive <see cref="Thread" />.
        /// </summary>
        private void ReceiveTask()
        {
            while (true)
            {
                while (this._receiveQueue.TryDequeue(out NetPacket packet))
                {
                    LogicArrayList<NetMessage> messages = packet.GetNetMessages();

                    for (int i = 0; i < packet.GetNetMessageCount(); i++)
                    {
                        messages[i].Decode();
                        this._messageManager.ReceiveMessage(messages[i]);
                    }

                    packet.Destruct();
                }

                Thread.Sleep(1);
            }
        }

        /// <summary>
        ///     Tasks for the send <see cref="Thread" />.
        /// </summary>
        private void SendTask()
        {
            while (true)
            {
                for (int i = 0; i < this._sockets.Length; i++)
                {
                    this._sockets[i].OnWakeup();
                }

                Thread.Sleep(1);
            }
        }

        /// <summary>
        ///     Receive the specified packet.
        /// </summary>
        internal void Receive(NetPacket packet)
        {
            this._receiveQueue.Enqueue(packet);
        }
        
        /// <summary>
        ///     Sets the <see cref="INetMessageManager" /> instance.
        /// </summary>
        internal void SetMessageManager(INetMessageManager manager)
        {
            this._messageManager = manager;
        }
    }
}