namespace ClashersRepublic.Magic.Services.Logic.Message
{
    using ClashersRepublic.Magic.Titan.Message;

    public class ServiceMessage : PiranhaMessage
    {
        protected string ExchangeKey;
        protected string RoutingKey;
        protected string ProxySessionId;

        /// <summary>
        ///     Initializes a new instance of the <see cref="ServiceMessage" /> class.
        /// </summary>
        public ServiceMessage(short messageVersion) : base(messageVersion)
        {
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public override void Decode()
        {
            base.Decode();

            if (this.Stream.ReadBoolean())
            {
                this.ExchangeKey = this.Stream.ReadStringReference(900000);
                this.RoutingKey = this.Stream.ReadStringReference(900000);
            }
            
            if (this.Stream.ReadBoolean())
            {
                this.ProxySessionId = this.Stream.ReadStringReference(900000);
            }
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public override void Encode()
        {
            base.Encode();

            if (this.RoutingKey == null && this.ExchangeKey == null)
            {
                this.Stream.WriteBoolean(false);
            }
            else
            {
                this.Stream.WriteBoolean(true);
                this.Stream.WriteStringReference(this.ExchangeKey);
                this.Stream.WriteStringReference(this.RoutingKey);
            }

            if (this.ProxySessionId == null)
            {
                this.Stream.WriteBoolean(false);
            }
            else
            {
                this.Stream.WriteBoolean(true);
                this.Stream.WriteStringReference(this.ProxySessionId);
            }
        }

        /// <summary>
        ///     Gets the message type of this instance.
        /// </summary>
        public override short GetMessageType()
        {
            return base.GetMessageType();
        }

        /// <summary>
        ///     Gets the service node type of this instance.
        /// </summary>
        public override int GetServiceNodeType()
        {
            return base.GetServiceNodeType();
        }

        /// <summary>
        ///     Destructs this message.
        /// </summary>
        public override void Destruct()
        {
            base.Destruct();

            this.RoutingKey = null;
            this.ProxySessionId = null;
        }

        /// <summary>
        ///     Gets the exchange name.
        /// </summary>
        public string GetExchangeName()
        {
            return this.ExchangeKey;
        }

        /// <summary>
        ///     Sets the exchange name.
        /// </summary>
        public void SetExchangeName(string value)
        {
            this.ExchangeKey = value;
        }

        /// <summary>
        ///     Gets the routing key.
        /// </summary>
        public string GetRoutingKey()
        {
            return this.RoutingKey;
        }

        /// <summary>
        ///     Sets the routing key.
        /// </summary>
        public void SetRoutingKey(string value)
        {
            this.RoutingKey = value;
        }

        /// <summary>
        ///     Gets the proxy session id.
        /// </summary>
        public string GetProxySessionId()
        {
            return this.ProxySessionId;
        }

        /// <summary>
        ///     Sets the proxy session id.
        /// </summary>
        public void SetProxySessionId(string value)
        {
            this.ProxySessionId = value;
        }
    }
}