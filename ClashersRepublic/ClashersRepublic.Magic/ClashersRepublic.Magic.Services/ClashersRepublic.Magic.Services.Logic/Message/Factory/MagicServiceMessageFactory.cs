namespace ClashersRepublic.Magic.Services.Logic.Message.Factory
{
    using ClashersRepublic.Magic.Services.Logic.Message.Account;

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
                    case 10105:
                    {
                        message = new CreateAccountMessage();
                        break;
                    }
                }
            }
            else
            {
                switch (type)
                {
                    case 20101:
                    {
                        message = new CreateAccountOkMessage();
                        break;
                    }

                    case 20102:
                    {
                        message = new CreateAccountFailedMessage();
                        break;
                    }
                }
            }

            return message;
        }
    }
}