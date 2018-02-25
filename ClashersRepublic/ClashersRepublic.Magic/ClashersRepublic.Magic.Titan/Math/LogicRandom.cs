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
        ///     Gets the iterated random seed.
        /// </summary>
        public int GetIteratedRandomSeed()
        {
            return this._seed;
        }

        /// <summary>
        ///     Sets the iterated random seed.
        /// </summary>
        public void SetIteratedRandomSeed(int value)
        {
            this._seed = value;
        }

        /// <summary>
        ///     Iterates the specified random seed.
        /// </summary>
        public int IterateRandomSeed(int seed)
        {
            if (seed == 0)
            {
                seed = -1;
            }

            int tmp = seed ^ (seed << 13);
            int tmp2 = tmp ^ (tmp >> 17);

            return tmp2 ^ 32 * tmp2;
        }

        /// <summary>
        ///     Returns a random int between 0 and Max.
        /// </summary>
        public int Rand(int max)
        {
            if (max > 0)
            {
                this._seed = this.IterateRandomSeed(this._seed);

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