namespace ClashersRepublic.Magic.Logic.Cooldown
{
    using ClashersRepublic.Magic.Titan.Json;
    using ClashersRepublic.Magic.Titan.Util;

    public class LogicCooldownManager
    {
        private LogicArrayList<LogicCooldown> _cooldowns;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicCooldownManager"/> class.
        /// </summary>
        public LogicCooldownManager()
        {
            this._cooldowns = new LogicArrayList<LogicCooldown>();
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public void Destruct()
        {
            this.DeleteCooldowns();
        }

        /// <summary>
        ///     Deletes all cooldowns.
        /// </summary>
        public void DeleteCooldowns()
        {
            if (this._cooldowns.Count != 0)
            {
                do
                {
                    this._cooldowns.Remove(0);
                } while (this._cooldowns.Count != 0);
            }
        }

        /// <summary>
        ///     Ticks this instance.
        /// </summary>
        public void Tick()
        {
            for (int i = 0; i < this._cooldowns.Count; i++)
            {
                this._cooldowns[i].Tick();

                if (this._cooldowns[i].GetCooldownSeconds() <= 0)
                {
                    this._cooldowns.Remove(i);
                }
            }
        }

        /// <summary>
        ///     Creates a fast forward of time.
        /// </summary>
        public void FastForwardTime(int secs)
        {
            for (int i = 0; i < this._cooldowns.Count; i++)
            {
                this._cooldowns[i].FastForwardTime(secs);
            }
        }

        /// <summary>
        ///     Loads this instance from json.
        /// </summary>
        public void Load(LogicJSONObject jsonObject)
        {
            LogicJSONArray cooldownArray = jsonObject.GetJSONArray("cooldowns");

            if (cooldownArray != null)
            {
                int size = cooldownArray.Size();

                for (int i = 0; i < size; i++)
                {
                    LogicCooldown cooldown = new LogicCooldown();
                    cooldown.Load(cooldownArray.GetJSONObject(size));
                    this._cooldowns.Add(cooldown);
                }
            }
        }

        /// <summary>
        ///     Saves this instance to json.
        /// </summary>
        public void Save(LogicJSONObject jsonObject)
        {
            LogicJSONArray cooldownArray = new LogicJSONArray();

            for (int i = 0; i < this._cooldowns.Count; i++)
            {
                LogicJSONObject cooldownObject = new LogicJSONObject();
                this._cooldowns[i].Save(cooldownObject);
                cooldownArray.Add(cooldownObject);
            }

            jsonObject.Put("cooldowns", cooldownArray);
        }

        /// <summary>
        ///     Adds the cooldown.
        /// </summary>
        public void AddCooldown(int targetGlobalId, int cooldownSecs)
        {
            this._cooldowns.Add(new LogicCooldown(targetGlobalId, cooldownSecs));
        }

        /// <summary>
        ///     Gets the remaining cooldown seconds for specified target.
        /// </summary>
        public int GetCooldownSeconds(int targetGlobalId)
        {
            for (int i = 0; i < this._cooldowns.Count; i++)
            {
                if (this._cooldowns[i].GetTargetGlobalId() == targetGlobalId)
                {
                    return this._cooldowns[i].GetCooldownSeconds();
                }
            }

            return 0;
        }
    }
}