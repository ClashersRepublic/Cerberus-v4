namespace ClashersRepublic.Magic.Logic.Message.Factory
{
    public class LogicMessageFactory
    {
        public static readonly LogicMessageFactory Instance = new LogicMessageFactory();

        /// <summary>
        ///     Creates a message by type.
        /// </summary>
        public virtual PiranhaMessage CreateMessageByType(int type)
        {
            PiranhaMessage message = null;

            if (type < 20000)
            {
            }

            return message;
        }
    }
}