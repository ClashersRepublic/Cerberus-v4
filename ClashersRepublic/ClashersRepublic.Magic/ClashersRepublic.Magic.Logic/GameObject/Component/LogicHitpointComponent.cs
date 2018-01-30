namespace ClashersRepublic.Magic.Logic.GameObject.Component
{
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.GameObject;
    using ClashersRepublic.Magic.Titan.Json;
    using ClashersRepublic.Magic.Titan.Math;

    public sealed class LogicHitpointComponent : LogicComponent
    {
        private bool _regenerationEnabled;

        private int _team;
        private int _hp;
        private int _maxHp;
        private int _maxRegenerationTime;
        private int _lastDamageTime;
        private int _regenTime;

        private LogicEffectData _dieEffect;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicHitpointComponent"/> class.
        /// </summary>
        public LogicHitpointComponent(LogicGameObject gameObject, int hp, int team) : base(gameObject)
        {
            this._team = team;

            this._hp = 100 * hp;
            this._maxHp = 100 * hp;
        }

        /// <summary>
        ///     Sets the max regeneration time.
        /// </summary>
        public void SetMaxRegenerationTime(int time)
        {
            this._maxRegenerationTime = time;
        }

        /// <summary>
        ///     Enables the regeneration.
        /// </summary>
        public void EnableRegeneration(bool state)
        {
            this._regenerationEnabled = state;
        }

        /// <summary>
        ///     Causes a damage to this gameobject.
        /// </summary>
        public void CauseDamage(int damage)
        {
            // TODO Implement LogicHitpointComponent::causeDamage.
        }

        /// <summary>
        ///     Updates the hero health to avatar.
        /// </summary>
        public void UpdateHeroHealthToAvatar(int hitpoint)
        {
            if (this._parent.GetGameObjectType() == 1)
            {
                // TODO Implement LogicHitpointComponent::updateHeroHealthToAvatar.
            }
        }

        /// <summary>
        ///     Called when a wall has been removed.
        /// </summary>
        public void WallRemoved()
        {
            // TODO Implement LogicHitpointComponent::wallRemoved.
        }

        /// <summary>
        ///     Sets the max hitpoints.
        /// </summary>
        public void SetMaxHitpoints(int maxHp)
        {
            this._maxHp = maxHp;
            this._hp = LogicMath.Clamp(this._hp, 0, 100 * maxHp);
        }

        /// <summary>
        ///     Sets hitpoints.
        /// </summary>
        public void SetHitpoints(int hp)
        {
            this._hp = LogicMath.Clamp(this._hp, 0, 100 * this._maxHp);
        }

        /// <summary>
        ///     Gets a value indicating whether the specified gameobject is a enemy for this gameobject.
        /// </summary>
        public bool IsEnemy(LogicGameObject gameObject)
        {
            // TODO Implement LogicHitpointComponent::isEnemy.
            return false;
        }

        /// <summary>
        ///     Gets a value indicating whether the specified gameobject is a enemy for specified team.
        /// </summary>
        public bool IsEnemyForTeam(int team)
        {
            return this._team != team && this._hp > 0;
        }

        /// <summary>
        ///     Gets the team.
        /// </summary>
        public int GetTeam()
        {
            return this._team;
        }

        /// <summary>
        ///     Sets the team.
        /// </summary>
        public void SetTeam(int team)
        {
            this._team = team;
        }

        /// <summary>
        ///     Gets a value indicating whether the gameobject has full hitpoints.
        /// </summary>
        public bool HasFullHitpoints()
        {
            return this._hp == this._maxHp;
        }

        /// <summary>
        ///     Sets the die effect.
        /// </summary>
        public void SetDieEffect(LogicEffectData data)
        {
            this._dieEffect = data;
        }

        /// <summary>
        ///     Gets a value indicating whether the gameobject has been damaged recently.
        /// </summary>
        public bool IsDamagedRecently()
        {
            return this._lastDamageTime < 30;
        }

        /// <summary>
        ///     Ticks for update this component. Called before Tick method.
        /// </summary>
        public override void SubTick()
        {
            base.SubTick();
        }

        /// <summary>
        ///     Ticks for update this component.
        /// </summary>
        public override void Tick()
        {
            if (this._regenerationEnabled)
            {
                if (this._hp < this._maxHp)
                {
                    if (this._maxRegenerationTime <= 0)
                    {
                        this._hp = this._maxHp;
                    }
                    else
                    {
                        this._regenTime += 64;
                        int tmp = LogicMath.Max(1000 * this._regenTime / (this._maxHp / 100), 1);
                        this._hp = this._hp + 100 * (this._regenTime / tmp);

                        if (this._hp > this._maxHp)
                        {
                            this._hp = this._maxHp;
                            this._regenTime = 0;
                        }
                        else
                        {
                            this._regenTime %= tmp;
                        }
                    }
                }
            }

            this._lastDamageTime += 1;
        }

        /// <summary>
        ///     Creates a fast forward of time.
        /// </summary>
        public override void FastForwardTime(int time)
        {
            if (this._regenerationEnabled)
            {
                this._regenTime += 64;

                if (this._maxRegenerationTime <= time)
                {
                    this._hp = this._maxHp;
                }
                else
                {
                    this._hp += this._maxHp * time / this._maxRegenerationTime;

                    if (this._hp > this._maxHp)
                    {
                        this._hp = this._maxHp;
                    }
                }
            }
        }

        /// <summary>
        ///     Gets the component type.
        /// </summary>
        public override int GetComponentType()
        {
            return 2;
        }

        /// <summary>
        ///     Loads this instance from json.
        /// </summary>
        public override void Load(LogicJSONObject jsonObject)
        {
            LogicJSONNumber hpNumber = jsonObject.GetJSONNumber("hp");
            LogicJSONBoolean regenBoolean = jsonObject.GetJSONBoolean("reg");

            if (hpNumber != null)
            {
                if (this._parent.GetLevel().GetState() != 2)
                {
                    this._hp = LogicMath.Clamp(hpNumber.GetIntValue(), 0, this._maxHp);
                }
                else
                {
                    this._hp = this._maxHp;
                }
            }
            else
            {
                this._hp = this._maxHp;
            }

            if (regenBoolean != null)
            {
                this._regenerationEnabled = regenBoolean.IsTrue();
            }
            else
            {
                this._regenerationEnabled = false;
            }
        }

        /// <summary>
        ///     Saves this instance to json.
        /// </summary>
        public override void Save(LogicJSONObject jsonObject)
        {
            if (this._hp < this._maxHp)
            {
                jsonObject.Put("hp", new LogicJSONNumber(this._hp));
                jsonObject.Put("reg", new LogicJSONBoolean(this._regenerationEnabled));
            }
        }

        /// <summary>
        ///     Gets the hitpoints.
        /// </summary>
        internal int InternalGetHp()
        {
            return this._hp;
        }

        /// <summary>
        ///     Gets the max hitpoints.
        /// </summary>
        internal int InternalGetMaxHp()
        {
            return this._maxHp;
        }
    }
}