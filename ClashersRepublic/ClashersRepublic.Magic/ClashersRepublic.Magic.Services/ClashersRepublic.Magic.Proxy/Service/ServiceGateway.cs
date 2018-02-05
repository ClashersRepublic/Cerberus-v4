namespace ClashersRepublic.Magic.Proxy.Service
{
    using ClashersRepublic.Magic.Proxy.Debug;
    using ClashersRepublic.Magic.Services.Logic;
    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    internal static class ServiceGateway
    {
        internal static IModel _channel;

        internal static string ExchangeName;
        internal static string QueueName;
        internal static string ServiceType;

        /// <summary>
        ///     Initializes this instance.
        /// </summary>
        internal static void Initialize()
        {
            ServiceGateway.ServiceType = ServiceExchangeName.SERVICE_PROXY_NAME;
            ServiceGateway.ExchangeName = ServiceExchangeName.BuildExchangeName(ServiceGateway.ServiceType);
            ServiceGateway.QueueName = ServiceExchangeName.BuildQueueName(ServiceGateway.ServiceType, Config.ServerId);

            IConnection connection = new ConnectionFactory { HostName = Config.RabbitServer, UserName = Config.RabbitUser, Password = Config.RabbitPassword }.CreateConnection();

            ServiceGateway._channel = connection.CreateModel();

            ServiceGateway.DeclareExchange(ServiceGateway.ExchangeName);
            ServiceGateway.DeclareQueue(ServiceGateway.QueueName);
            ServiceGateway.BindQueue(ServiceGateway.ExchangeName,
                ServiceGateway.QueueName,
                ServiceGateway.QueueName);
            ServiceGateway.ListenQueue(ServiceGateway.QueueName);
        }

        /// <summary>
        ///     Declares the exchange.
        /// </summary>
        internal static void DeclareExchange(string exchange)
        {
            ServiceGateway._channel.ExchangeDeclare(exchange, ExchangeType.Direct, true);
        }

        /// <summary>
        ///     Declares the queue.
        /// </summary>
        internal static void DeclareQueue(string queueName, bool isEsclusive = true)
        {
            ServiceGateway._channel.QueueDeclare(queueName, true, isEsclusive);
        }

        /// <summary>
        ///     Binds the queue to specified exchange.
        /// </summary>
        internal static void BindQueue(string exchange, string queue, params string[] routingKeys)
        {
            for (int i = 0; i < routingKeys.Length; i++)
            {
                ServiceGateway._channel.QueueBind(queue, exchange, routingKeys[i]);
            }
        }

        /// <summary>
        ///     Listens the specified queue.
        /// </summary>
        internal static void ListenQueue(string queue)
        {
            EventingBasicConsumer consumer = new EventingBasicConsumer(ServiceGateway._channel);
            consumer.Received += ServiceGateway.ReceiveCallback;
            ServiceGateway._channel.BasicConsume(queue, true, consumer);
        }

        /// <summary>
        ///     Sends the message to specified client.
        /// </summary>
        internal static void Send(byte[] content, string exchange, string routingKey)
        {
            ServiceGateway._channel.BasicPublish(exchange, routingKey, ServiceGateway._channel.CreateBasicProperties(), content);
        }

        /// <summary>
        ///     Called when a content has been received.
        /// </summary>
        private static void ReceiveCallback(object sender, BasicDeliverEventArgs args)
        {
            if (args.Body != null)
            {
                ServiceMessaging.OnReceive(args);
            }
            else
            {
                Logging.Error(typeof(ServiceGateway), "ServiceGateway::receiveCallback body is NULL");
            }
        }
    }
}