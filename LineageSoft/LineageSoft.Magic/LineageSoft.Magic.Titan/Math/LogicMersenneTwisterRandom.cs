namespace LineageSoft.Magic.Titan.Math
{
    public class LogicMersenneTwisterRandom
    {
        private readonly int[] _seeds = new int[624];
        private int _ix;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicMersenneTwisterRandom" /> class.
        /// </summary>
        public LogicMersenneTwisterRandom() : this(324876476)
        {
            // LogicMersenneTwisterRandom.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicMersenneTwisterRandom" /> class.
        /// </summary>
        public LogicMersenneTwisterRandom(int seed)
        {
            this._seeds[0] = seed;

            for (int i = 1; i < this._seeds.Length; i++)
            {
                seed = 1812433253 * (seed ^ (seed >> 30)) + 1812433253;
                this._seeds[i] = seed;
            }
        }

        /// <summary>
        ///     Generates the seeds array.
        /// </summary>
        public void Generate()
        {
            for (int i = 1, j = 0; i <= this._seeds.Length; i++, j++)
            {
                int v4 = (this._seeds[i % this._seeds.Length] & 0x7fffffff) + (this._seeds[j] & -0x80000000);
                int v6 = (v4 >> 1) ^ this._seeds[(i + 396) % this._seeds.Length];

                if ((v4 & 1) == 1)
                {
                    v6 ^= -0x66F74F21;
                }

                this._seeds[j] = v6;
            }
        }

        /// <summary>
        ///     Gets the next random int value.
        /// </summary>
        public int NextInt()
        {
            if (this._ix == 0)
            {
                this.Generate();
            }

            int seed = this._seeds[this._ix];
            this._ix = (this._ix + 1) % 624;

            seed ^= seed >> 11;
            seed = seed ^ ((seed << 7) & -1658038656) ^ (((seed ^ ((seed << 7) & -1658038656)) << 15) & -0x103A0000) ^ ((seed ^ ((seed << 7) & -1658038656) ^ (((seed ^ ((seed << 7) & -1658038656)) << 15) & -0x103A0000)) >> 18);

            return seed;
        }

        /// <summary>
        ///     Gets a random integer value between 0 and max.
        /// </summary>
        public int Rand(int max)
        {
            int rnd = this.NextInt();

            if (rnd < 0)
            {
                rnd = -rnd;
            }

            return rnd % max;
        }
    }
}