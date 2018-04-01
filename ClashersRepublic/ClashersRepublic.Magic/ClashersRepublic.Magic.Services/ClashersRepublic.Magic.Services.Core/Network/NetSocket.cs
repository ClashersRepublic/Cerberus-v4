namespace ClashersRepublic.Magic.Services.Core.Network
{
    using ClashersRepublic.Magic.Services.Core.Utils;
    using ClashersRepublic.Magic.Services.Net;

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
        public NetClient Socket { get; }
        
        /// <summary>
        ///     Initializes a new instance of the <see cref="NetSocket" /> class.
        /// </summary>
        public NetSocket(int type, int id, string host)
        {
            this.Type = type;
            this.Id = id;

            this.Socket = new NetClient(host, NetUtils.GetNetPort(type, id));
        }
        
        /// <summary>
        ///     Sends the bufffer to the socket.
        /// </summary>
        public void Send(byte[] buffer, int length)
        {
            this.Socket.Send(buffer, length);
        }
    }
}