namespace ClashersRepublic.Magic.Proxy.Service
{
    using System;
    using ClashersRepublic.Magic.Services.Logic;

    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    internal static class ServiceConnection
    {
        private static bool _initialized;
        private static IModel _model;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            if (ServiceConnection._initialized)
            {
                return;
            }

            ServiceConnection._initialized = true;

            IConnection connection = new ConnectionFactory {HostName = "127.0.0.1"}.CreateConnection();

            ServiceConnection._model = connection.CreateModel();

            ServiceConnection.DeclareExchange(ServiceExchangeName.PROXY_EXCHANGE_NAME);
            ServiceConnection.DeclareQueue(ServiceExchangeName.PROXY_QUEUE_PREFIX + Config.ServerId);
            ServiceConnection.BindQueue(ServiceExchangeName.PROXY_EXCHANGE_NAME,
                ServiceExchangeName.PROXY_QUEUE_PREFIX + Config.ServerId,
                ServiceExchangeName.PROXY_ROUTING_KEY_PREFIX + Config.ServerId);
            ServiceConnection.ListenQueue(ServiceExchangeName.PROXY_QUEUE_PREFIX + Config.ServerId);
        }

        /// <summary>
        ///     Declares the exchange.
        /// </summary>
        internal static void DeclareExchange(string exchange)
        {
            ServiceConnection._model.ExchangeDeclare(exchange, ExchangeType.Direct, true);
        }

        /// <summary>
        ///     Declares the queue.
        /// </summary>
        internal static void DeclareQueue(string queueName, bool isExclusive = true)
        {
            ServiceConnection._model.QueueDeclare(queueName, true, isExclusive);
        }

        /// <summary>
        ///     Binds the queue to specified exchange.
        /// </summary>
        internal static void BindQueue(string exchange, string queue, params string[] routingKeys)
        {
            for (int i = 0; i < routingKeys.Length; i++)
            {
                ServiceConnection._model.QueueBind(queue, exchange, routingKeys[i]);
            }
        }
        
        /// <summary>
        ///     Listens the specified queue.
        /// </summary>
        internal static void ListenQueue(string queue)
        {
            EventingBasicConsumer consumer = new EventingBasicConsumer(ServiceConnection._model);
            consumer.Received += ServiceConnection.ReceiveCallback;
            ServiceConnection._model.BasicConsume(queue, true, consumer);
        }

        /// <summary>
        ///     Sends the message to specified client.
        /// </summary>
        internal static void Send(byte[] content, string exchange, string routingKey)
        {
            ServiceConnection._model.BasicPublish(exchange, routingKey, ServiceConnection._model.CreateBasicProperties(), content);
        }

        /// <summary>
        ///     Called when a content has been received.
        /// </summary>
        private static void ReceiveCallback(object sender, BasicDeliverEventArgs args)
        {
            try
            {
                ServiceMessaging.ReceiveMessage(args.Body, args);
            }
            catch(Exception exception)
            {
                Logging.Error(typeof(ServiceConnection), "An exception has been thrown when the handle of service message. trace: " + exception);
            }
        }
    }
}