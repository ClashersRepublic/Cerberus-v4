namespace LineageSoft.Magic.Logic.Helper
{
    using LineageSoft.Magic.Titan.Json;

    public static class LogicJSONHelper
    {
        /// <summary>
        ///     Gets the specified json boolean.
        /// </summary>
        public static bool GetJSONBoolean(LogicJSONObject jsonObject, string key)
        {
            LogicJSONBoolean jsonBoolean = jsonObject.GetJSONBoolean(key);

            if (jsonBoolean != null)
            {
                return jsonBoolean.IsTrue();
            }

            return false;
        }

        /// <summary>
        ///     Gets the specified json number.
        /// </summary>
        public static int GetJSONNumber(LogicJSONObject jsonObject, string key)
        {
            LogicJSONNumber jsonNumber = jsonObject.GetJSONNumber(key);

            if (jsonNumber != null)
            {
                return jsonNumber.GetIntValue();
            }

            return 0;
        }

        /// <summary>
        ///     Gets the specified json string.
        /// </summary>
        public static string GetJSONString(LogicJSONObject jsonObject, string key)
        {
            LogicJSONString jsonString = jsonObject.GetJSONString(key);

            if (jsonString != null)
            {
                return jsonString.GetStringValue();
            }

            return null;
        }

        /// <summary>
        ///     Sets the json number value.
        /// </summary>
        public static void SetJSONNumber(LogicJSONObject jsonObject, string key, int value)
        {
            if (value != 0)
            {
                jsonObject.Put(key, new LogicJSONNumber(value));
            }
        }

        /// <summary>
        ///     Sets the json string value.
        /// </summary>
        public static void SetJSONString(LogicJSONObject jsonObject, string key, string value)
        {
            if (value != null)
            {
                jsonObject.Put(key, new LogicJSONString(value));
            }
        }

        /// <summary>
        ///     Sets the json boolean value.
        /// </summary>
        public static void SetJSONBoolean(LogicJSONObject jsonObject, string key, bool value)
        {
            jsonObject.Put(key, new LogicJSONBoolean(value));
        }
    }
}