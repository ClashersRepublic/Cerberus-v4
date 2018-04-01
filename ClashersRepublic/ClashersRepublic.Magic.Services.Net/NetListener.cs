namespace ClashersRepublic.Magic.Services.Net
{
    public class NetListener
    {
        /// <summary>
        ///     Called when a packet is received.
        /// </summary>
        public virtual int OnReceive(byte[] buffer, int length)
        {
            return 0;
        }
    }
}