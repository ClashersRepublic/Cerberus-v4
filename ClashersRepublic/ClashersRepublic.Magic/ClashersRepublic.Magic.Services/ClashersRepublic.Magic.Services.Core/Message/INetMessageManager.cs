namespace ClashersRepublic.Magic.Services.Core.Message
{
    using ClashersRepublic.Magic.Services.Core.Network;

    public interface INetMessageManager
    {
        void ReceiveMessage(NetMessage message);
    }
}