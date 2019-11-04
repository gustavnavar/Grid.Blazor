using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;

namespace GridShared.Utility
{
    public class StringValuesConverter : JsonConverter<StringValues>
    {
        public override void WriteJson(JsonWriter writer, StringValues values, JsonSerializer serializer)
        {
            writer.WriteValue(string.Join("|", values.ToArray()));
        }

        public override StringValues ReadJson(JsonReader reader, Type objectType, StringValues existingValue, bool hasExistingValue, JsonSerializer serializer)
        {
            string[] values = reader.Value.ToString().Split('|');

            switch (values.Length)
            {
                case 0: return StringValues.Empty;
                case 1: return new StringValues(values[0]);
                default: return new StringValues(values);
            }
        }
    }
}