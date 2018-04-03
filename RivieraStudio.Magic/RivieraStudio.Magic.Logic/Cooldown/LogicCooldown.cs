namespace RivieraStudio.Magic.Logic.Cooldown
{
    using RivieraStudio.Magic.Logic.Time;
    using RivieraStudio.Magic.Titan.Debug;
    using RivieraStudio.Magic.Titan.Json;
    using RivieraStudio.Magic.Titan.Math;

    public class LogicCooldown
    {
        private int _targetGlobalId;
        private int _cooldownTime;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicCooldown" /> class.
        /// </summary>
        public LogicCooldown()
        {
            // LogicCooldown.
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicCooldown" /> class.
        /// </summary>
        public LogicCooldown(int targetGlobalId, int cooldownSecs)
        {
            this._targetGlobalId = targetGlobalId;
            this._cooldownTime = LogicTime.GetCooldownSecondsInTicks(cooldownSecs);
        }

        /// <summary>
        ///     Ticks this instance.
        /// </summary>
        public void Tick()
        {
            if (this._cooldownTime > 0)
            {
                --this._cooldownTime;
            }
        }

        /// <summary>
        ///     Creates a fast forward of time.
        /// </summary>
        public void FastForwardTime(int secs)
        {
            this._cooldownTime = LogicMath.Max(this._cooldownTime - LogicTime.GetCooldownSecondsInTicks(secs), 0);
        }

        /// <summary>
        ///     Loads this instance from json.
        /// </summary>
        public void Load(LogicJSONObject jsonObject)
        {
            LogicJSONNumber cooldownNumber = jsonObject.GetJSONNumber("cooldown");
            LogicJSONNumber targetNumber = jsonObject.GetJSONNumber("target");

            if (cooldownNumber != null)
            {
                this._cooldownTime = cooldownNumber.GetIntValue();
            }
            else
            {
                Debugger.Error("LogicCooldown::load - Cooldown was not found!");
            }

            if (targetNumber != null)
            {
                this._targetGlobalId = targetNumber.GetIntValue();
            }
            else
            {
                Debugger.Error("LogicCooldown::load - Target was not found!");
            }
        }

        /// <summary>
        ///     Saves this instance to json.
        /// </summary>
        public void Save(LogicJSONObject jsonObject)
        {
            jsonObject.Put("cooldown", new LogicJSONNumber(this._cooldownTime));
            jsonObject.Put("target", new LogicJSONNumber(this._targetGlobalId));
        }

        /// <summary>
        ///     Gets the remaining cooldown seconds.
        /// </summary>
        public int GetCooldownSeconds()
        {
            return LogicTime.GetCooldownTicksInSeconds(this._cooldownTime);
        }

        /// <summary>
        ///     Gets the target global id.
        /// </summary>
        public int GetTargetGlobalId()
        {
            return this._targetGlobalId;
        }
    }
}