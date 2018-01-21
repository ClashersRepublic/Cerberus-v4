namespace ClashersRepublic.Magic.Proxy.Service
{
    using System;

    using ClashersRepublic.Magic.Logic;
    using ClashersRepublic.Magic.Logic.Message;
    using ClashersRepublic.Magic.Logic.Message.Factory;
    using ClashersRepublic.Magic.Services.Logic;
    using ClashersRepublic.Magic.Services.Logic.Message.Factory;

    using RabbitMQ.Client;
    using RabbitMQ.Client.Events;

    internal class ServiceConnection
    {
        private static bool _initialized;
        private static string _queueName;
        private static string _exchangeName;
        private static IModel _channel;

        private static LogicMessageFactory _messageFactory;

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

            ServiceConnection._messageFactory = MagicServiceMessageFactory.Instance;
            ServiceConnection._channel = new ConnectionFactory
            {
                HostName = "127.0.0.1"
            }.CreateConnection().CreateModel();

            ServiceConnection._exchangeName = ServiceExchangeName.PROXY_EXCHANGE_NAME;
            ServiceConnection._queueName = ServiceConnection._exchangeName + '.' + Config.ServerId;

            ServiceConnection._channel.ExchangeDeclare(ServiceConnection._exchangeName, ExchangeType.Topic);
            ServiceConnection._channel.QueueDeclare(ServiceConnection._queueName);
            ServiceConnection._channel.QueueBind(ServiceConnection._queueName, ServiceConnection._exchangeName, Config.ServerId.ToString());

            EventingBasicConsumer consumer = new EventingBasicConsumer(ServiceConnection._channel);
            consumer.Received += ServiceConnection.ConsumerOnReceived;
            ServiceConnection._channel.BasicConsume(ServiceConnection._queueName, true, consumer);
        }

        /// <summary>
        ///     Called when a message has been received.
        /// </summary>
        private static void ConsumerOnReceived(object sender, BasicDeliverEventArgs args)
        {
            try
            {
                byte[] packet = args.Body;

                if (packet.Length < 4)
                {
                    Logging.Error(typeof(ServiceConnection), "Invalid message body.");
                    return;
                }

                int messageType = packet[1] | (packet[0] << 8);
                int messageVersion = packet[3] | (packet[2] << 8);
                byte[] messageBytes = new byte[packet.Length - 4];

                Array.Copy(packet, 4, messageBytes, 0, messageBytes.Length);
                PiranhaMessage message = ServiceConnection._messageFactory.CreateMessageByType(messageType);

                if (message != null)
                {
                    message.SetMessageVersion((short) messageVersion);
                    message.GetByteStream().SetByteArray(messageBytes, messageBytes.Length);

                    ServiceConnectionProcessor.EnqueueReceivedMessage(message, args.RoutingKey);
                }
                else
                {
                    Logging.Error(typeof(ServiceConnection), "A unknown message type has been received. type: " + messageType);
                }
            }
            catch (Exception exception)
            {
                Logging.Error(typeof(ServiceConnection), "An exception has been thorwed when the handle of service message. trace: " + exception);
            }
        }

        /// <summary>
        ///     Sends the content to specified node.
        /// </summary>
        internal static void SendToService(string exchange, string routingKey, byte[] content)
        {
            IBasicProperties props = ServiceConnection._channel.CreateBasicProperties();
            props.ReplyTo = ServiceConnection._queueName;
            ServiceConnection._channel.BasicPublish(exchange, routingKey, props, content);
        }
    }
}