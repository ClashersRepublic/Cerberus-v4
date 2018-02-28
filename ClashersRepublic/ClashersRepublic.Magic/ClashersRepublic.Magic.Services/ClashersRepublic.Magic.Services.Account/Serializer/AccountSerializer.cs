namespace ClashersRepublic.Magic.Services.Account.Serializer
{
    using System;
    using ClashersRepublic.Magic.Services.Account.Game;
    using ClashersRepublic.Magic.Titan.Json;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;

    internal class AccountSerializer : JsonConverter<Account>
    {
        public override void WriteJson(JsonWriter writer, Account value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
            }
            else
            {
                LogicJSONObject jsonRoot = new LogicJSONObject();
                value.Save(jsonRoot);
                writer.WriteRaw(LogicJSONParser.CreateJSONString(jsonRoot));
            }
        }

        public override Account ReadJson(JsonReader reader, Type objectType, Account existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                existingValue = null;
            }
            else
            {
                existingValue = new Account();
                existingValue.Load((LogicJSONObject) LogicJSONParser.Parse(JObject.Load(reader).ToString()));
            }

            return existingValue;
        }
    }
}