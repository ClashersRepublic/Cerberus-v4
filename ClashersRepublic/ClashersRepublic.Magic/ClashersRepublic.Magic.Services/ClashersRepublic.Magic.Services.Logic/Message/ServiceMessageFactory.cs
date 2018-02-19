namespace ClashersRepublic.Magic.Services.Logic.Message
{
    using ClashersRepublic.Magic.Services.Logic.Message.Game;
    using ClashersRepublic.Magic.Services.Logic.Message.Messaging;
    using ClashersRepublic.Magic.Services.Logic.Message.Session;

    public static class ServiceMessageFactory
    {
        /// <summary>
        ///     Creates a message by type.
        /// </summary>
        public static ServiceMessage CreateMessageByType(int type)
        {
            ServiceMessage message = null;

            switch (type)
            {
                case 10100:
                {
                    message = new CreateDataMessage();
                    break;
                }

                case 10201:
                {
                    message = new ServiceNodeBoundToSessionMessage();
                    break;
                }

                case 10202:
                {
                    message = new ServiceNodeUnboundToSessionMessage();
                    break;
                }

                case 10300:
                {
                    message = new ForwardPiranhaMessage();
                    break;
                }
            }

            return message;
        }
    }
}