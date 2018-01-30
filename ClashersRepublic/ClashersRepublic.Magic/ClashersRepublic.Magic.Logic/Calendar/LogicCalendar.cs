namespace ClashersRepublic.Magic.Logic.Calendar
{
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Titan.Util;

    public class LogicCalendar
    {
        private int _activeTimestamp;
        private LogicArrayList<LogicCalendarEvent> _activeCalendarEvents;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicCalendar"/> class.
        /// </summary>
        public LogicCalendar()
        {
            this._activeCalendarEvents = new LogicArrayList<LogicCalendarEvent>();
        }

        /// <summary>
        ///     Gets the checksum of this instance.
        /// </summary>
        public void GetChecksum(ChecksumHelper checksum)
        {
            checksum.StartObject("LogicCalendar");
            checksum.StartArray("m_pActiveCalendarEvents");

            for (int i = 0; i < this._activeCalendarEvents.Count; i++)
            {
                checksum.StartObject("LogicCalendarEvent");
                checksum.EndObject();
            }

            checksum.EndArray();
            checksum.WriteValue("m_activeTimestamp", this._activeTimestamp);
            checksum.EndObject();
        }
    }
}