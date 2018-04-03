namespace LineageSoft.Magic.Logic.Calendar
{
    using LineageSoft.Magic.Logic.Avatar;
    using LineageSoft.Magic.Logic.Helper;
    using LineageSoft.Magic.Logic.Level;
    using LineageSoft.Magic.Titan.Debug;
    using LineageSoft.Magic.Titan.Json;
    using LineageSoft.Magic.Titan.Util;

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

                    if (eventArray != null)
                    {
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
                }
                else
                {
                    Debugger.Error("Events json malformed!");
                }
            }
        }
        
        /// <summary>
        ///     Sets the calendar data.
        /// </summary>
        public void SetCalendarData(int activeTimestamp, LogicAvatar homeOwnerAvatar, LogicLevel level)
        {
            Debugger.DoAssert(activeTimestamp != -1, "You must set a valid time for calendar.");

            if (this._activeTimestamp != activeTimestamp)
            {
                this._activeTimestamp = activeTimestamp;

                /*
                if ( sub_294F78(a1, *(_DWORD **)(a1 + 4), a2, a2) )
                {
                   v4 = sub_2950F4(a1, a2, a2);
                   v5 = *(void ***)(a1 + 4);
                   if ( v5 )
                   {
                       v7 = v4;
                       if ( *v5 )
                       operator delete[](*v5);
                       operator delete(v5);
                       v4 = v7;
                   }
                   *(_DWORD *)(a1 + 4) = v4;
                   if ( a3 && a4 )
                        sub_295186(a1, a3, a4);
                   v6 = *(_DWORD *)(a1 + 8);
                   if ( v6 )
                        (*(void (__cdecl **)(_DWORD))(*(_DWORD *)v6 + 12))(*(_DWORD *)(a1 + 8));
                }
                */
            }
        }
    }
}
 