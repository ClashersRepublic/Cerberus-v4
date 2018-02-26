namespace ClashersRepublic.Magic.Services.Core.Network
{
    using ClashersRepublic.Magic.Services.Core.Libs.NetMQ;
    using ClashersRepublic.Magic.Services.Core.Libs.NetMQ.Sockets;

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

        /// <summary>
        ///     Initializes a new instance of the <see cref="NetSocket"/> class.
        /// </summary>
        public NetSocket(int type, int id, string socket)
        {
            this.Type = type;
            this.Id = id;

            this.Socket = new DealerSocket(">tcp://" + socket + ":" + NetUtils.GetNetPort(type));
        }

        /// <summary>
        ///     Sends the bufffer to the socket.
        /// </summary>
        public void Send(byte[] buffer, int length)
        {
            if (!this.Socket.TrySendFrame(buffer, length))
            {
                Logging.Error(this, "NetSocket::send");
            }
        }
    }
}