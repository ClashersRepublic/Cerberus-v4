﻿namespace ClashersRepublic.Magic.Logic.Message
{
    using ClashersRepublic.Magic.Logic.Message.Account;
    using ClashersRepublic.Magic.Titan.Message;

    public sealed class LogicMagicMessageFactory : LogicMessageFactory
    {
        public static readonly LogicMessageFactory Instance = new LogicMagicMessageFactory();
        public const string RC4_KEY = "fhsd6f86f67rt8fw78fw789we78r9789wer6re";

        /// <summary>
        ///     Creates a message by type.
        /// </summary>
        public override PiranhaMessage CreateMessageByType(int type)
        {
            PiranhaMessage message = null;

            if (type < 20000)
            {
                switch (type)
                {
                    case 10101:
                    {
                        message = new LoginMessage();
                        break;
                    }

                    case 10108:
                    {
                        message = new KeepAliveMessage();
                        break;
                    }
                }
            }
            else
            {
                switch (type)
                {
                    case 20103:
                    {
                        message = new LoginFailedMessage();
                        break;
                    }

                    case 20108:
                    {
                        message = new KeepAliveServerMessage();
                        break;
                    }
                }
            }

            return message;
        }
    }
}