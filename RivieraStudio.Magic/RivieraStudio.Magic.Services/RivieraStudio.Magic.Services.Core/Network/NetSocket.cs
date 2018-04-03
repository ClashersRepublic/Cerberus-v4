namespace RivieraStudio.Magic.Services.Core.Network
{
    using System.Threading;
    using RivieraStudio.Magic.Services.Core.Libs.NetMQ;
    using RivieraStudio.Magic.Services.Core.Libs.NetMQ.Sockets;
    using RivieraStudio.Magic.Services.Core.Utils;

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
        ///     Initializes a new instance of the <see cref="NetSocket" /> class.
        /// </summary>
        public NetSocket(int type, int id, string socket)
        {
            this.Type = type;
            this.Id = id;

            this.Socket = new DealerSocket(">tcp://" + socket + ":" + NetUtils.GetNetPort(type, id));
        }
        
        /// <summary>
        ///     Sends the bufffer to the socket.
        /// </summary>
        public void Send(byte[] buffer, int length)
        {
            while (!this.Socket.TrySendFrame(buffer, length))
            {
                Thread.Sleep(2);
            }
        }
    }
}