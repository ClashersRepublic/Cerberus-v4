﻿namespace RivieraStudio.Magic.Titan.Util
{
    using System;

    public static class LogicTimeUtil
    {
        private static readonly DateTime Unix = new DateTime(1970, 1, 1);

        /// <summary>
        ///     Gets the current timestamp.
        /// </summary>
        public static int GetTimestamp()
        {
            return (int) DateTime.UtcNow.Subtract(LogicTimeUtil.Unix).TotalSeconds;
        }

        /// <summary>
        ///     Gets the current timestamp.
        /// </summary>
        public static int GetTimestamp(DateTime time)
        {
            return (int) time.Subtract(LogicTimeUtil.Unix).TotalSeconds;
        }

        /// <summary>
        ///     Gets the current timestamp.
        /// </summary>
        public static string GetTimestampMS()
        {
            return DateTime.UtcNow.Subtract(LogicTimeUtil.Unix).TotalMilliseconds.ToString("#");
        }

        /// <summary>
        ///     Gets the <see cref="DateTime"/> instance from timestamp.
        /// </summary>
        public static DateTime GetDateTimeFromTimestamp(int timestamp)
        {
            return LogicTimeUtil.Unix.AddSeconds(timestamp);
        }
    }
}