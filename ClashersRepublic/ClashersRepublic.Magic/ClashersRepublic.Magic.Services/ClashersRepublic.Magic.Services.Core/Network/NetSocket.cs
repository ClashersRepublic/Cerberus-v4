namespace ClashersRepublic.Magic.Services.Core.Network
{
    using System;
    using ClashersRepublic.Magic.Services.Core.Utils;
    using ClashersRepublic.Magic.Services.Net;
    using ClashersRepublic.Magic.Services.Net.ClientSocket;

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
        public NetTcpClientSocket Socket { get; }
        
        /// <summary>
        ///     Initializes a new instance of the <see cref="NetSocket" /> class.
        /// </summary>
        public NetSocket(int type, int id, string host)
        {
            this.Type = type;
            this.Id = id;

            this.Socket = new NetTcpClientSocket(host, NetUtils.GetNetPort(type, id));
        }
        
        /// <summary>
        ///     Sends the bufffer to the socket.
        /// </summary>
        public void Send(byte[] buffer, int length)
        {
            byte[] packet = new byte[4 + length];

            packet[0] = (byte) (length >> 24);
            packet[1] = (byte) (length >> 16);
            packet[2] = (byte) (length >> 8);
            packet[3] = (byte) (length);

            Array.Copy(buffer, 0, packet, 4, length);

            this.Socket.Send(packet, length + 4);
        }
    }
}