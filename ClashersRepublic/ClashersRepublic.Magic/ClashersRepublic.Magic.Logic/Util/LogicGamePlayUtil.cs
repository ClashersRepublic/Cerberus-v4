﻿namespace ClashersRepublic.Magic.Logic.Util
{
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Logic.Level;
    using ClashersRepublic.Magic.Titan.Math;

    public static class LogicGamePlayUtil
    {
        /// <summary>
        ///     Converts the specified time to exp point.
        /// </summary>
        public static int TimeToExp(int time)
        {
            return LogicMath.Sqrt(time);
        }

        /// <summary>
        ///     Gets the cost of the speed up.
        /// </summary>
        public static int GetSpeedUpCost(int time, int speedUpType, int villageType)
        {
            int multiplier = 100;

            switch (speedUpType)
            {
                case 1:
                    multiplier = LogicDataTables.GetGlobals().UseNewTraining() ? LogicDataTables.GetGlobals().GetSpellTrainingCostMultiplier() : 
                                                                                 LogicDataTables.GetGlobals().GetSpellSpeedUpCostMultiplier();
                    break;
                case 2:
                    multiplier = LogicDataTables.GetGlobals().GetHeroHealthSpeedUpCostMultipler();
                    break;
                case 3:
                    multiplier = LogicDataTables.GetGlobals().GetTroopRequestSpeedUpCostMultiplier();
                    break;
                case 4:
                    multiplier = LogicDataTables.GetGlobals().GetTroopTrainingCostMultiplier();
                    break;
                case 5:
                    multiplier = LogicDataTables.GetGlobals().GetSpeedUpBoostCooldownCostMultiplier();
                    break;
                case 6:
                    multiplier = LogicDataTables.GetGlobals().GetClockTowerSpeedUpMultiplier();
                    break;
            }

            return LogicDataTables.GetGlobals().GetSpeedUpCost(time, multiplier, villageType);
        }
    }
}