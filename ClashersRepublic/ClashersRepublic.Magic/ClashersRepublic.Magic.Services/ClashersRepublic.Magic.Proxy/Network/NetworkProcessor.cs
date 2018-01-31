namespace ClashersRepublic.Magic.Proxy.Network
{
    using System;
    using System.Collections.Concurrent;
    using System.Threading;
    using ClashersRepublic.Magic.Proxy.Debug;

    internal static class NetworkProcessor
    {
        private static Thread[] _threads;
        private static ConcurrentQueue<Action>[] _actions;
        private static AutoResetEvent[] _events;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            NetworkProcessor._threads = new Thread[2];
            NetworkProcessor._actions = new ConcurrentQueue<Action>[2];
            NetworkProcessor._events = new AutoResetEvent[2];

            for (int i = 0; i < 2; i++)
            {
                int index = i;

                NetworkProcessor._events[index] = new AutoResetEvent(false);
                NetworkProcessor._actions[index] = new ConcurrentQueue<Action>();
                NetworkProcessor._threads[index] = new Thread(() => NetworkProcessor.Action(index));
                NetworkProcessor._threads[index].Start();
            }
        }

        /// <summary>
        ///     Tasks for thread.
        /// </summary>
        private static void Action(int index)
        {
            ConcurrentQueue<Action> queue = NetworkProcessor._actions[index];
            AutoResetEvent queueEvent = NetworkProcessor._events[index];

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
                        Logging.Error(typeof(NetworkProcessor), "An exception has been throwns while the execution of action. thread id: " + index + ", trace: " + exception);
                    }
                }
            }
        }

        /// <summary>
        ///     Enqueues the specified action.
        /// </summary>
        internal static void EnqueueReceiveAction(Action action)
        {
            NetworkProcessor._actions[0].Enqueue(action);
            NetworkProcessor._events[0].Set();
        }

        /// <summary>
        ///     Enqueues the specified action.
        /// </summary>
        internal static void EnqueueSendAction(Action action)
        {
            NetworkProcessor._actions[1].Enqueue(action);
            NetworkProcessor._events[1].Set();
        }
    }
}