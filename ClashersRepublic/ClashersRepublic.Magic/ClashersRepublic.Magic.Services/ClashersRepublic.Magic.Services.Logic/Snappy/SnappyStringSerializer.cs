namespace ClashersRepublic.Magic.Services.Logic.Snappy
{
    using MongoDB.Bson;
    using MongoDB.Bson.Serialization;
    using MongoDB.Bson.Serialization.Serializers;

    public class SnappyStringSerializer : SerializerBase<SnappyString>
    {
        public override void Serialize(BsonSerializationContext context, BsonSerializationArgs args, SnappyString value)
        {
            context.Writer.WriteBytes(value.GetArray());
        }

        public override SnappyString Deserialize(BsonDeserializationContext context, BsonDeserializationArgs args)
        {
            if (context.Reader.CurrentBsonType != BsonType.Binary)
            {
                throw new BsonSerializationException("Invalid bson type");
            }

            return new SnappyString(context.Reader.ReadBytes());
        }
    }
}