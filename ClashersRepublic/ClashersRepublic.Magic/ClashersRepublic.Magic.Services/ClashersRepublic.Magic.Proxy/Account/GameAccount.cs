namespace ClashersRepublic.Magic.Proxy.Account
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization.Attributes;

    internal class GameAccount
    {
        [BsonId]
        public ObjectId _id;

        public int HighId;
        public int LowId;

        public string PassToken;
        public string AccountCreationDate;

        /// <summary>
        ///     Initializes a new instance of the <see cref="GameAccount"/> class.
        /// </summary>
        internal GameAccount()
        {
            // GameAccount.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="GameAccount"/> class.
        /// </summary>
        internal GameAccount(int highId, int lowId, string passToken)
        {
            this.HighId = highId;
            this.LowId = lowId;
            this.PassToken = passToken;
        }
    }
}