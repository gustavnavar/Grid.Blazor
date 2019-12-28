using Microsoft.Extensions.Primitives;
using System;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GridShared.Utility
{
    public class StringValuesConverter : JsonConverter<StringValues>
    {
        public override StringValues Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            string[] values = reader.GetString().Split('|');

            switch (values.Length)
            {
                case 0: return StringValues.Empty;
                case 1: return new StringValues(values[0]);
                default: return new StringValues(values);
            }
        }

        public override void Write(Utf8JsonWriter writer, StringValues value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(string.Join("|", value.ToArray()));
        }
    }
}