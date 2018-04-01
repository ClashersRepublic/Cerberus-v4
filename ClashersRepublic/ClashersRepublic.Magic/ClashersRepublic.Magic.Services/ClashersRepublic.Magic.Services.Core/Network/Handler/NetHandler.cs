namespace ClashersRepublic.Magic.Services.Core.Network.Handler
{
    using System.Collections.Concurrent;
    using System.Threading;

    using ClashersRepublic.Magic.Services.Core.Message;
    using ClashersRepublic.Magic.Titan.DataStream;

    public class NetHandler
    {
        private readonly Thread _sendThread;
        private readonly Thread _receiveThread;
        private readonly ConcurrentQueue<SendItem> _sendQueue;
        private readonly ConcurrentQueue<NetPacket> _receiveQueue;
        private readonly ByteStream _writeStream;

        private NetMessageManager _messageManager;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetHandler" /> class.
        /// </summary>
        public NetHandler()
        {
            this._sendThread = new Thread(this.SendTask);
            this._receiveThread = new Thread(this.ReceiveTask);
            this._receiveQueue = new ConcurrentQueue<NetPacket>();
            this._sendQueue = new ConcurrentQueue<SendItem>();
            this._writeStream = new ByteStream(20);

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
                    NetMessage message = packet.GetNetMessage();

                    if (message != null)
                    {
                        message.Decode();
                        this._messageManager.ReceiveMessage(message);
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
                while (this._sendQueue.TryDequeue(out SendItem item))
                {
                    NetSocket socket = item.Socket;
                    NetPacket packet = item.Packet;

                    packet.Encode(this._writeStream);
                    socket.Send(this._writeStream.GetByteArray(), this._writeStream.GetOffset());
                    packet.Destruct();

                    this._writeStream.SetOffset(0);
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
            this._sendQueue.Enqueue(new SendItem(socket, packet));
        }

        /// <summary>
        ///     Sets the <see cref="NetMessageManager" /> instance.
        /// </summary>
        internal void SetMessageManager(NetMessageManager manager)
        {
            this._messageManager = manager;
        }

        private struct SendItem
        {
            /// <summary>
            ///     Gets the <see cref="NetMessage"/> instance.
            /// </summary>
            internal NetPacket Packet { get; }

            /// <summary>
            ///     Gets the <see cref="NetSocket"/> instance.
            /// </summary>
            internal NetSocket Socket { get; }

            /// <summary>
            ///     Initializes a new instance of the <see cref="SendItem"/> struct.
            /// </summary>
            internal SendItem(NetSocket socket, NetPacket packet)
            {
                this.Socket = socket;
                this.Packet = packet;
            }
        }
    }
}