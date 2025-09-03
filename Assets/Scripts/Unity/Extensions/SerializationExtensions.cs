using Newtonsoft.Json;

using System;

namespace Scripts.Unity
{
    public static class SerializationExtensions
    {
        public static void Init()
        {
            JsonConvert.DefaultSettings = () =>
            {
                var settings = new JsonSerializerSettings();
                settings.Converters.Add(GuidConverter.Default);
                return settings;
            };
        }
    }

    public class GuidConverter : JsonConverter<Guid>
    {
        public static readonly GuidConverter Default = new();

        public override void WriteJson(JsonWriter writer, Guid value, JsonSerializer serializer)
        {
            serializer.Serialize(writer, value.ToString());
        }

        public override Guid ReadJson(JsonReader reader, Type objectType, Guid existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            return Guid.Parse(serializer.Deserialize<string>(reader));
        }
    }
}
