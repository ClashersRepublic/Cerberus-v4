namespace ClashersRepublic.Magic.Services.Logic.Math
{
    using System;
    using ClashersRepublic.Magic.Services.Logic.Log;
    using ClashersRepublic.Magic.Titan.Math;
    using Newtonsoft.Json;

    public class LogicLongSerializer : JsonConverter<LogicLong>
    {
        public override void WriteJson(JsonWriter writer, LogicLong value, JsonSerializer serializer)
        {
            writer.WriteStartArray();
            writer.WriteValue(value.GetHigherInt());
            writer.WriteValue(value.GetLowerInt());
            writer.WriteEndArray();
        }

        public override LogicLong ReadJson(JsonReader reader, Type objectType, LogicLong existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.StartArray)
            {
                int hVal = reader.ReadAsInt32() ?? 0;
                int lVal = reader.ReadAsInt32() ?? 0;

                reader.Read();

                if (reader.TokenType == JsonToken.EndArray)
                {
                    return new LogicLong(hVal, lVal);
                }
            }

            Logging.Warning(this, "LogicLongSerializer::readJson invalid token type, " + reader.TokenType);

            return new LogicLong(-1, -1);
        }
    }
}