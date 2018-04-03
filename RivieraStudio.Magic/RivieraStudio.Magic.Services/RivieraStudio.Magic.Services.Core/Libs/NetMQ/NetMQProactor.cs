namespace RivieraStudio.Magic.Services.Core.Libs.NetMQ
{
    using System;
    using RivieraStudio.Magic.Services.Core.Libs.NetMQ.Sockets;

    /// <summary>
    ///     Class to quickly handle incoming messages of socket.
    ///     New thread is created to handle the messages. Call dispose to stop the thread.
    ///     Provided socket will not be disposed by the class.
    /// </summary>
    public class NetMQProactor : IDisposable
    {
        private readonly NetMQActor m_actor;
        private readonly NetMQSocket m_receiveSocket;
        private readonly Action<NetMQSocket, NetMQMessage> m_handler;
        private NetMQPoller m_poller;

        /// <summary>
        ///     Create NetMQProactor and start dedicate thread to handle incoming messages.
        /// </summary>
        /// <param name="receiveSocket">Socket to handle messages from</param>
        /// <param name="handler">Handler to handle incoming messages</param>
        public NetMQProactor(NetMQSocket receiveSocket, Action<NetMQSocket, NetMQMessage> handler)
        {
            this.m_receiveSocket = receiveSocket;
            this.m_handler = handler;
            this.m_actor = NetMQActor.Create(this.Run);
        }

        /// <summary>
        ///     Stop the proactor. Provided socket will not be disposed.
        /// </summary>
        public void Dispose()
        {
            this.m_actor.Dispose();
        }

        private void Run(PairSocket shim)
        {
            shim.ReceiveReady += this.OnShimReady;
            this.m_receiveSocket.ReceiveReady += this.OnSocketReady;
            this.m_poller = new NetMQPoller {this.m_receiveSocket, shim};

            shim.SignalOK();
            this.m_poller.Run();

            this.m_receiveSocket.ReceiveReady -= this.OnSocketReady;
        }

        private void OnShimReady(object sender, NetMQSocketEventArgs e)
        {
            string command = e.Socket.ReceiveFrameString();
            if (command == NetMQActor.EndShimMessage)
            {
                this.m_poller.Stop();
            }
        }

        private void OnSocketReady(object sender, NetMQSocketEventArgs e)
        {
            NetMQMessage message = this.m_receiveSocket.ReceiveMultipartMessage();

            this.m_handler(this.m_receiveSocket, message);
        }
    }
}