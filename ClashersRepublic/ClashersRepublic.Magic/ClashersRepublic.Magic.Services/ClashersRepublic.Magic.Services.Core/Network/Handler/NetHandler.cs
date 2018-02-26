namespace ClashersRepublic.Magic.Services.Core.Network.Handler
{
    using System.Collections.Concurrent;
    using System.Threading;

    using ClashersRepublic.Magic.Services.Core.Libs.NetMQ;
    using ClashersRepublic.Magic.Services.Core.Message;
    using ClashersRepublic.Magic.Titan.Util;

    public class NetHandler
    {
        private readonly Thread _sendThread;
        private readonly Thread _receiveThread;
        private readonly ConcurrentQueue<SendItem> _sendQueue;
        private readonly ConcurrentQueue<NetPacket> _receiveQueue;

        private INetMessageManager _messageManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetHandler"/> class.
        /// </summary>
        public NetHandler()
        {
            this._sendThread = new Thread(this.SendTask);
            this._receiveThread = new Thread(this.ReceiveTask);
            this._sendQueue = new ConcurrentQueue<SendItem>();
            this._receiveQueue = new ConcurrentQueue<NetPacket>();

            this._sendThread.Start();
            this._receiveThread.Start();
        }

        /// <summary>
        ///     Task for the receive <see cref="Thread"/>.
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
                }

                Thread.Sleep(1);
            }
        }

        /// <summary>
        ///     Tasks for the send <see cref="Thread"/>.
        /// </summary>
        private void SendTask()
        {
            while (true)
            {
                while (this._sendQueue.TryDequeue(out SendItem item))
                {
                    NetMessaging.InternalSend(item.DestinationSocket, item.Packet);
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
        ///     Sends the specified packet.
        /// </summary>
        internal void Send(NetSocket socket, NetPacket packet)
        {
            this._sendQueue.Enqueue(new SendItem(packet, socket));
        }

        /// <summary>
        ///     Sets the <see cref="INetMessageManager"/> instance.
        /// </summary>
        internal void SetMessageManager(INetMessageManager manager)
        {
            this._messageManager = manager;
        }

        private struct SendItem
        {
            internal readonly NetPacket Packet;
            internal readonly NetSocket DestinationSocket;

            /// <summary>
            ///     Initializes a new instance of the <see cref="SendItem"/> struct.
            /// </summary>
            internal SendItem(NetPacket packet, NetSocket socket)
            {
                this.Packet = packet;
                this.DestinationSocket = socket;
            }
        }
    }
}