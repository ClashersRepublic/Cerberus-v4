namespace ClashersRepublic.Magic.Services.Core.Message
{
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
        public static void SendMessage(NetSocket socket, NetMessage message)
        {
            NetMessaging.Send(socket, message);
        }

        /// <summary>
        ///     Sends the <see cref="NetMessage"/> to the specified <see cref="NetSocket"/>.
        /// </summary>
        public static void SendMessage(NetSocket socket, NetMessage message, byte[] sessionId, int sessionIdLength)
        {
            NetMessaging.Send(socket, message, sessionId, sessionIdLength);
        }
    }
}