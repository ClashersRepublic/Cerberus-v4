namespace RivieraStudio.Magic.Logic.Calendar
{
    using RivieraStudio.Magic.Titan.Json;

    public static class LogicCalendarEventFactory
    {
        /// <summary>
        ///     Creates a event with type.
        /// </summary>
        public static LogicCalendarEvent CreateByType(int type)
        {
            switch (type)
            {
                case 0:
                    break;
            }

            return null;
        }

        /// <summary>
        ///     Loads the calendar event from json.
        /// </summary>
        public static LogicCalendarEvent LoadFromJSON(LogicJSONObject jsonObject)
        {
            LogicJSONNumber typeObject = jsonObject.GetJSONNumber("type");
            LogicCalendarEvent calendarEvent = LogicCalendarEventFactory.CreateByType(typeObject.GetIntValue());
            calendarEvent.Load(jsonObject);
            return calendarEvent;
        }
    }
}