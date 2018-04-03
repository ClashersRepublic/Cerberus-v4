namespace RivieraStudio.Magic.Titan.Math
{
    using RivieraStudio.Magic.Titan.DataStream;

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
        ///     Returns a random int between 0 and Max.
        /// </summary>
        public int Rand(int max)
        {
            if (max > 0)
            {
                int seed = this._seed;

                if (seed == 0)
                {
                    seed = -1;
                }

                int tmp = seed ^ (seed << 13) ^ ((seed ^ (seed << 13)) >> 17);
                int tmp2 = tmp ^ 32 * tmp;
                this._seed = tmp2;

                if (tmp2 < 0)
                {
                    tmp2 = -tmp2;
                }

                return tmp2 % max;
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