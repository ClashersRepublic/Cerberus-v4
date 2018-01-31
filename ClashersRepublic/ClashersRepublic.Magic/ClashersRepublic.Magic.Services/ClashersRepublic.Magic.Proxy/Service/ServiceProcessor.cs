namespace ClashersRepublic.Magic.Proxy.Service
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;

    using ClashersRepublic.Magic.Proxy.Debug;

    internal static class ServiceProcessor
    {
        private static Thread[] _threads;
        private static ConcurrentQueue<Action>[] _actions;
        private static AutoResetEvent[] _events;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            ServiceProcessor._threads = new Thread[2];
            ServiceProcessor._actions = new ConcurrentQueue<Action>[2];
            ServiceProcessor._events = new AutoResetEvent[2];

            for (int i = 0; i < 2; i++)
            {
                int index = i;

                ServiceProcessor._events[index] = new AutoResetEvent(false);
                ServiceProcessor._actions[index] = new ConcurrentQueue<Action>();
                ServiceProcessor._threads[index] = new Thread(() => ServiceProcessor.Action(index));
                ServiceProcessor._threads[index].Start();
            }
        }

        /// <summary>
        ///     Tasks for thread.
        /// </summary>
        private static void Action(int index)
        {
            ConcurrentQueue<Action> queue = ServiceProcessor._actions[index];
            AutoResetEvent queueEvent = ServiceProcessor._events[index];

            while (true)
            {
                queueEvent.WaitOne();

                while (queue.TryDequeue(out Action action))
                {
                    try
                    {
                        action();
                    }
                    catch (Exception exception)
                    {
                        Logging.Error(typeof(ServiceProcessor), "An exception has been throwns while the execution of action. thread id: " + index + ", trace: " + exception);
                    }
                }
            }
        }

        /// <summary>
        ///     Enqueues the specified action.
        /// </summary>
        internal static void EnqueueReceiveAction(Action action)
        {
            ServiceProcessor._actions[0].Enqueue(action);
            ServiceProcessor._events[0].Set();
        }

        /// <summary>
        ///     Enqueues the specified action.
        /// </summary>
        internal static void EnqueueSendAction(Action action)
        {
            ServiceProcessor._actions[1].Enqueue(action);
            ServiceProcessor._events[1].Set();
        }
    }
}