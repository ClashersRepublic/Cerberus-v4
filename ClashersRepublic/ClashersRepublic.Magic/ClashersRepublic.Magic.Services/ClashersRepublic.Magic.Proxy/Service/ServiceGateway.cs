namespace ClashersRepublic.Magic.Proxy.Service
{
    using System;
    using System.Threading;
    using ClashersRepublic.Magic.Proxy.Log;
    using NetMQ;
    using NetMQ.Sockets;

    internal class ServiceGateway
    {
        private NetMQSocket _client;
        private Thread _receiveThread;
        /// <summary>
        ///     Initializes a new instance of the <see cref="ServiceGateway"/> class.
        /// </summary>
        internal ServiceGateway(int port)
        {
            this._client = new DealerSocket("@tcp://*:" + port);

            Logging.Debug(typeof(ServiceGateway), "Service listens on port " + port);

            this._receiveThread = new Thread(this.Receive);
            this._receiveThread.Start();        }

        /// <summary>
        ///     Receives bytes.
        /// </summary>
        private void Receive()
        {
            while (true)
            {
                this.ProcessReceive();
            }
        }

        /// <summary>
        ///     Processes the receive bytes.
        /// </summary>
        private void ProcessReceive()
        {
            byte[] buffer = this._client.ReceiveFrameBytes(out bool more);

            if (buffer != null)
            {
                ServiceMessaging.OnReceive(buffer, buffer.Length);

                if (more)
                {
                    this.ProcessReceive();
                }
            }
        }
        
        /// <summary>
        ///     Sends the message to specified socket.
        /// </summary>
        internal static void Send(byte[] buffer, int length, NetMQSocket socket)
        {
            if (!socket.TrySendFrame(buffer, length))
            {
                Logging.Error(typeof(ServiceGateway), "ServiceGateway::send send failed");
            }
        }
    }
}