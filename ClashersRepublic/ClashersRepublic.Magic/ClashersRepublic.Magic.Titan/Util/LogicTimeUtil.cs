namespace ClashersRepublic.Magic.Titan.Util
{
    using System;

    public static class LogicTimeUtil
    {
        private static readonly DateTime _unix = new DateTime(1970, 1, 1);

        /// <summary>
        ///     Gets the current timestamp.
        /// </summary>
        public static int GetTimestamp()
        {
            return (int) DateTime.UtcNow.Subtract(LogicTimeUtil._unix).TotalSeconds;
        }
    }
}