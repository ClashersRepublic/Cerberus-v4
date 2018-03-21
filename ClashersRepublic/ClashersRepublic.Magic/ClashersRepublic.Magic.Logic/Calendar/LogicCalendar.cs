namespace ClashersRepublic.Magic.Logic.Calendar
{
    using ClashersRepublic.Magic.Logic.Helper;
    using ClashersRepublic.Magic.Titan.Debug;
    using ClashersRepublic.Magic.Titan.Json;
    using ClashersRepublic.Magic.Titan.Util;

    public class LogicCalendar
    {
        private int _activeTimestamp;
        private LogicArrayList<LogicCalendarEvent> _activeCalendarEvents;

        /// <summary>
        ///     Initializes a new instance of the <see cref="LogicCalendar" /> class.
        /// </summary>
        public LogicCalendar()
        {
            this._activeCalendarEvents = new LogicArrayList<LogicCalendarEvent>();
        }

        /// <summary>
        ///     Destructs this instance.
        /// </summary>
        public void Destruct()
        {
            if (this._activeCalendarEvents != null)
            {
                if (this._activeCalendarEvents.Count != 0)
                {
                    do
                    {
                        this._activeCalendarEvents.Remove(0);
                    } while (this._activeCalendarEvents.Count != 0);
                }

                this._activeCalendarEvents = null;
            }
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

        /// <summary>
        ///     Laods the calendar.
        /// </summary>
        public void Load(string json, int activeTimestamp)
        {
            Debugger.DoAssert(json != null, "Event json NULL");

            if (json.Length > 0)
            {
                LogicJSONObject jsonObject = (LogicJSONObject) LogicJSONParser.Parse(json);

                if (jsonObject != null)
                {
                    LogicJSONArray eventArray = jsonObject.GetJSONArray("events");

                    for (int i = 0; i < eventArray.Size(); i++)
                    {
                        LogicJSONObject calendarObject = eventArray.GetJSONObject(i);

                        if (calendarObject != null)
                        {

                        }
                        else
                        {
                            Debugger.Error("Events json malformed!");
                        }
                    }
                }
                else
                {
                    Debugger.Error("Events json malformed!");
                }
            }
        }
    }
}