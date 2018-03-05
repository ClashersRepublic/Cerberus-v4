namespace ClashersRepublic.Magic.Logic.GameObject
{
    using ClashersRepublic.Magic.Logic.Avatar;
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.Level;
    using ClashersRepublic.Magic.Logic.Time;
    using ClashersRepublic.Magic.Logic.Util;
    using ClashersRepublic.Magic.Titan.Math;

    public sealed class LogicObstacle : LogicGameObject
    {
        private LogicTimer _clearTimer;
        private int _fadeTime;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicObstacle" /> class.
        /// </summary>
        public LogicObstacle(LogicData data, LogicLevel level, int villageType) : base(data, level, villageType)
        {
            // LogicObstacle.
        }

        /// <summary>
        ///     Gets the <see cref="LogicObstacleData"/> instance.
        /// </summary>
        public LogicObstacleData GetObstacleData()
        {
            return (LogicObstacleData) this._data;
        }

        /// <summary>
        ///     Gets if this <see cref="LogicObstacleData"/> is passable.
        /// </summary>
        public override bool IsPassable()
        {
            return this.GetObstacleData().Passable;
        }

        /// <summary>
        ///     Creates a fast forward of time.
        /// </summary>
        public override void FastForwardTime(int time)
        {
            base.FastForwardTime(time);

            if (this._clearTimer != null)
            {
                int remainingSeconds = this._clearTimer.GetRemainingSeconds(this._level.GetLogicTime());

                if (remainingSeconds <= time)
                {
                    if (LogicDataTables.GetGlobals().CompleteConstructionOnlyHome())
                    {
                        this._clearTimer.StartTimer(0, this._level.GetLogicTime(), false, -1);
                    }
                    else
                    {
                        this.ClearingFinished(true);
                    }
                }
                else
                {
                    this._clearTimer.StartTimer(remainingSeconds - time, this._level.GetLogicTime(), false, -1);
                }
            }
        }

        /// <summary>
        ///     Ticks this instance.
        /// </summary>
        public override void Tick()
        {
            base.Tick();

            if (this._clearTimer != null)
            {
                if (this._clearTimer.GetRemainingSeconds(this._level.GetLogicTime()) > 0 && this._level.GetRemainingClockTowerBoostTime() > 0 && this.GetObstacleData().VillageType == 1)
                {
                    this._clearTimer.SetBoostedTime(this._clearTimer.GetBoostedTime() + 4 * LogicDataTables.GetGlobals().GetClockTowerBoostMultiplier() - 4);
                }
            }

            if (this._fadeTime < 1)
            {
                if (this._clearTimer != null)
                {
                    if (this._clearTimer.GetRemainingSeconds(this._level.GetLogicTime()) <= 0)
                    {
                        this.ClearingFinished(false);
                    }
                }
            }
            else
            {
                int tmp = 2000;

                if (this.GetObstacleData().LootDefensePercentage > 1)
                {
                    tmp = 4000;
                }

                if (this.GetObstacleData().TallGrass)
                {
                    tmp = 1000;
                }

                this._fadeTime = LogicMath.Min(this._fadeTime + 64, tmp);
            }
        }

        /// <summary>
        ///     Gets if this <see cref="LogicObstacle"/> instance should be destroy.
        /// </summary>
        public override bool ShouldDestruct()
        {
            int tmp = 2000;

            if (this.GetObstacleData().LootDefensePercentage > 1)
            {
                tmp = 4000;
            }

            if (this.GetObstacleData().TallGrass)
            {
                tmp = 1000;
            }

            return this._fadeTime >= tmp;
        }

        /// <summary>
        ///     Gets if this <see cref="LogicObstacle"/> is unbuildable.
        /// </summary>
        public override bool IsUnbuildable()
        {
            return this.GetObstacleData().IsTombstone;
        }

        /// <summary>
        ///     Gets the fade time.
        /// </summary>
        public int GetFadeTime()
        {
            return this._fadeTime;
        }

        /// <summary>
        ///     Gets the remaining clearing time.
        /// </summary>
        public int GetRemainingClearingTime()
        {
            if (this._clearTimer != null)
            {
                return this._clearTimer.GetRemainingSeconds(this._level.GetLogicTime());
            }

            return 0;
        }

        /// <summary>
        ///     Gets the remaining clearing time in ms.
        /// </summary>
        public int GetRemainingClearingTimeMS()
        {
            if (this._clearTimer != null)
            {
                return this._clearTimer.GetRemainingMS(this._level.GetLogicTime());
            }

            return 0;
        }

        /// <summary>
        ///     Called when the clearing of this <see cref="LogicObstacle"/> instance is finished.
        /// </summary>
        public void ClearingFinished(bool clearedWithFastForward)
        {
            // TODO: Implement LogicObstacle::clearingFinished(bool).
        }

        /// <summary>
        ///     Speeds up the clearing of the obstacle.
        /// </summary>
        public bool SpeedUpClearing()
        {
            if (this._clearTimer != null)
            {
                LogicClientAvatar playerAvatar = (LogicClientAvatar) this._level.GetPlayerAvatar();
                int speedUpCost = LogicGamePlayUtil.GetSpeedUpCost(this._clearTimer.GetRemainingSeconds(this._level.GetLogicTime()), 0, this._villageType > 0);

                if (playerAvatar.HashEnoughDiamonds(speedUpCost, true, this._level))
                {
                    playerAvatar.UseDiamonds(speedUpCost);
                    this.ClearingFinished(false);
                    return true;
                }
            }

            return false;
        }
    }
}