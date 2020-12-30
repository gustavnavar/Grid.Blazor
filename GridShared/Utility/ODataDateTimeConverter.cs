using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GridShared.Utility
{
    public class ODataDateTimeConverter : JsonConverter<DateTime>
    {
        private static readonly string _ISO8601DateTime = "yyyy'-'MM'-'dd'T'HH':'mm':'sszzz";

        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Debug.Assert(typeToConvert == typeof(DateTime));
            return DateTime.Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(ToDateString(value));
        }

        public static string ToDateString(DateTime date)
        {
            if (date.Kind == DateTimeKind.Unspecified)
                date = DateTime.SpecifyKind(date, DateTimeKind.Local);
            return date.ToString(_ISO8601DateTime);
        }
    }
}
