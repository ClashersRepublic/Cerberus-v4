namespace ClashersRepublic.Magic.Services.Logic.Message
{
    using ClashersRepublic.Magic.Services.Logic.Message.Client;
    using ClashersRepublic.Magic.Services.Logic.Message.Debug;
    using ClashersRepublic.Magic.Services.Logic.Message.Home;
    using ClashersRepublic.Magic.Services.Logic.Message.Messaging;
    using ClashersRepublic.Magic.Services.Logic.Message.Network;

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
                    case 10108:
                    {
                        message = new KeepAliveMessage();
                        break;
                    }

                    case 10130:
                    {
                        message = new AskForMaintenanceStateMessage();
                        break;
                    }
                        
                    case 10140:
                    {
                        message = new ForwardServerMessage();
                        break;
                    }

                    case 10141:
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
                    case 20108:
                    {
                        message = new KeepAliveServerMessage();
                        break;
                    }

                    case 20130:
                    {
                        message = new MaintenanceStateDataMessage();
                        break;
                    }

                    case 20190:
                    {
                        message = new UpdateHomeQueueNameMessage();
                        break;
                    }
                }
            }

            return message;
        }
    }
}