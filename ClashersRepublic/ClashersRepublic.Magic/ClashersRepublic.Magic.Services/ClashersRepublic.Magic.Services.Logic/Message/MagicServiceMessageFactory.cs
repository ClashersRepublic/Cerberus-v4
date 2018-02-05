namespace ClashersRepublic.Magic.Services.Logic.Message
{
    using ClashersRepublic.Magic.Services.Logic.Message.Client;
    using ClashersRepublic.Magic.Services.Logic.Message.Messaging;
    using ClashersRepublic.Magic.Services.Logic.Message.Session;

    public sealed class MagicServiceMessageFactory
    {
        public static readonly MagicServiceMessageFactory Instance = new MagicServiceMessageFactory();

        /// <summary>
        ///     Creates a message by type.
        /// </summary>
        public ServiceMessage CreateMessageByType(int type)
        {
            ServiceMessage message = null;

            if (type < 20000)
            {
                switch (type)
                {
                    case 10140:
                    {
                        message = new ForwardClientMessage();
                        break;
                    }

                    case 10198:
                    {
                        message = new ClientConnectedMessage();
                        break;
                    }

                    case 10199:
                    {
                        message = new ClientDisconnectedMessage();
                        break;
                    }
                }
            }
            else
            {
                switch (type)
                {
                    case 20110:
                    {
                        message = new SessionClosedMessage();
                        break;
                    }

                    case 20140:
                    {
                        message = new ForwardServerMessage();
                        break;
                    }
                }
            }

            return message;
        }
    }
}