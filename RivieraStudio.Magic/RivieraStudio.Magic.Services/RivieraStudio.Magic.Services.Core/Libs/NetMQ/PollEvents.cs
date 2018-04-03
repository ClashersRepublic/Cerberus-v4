﻿namespace RivieraStudio.Magic.Services.Core.Libs.NetMQ
{
    using System;

    /// <summary>
    ///     This flags enum-type is simply an indication of the direction of the poll-event,
    ///     and can be None, PollIn, PollOut, or PollError.
    /// </summary>
    [Flags]
    public enum PollEvents
    {
        /// <summary>
        /// </summary>
        None = 0x0,

        /// <summary>
        /// </summary>
        PollIn = 0x1,

        /// <summary>
        /// </summary>
        PollOut = 0x2,

        /// <summary>
        /// </summary>
        PollError = 0x4
    }

    /// <summary>
    ///     Extension methods for the <see cref="PollEvents" /> enum.
    /// </summary>
    public static class PollEventsExtensions
    {
        /// <summary>Test whether <paramref name="pollEvents" /> has the <see cref="PollEvents.PollIn" /> flag set.</summary>
        public static bool HasIn(this PollEvents pollEvents)
        {
            return (pollEvents & PollEvents.PollIn) == PollEvents.PollIn;
        }

        /// <summary>Test whether <paramref name="pollEvents" /> has the <see cref="PollEvents.PollOut" /> flag set.</summary>
        public static bool HasOut(this PollEvents pollEvents)
        {
            return (pollEvents & PollEvents.PollOut) == PollEvents.PollOut;
        }

        /// <summary>Test whether <paramref name="pollEvents" /> has the <see cref="PollEvents.PollError" /> flag set.</summary>
        public static bool HasError(this PollEvents pollEvents)
        {
            return (pollEvents & PollEvents.PollError) == PollEvents.PollError;
        }
    }
}