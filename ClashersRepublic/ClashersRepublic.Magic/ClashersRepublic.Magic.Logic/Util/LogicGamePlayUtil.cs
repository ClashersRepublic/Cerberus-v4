namespace ClashersRepublic.Magic.Logic.Util
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
        public static int GetSpeedUpCost(int time, int speedUpType, bool isVillage2)
        {
            int multiplier = 100;

            switch (speedUpType)
            {
                case 1:
                    break;
            }

            return LogicDataTables.GetGlobals().GetSpeedUpCost(time, multiplier, isVillage2);
        }
    }
}