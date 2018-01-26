namespace ClashersRepublic.Magic.Titan.Message
{
    public class LogicMessageFactory
    {
        public static readonly LogicMessageFactory Instance = new LogicMessageFactory();

        /// <summary>
        ///     Creates a message by type.
        /// </summary>
        public virtual PiranhaMessage CreateMessageByType(int type)
        {
            return null;
        }
    }
}