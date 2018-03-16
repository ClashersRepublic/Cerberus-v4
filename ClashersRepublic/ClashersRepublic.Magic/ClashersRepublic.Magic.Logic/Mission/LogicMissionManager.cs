namespace ClashersRepublic.Magic.Logic.Mission
{
    using ClashersRepublic.Magic.Logic.Data;
    using ClashersRepublic.Magic.Titan.Debug;

    public class LogicMissionManager
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicMissionManager"/> class.
        /// </summary>
        public LogicMissionManager(LogicMissionData data)
        {
            if (data == null)
            {
                Debugger.Error("LogicMission::constructor - pData is NULL!");
            }
        }
    }
}