﻿namespace LineageSoft.Magic.Services.Core.Libs.NetMQ
{
    using System;
    using System.Threading;
    using JetBrains.Annotations;

    /// <summary>
    ///     Facilitates a pattern whereby an event may be decorated with logic that transforms its arguments.
    /// </summary>
    /// <remarks>
    ///     Use of this class requires providing actions that register and unregister a handler of the source
    ///     event that calls <see cref="Fire" /> with updated arguments in response.
    /// </remarks>
    /// <typeparam name="T">Argument type of the decorated event.</typeparam>
    internal class EventDelegator<T> : IDisposable where T : EventArgs
    {
        private readonly Action m_registerToEvent;
        private readonly Action m_unregisterFromEvent;
        private EventHandler<T> m_event;
        private int m_counter;

        /// <summary>
        ///     Initialises a new instance.
        /// </summary>
        /// <param name="registerToEvent">an Action to perform when the first handler is registered for the event</param>
        /// <param name="unregisterFromEvent">an Action to perform when the last handler is unregistered from the event</param>
        public EventDelegator([NotNull] Action registerToEvent, [NotNull] Action unregisterFromEvent)
        {
            this.m_registerToEvent = registerToEvent;
            this.m_unregisterFromEvent = unregisterFromEvent;
        }

        public event EventHandler<T> Event
        {
            add
            {
                this.m_event += value;

                if (Interlocked.Increment(ref this.m_counter) == 1)
                {
                    this.m_registerToEvent();
                }
            }
            remove
            {
                this.m_event -= value;

                if (Interlocked.Decrement(ref this.m_counter) == 0)
                {
                    this.m_unregisterFromEvent();
                }
            }
        }

        /// <summary>
        ///     Raise, or "Fire", the Event.
        /// </summary>
        /// <param name="sender">the sender that the event-handler that gets notified of this event will receive</param>
        /// <param name="args">the subclass of EventArgs that the event-handler will receive</param>
        public void Fire([NotNull] object sender, [NotNull] T args)
        {
            EventHandler<T> temp = this.m_event;
            temp?.Invoke(sender, args);
        }

        public void Dispose()
        {
            if (this.m_counter != 0)
            {
                this.m_unregisterFromEvent();
                this.m_counter = 0;
            }
        }
    }
}