namespace ClashersRepublic.Magic.Titan.Math
{
    using ClashersRepublic.Magic.Titan.DataStream;

    public class LogicRandom
    {
        private int _seed;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicRandom" /> class.
        /// </summary>
        public LogicRandom()
        {
            // LogicRandom.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicRandom" /> class.
        /// </summary>
        /// <param name="seed">The seed.</param>
        public LogicRandom(int seed)
        {
            this._seed = seed;
        }

        /// <summary>
        ///     Returns a random int between 0 and Max.
        /// </summary>
        public int Rand(int max)
        {
            if (max > 0)
            {
                if (this._seed == 0)
                {
                    this._seed = -1;
                }

                int tmp = this._seed ^ (this._seed << 13) ^ ((this._seed ^ (this._seed << 13)) >> 17);
                this._seed = tmp ^ (32 * tmp);

                if (this._seed < 0)
                {
                    return -this._seed % max;
                }

                return this._seed % max;
            }

            return 0;
        }

        /// <summary>
        ///     Decodes this instance.
        /// </summary>
        public void Decode(ByteStream stream)
        {
            this._seed = stream.ReadInt();
        }

        /// <summary>
        ///     Encodes this instance.
        /// </summary>
        public void Encode(ChecksumEncoder stream)
        {
            stream.WriteInt(this._seed);
        }
    }
}