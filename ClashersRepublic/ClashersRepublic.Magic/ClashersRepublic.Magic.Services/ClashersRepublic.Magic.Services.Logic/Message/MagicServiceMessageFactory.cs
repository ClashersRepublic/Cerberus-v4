namespace ClashersRepublic.Magic.Services.Logic.Message
{
    using ClashersRepublic.Magic.Services.Logic.Message.Account;
    using ClashersRepublic.Magic.Services.Logic.Message.Avatar;
    using ClashersRepublic.Magic.Services.Logic.Message.Session;

    public sealed class MagicServiceMessageFactory
    {
        public static readonly MagicServiceMessageFactory Instance = new MagicServiceMessageFactory();

        /// <summary>
        ///     Creates a message by type.
        /// </summary>
        public MagicServiceMessage CreateMessageByType(int type)
        {
            MagicServiceMessage message = null;

            if (type < 20000)
            {
                switch (type)
                {
                    case 10150:
                    {
                        message = new LoginAccountMessage();
                        break;
                    }

                    case 10200:
                    {
                        message = new CreateAvatarMessage();
                        break;
                    }

                    case 10500:
                    {
                        message = new CreateSessionMessage();
                        break;
                    }

                    case 10501:
                    {
                        message = new ForwardClientMessage();
                        break;
                    }
                }
            }
            else
            {
                switch (type)
                {

                }
            }

            return message;
        }
    }
}