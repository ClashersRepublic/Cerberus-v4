namespace ClashersRepublic.Magic.Services.Core.Network
{
    using System.Collections.Concurrent;
    using ClashersRepublic.Magic.Services.Core.Libs.NetMQ;
    using ClashersRepublic.Magic.Services.Core.Libs.NetMQ.Sockets;
    using ClashersRepublic.Magic.Services.Core.Message;
    using ClashersRepublic.Magic.Titan.DataStream;

    public class NetSocket
    {
        /// <summary>
        ///     Gets the service node type.
        /// </summary>
        public int Type { get; }

        /// <summary>
        ///     Gets the service node id.
        /// </summary>
        public int Id { get; }

        /// <summary>
        ///     Gets the service node socket.
        /// </summary>
        public NetMQSocket Socket { get; }

        private ByteStream _stream;
        private ConcurrentQueue<NetMessage> _messages;

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetSocket" /> class.
        /// </summary>
        public NetSocket(int type, int id, string socket)
        {
            this.Type = type;
            this.Id = id;

            this.Socket = new DealerSocket(">tcp://" + socket + ":" + NetUtils.GetNetPort(type));
            this._stream = new ByteStream(100);
            this._messages = new ConcurrentQueue<NetMessage>();
        }

        /// <summary>
        ///     Adds the specified <see cref="NetMessage"/> for send.
        /// </summary>
        public void AddMessage(NetMessage message)
        {
            this._messages.Enqueue(message);
        }

        /// <summary>
        ///     Called for send all messages in queue.
        /// </summary>
        public void OnWakeup()
        {
            if (this._messages.Count != 0)
            {
                NetPacket packet = new NetPacket();

                while (this._messages.TryDequeue(out NetMessage message))
                {
                    packet.AddMessage(message);
                }
                

                packet.Encode(this._stream);
                this.Send(this._stream.GetByteArray(), this._stream.GetOffset());
                packet.Destruct();
                this._stream.ResetOffset();
            }
        }

        /// <summary>
        ///     Sends the bufffer to the socket.
        /// </summary>
        public void Send(byte[] buffer, int length)
        {
            this.Socket.SendFrame(buffer, length);
        }
    }
}