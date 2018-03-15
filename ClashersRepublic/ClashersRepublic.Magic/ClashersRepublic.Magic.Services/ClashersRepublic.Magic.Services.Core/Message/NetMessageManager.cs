namespace ClashersRepublic.Magic.Services.Core.Message
{
    using System;
    using ClashersRepublic.Magic.Services.Core.Network;

    public class NetMessageManager
    {
        /// <summary>
        ///     Receives the <see cref="NetMessage"/>.
        /// </summary>
        public virtual void ReceiveMessage(NetMessage message)
        {
            // ReceiveMessage.
        }

        /// <summary>
        ///     Sends the <see cref="NetMessage"/> to the specified <see cref="NetSocket"/>.
        /// </summary>
        public static void SendMessage(int serviceNodeType, int serviceNodeId, NetMessage message)
        {
            NetMessaging.Send(serviceNodeType, serviceNodeId, message);
        }

        /// <summary>
        ///     Sends the <see cref="NetMessage"/> to the specified <see cref="NetSocket"/>.
        /// </summary>
        public static void SendMessage(int serviceNodeType, int serviceNodeId, byte[] sessionId, NetMessage message)
        {
            NetMessaging.Send(serviceNodeType, serviceNodeId, message, sessionId);
        }

        /// <summary>
        ///     Sends the <see cref="NetMessage"/> to the specified <see cref="NetSocket"/>.
        /// </summary>
        public static void SendMessage(NetSocket socket, NetMessage message)
        {
            NetMessaging.Send(socket, message);
        }

        /// <summary>
        ///     Sends the <see cref="NetMessage"/> to the specified <see cref="NetSocket"/>.
        /// </summary>
        public static void SendMessage(NetSocket socket, byte[] sessionId, NetMessage message)
        {
            if (sessionId == null)
            {
                throw new ArgumentNullException("sessionId");
            }

            NetMessaging.Send(socket, message, sessionId);
        }
    }
}