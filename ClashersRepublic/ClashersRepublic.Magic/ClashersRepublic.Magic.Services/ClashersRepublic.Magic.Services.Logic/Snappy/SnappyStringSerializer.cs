namespace ClashersRepublic.Magic.Services.Logic.Snappy
{
    using System;
    using Newtonsoft.Json;

    public class SnappyStringSerializer : JsonConverter<SnappyString>
    {
        public override void WriteJson(JsonWriter writer, SnappyString value, JsonSerializer serializer)
        {
            writer.WriteValue(value.GetArray());
        }

        public override SnappyString ReadJson(JsonReader reader, Type objectType, SnappyString existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return new SnappyString(Convert.FromBase64String((string) reader.Value));
        }
    }
}