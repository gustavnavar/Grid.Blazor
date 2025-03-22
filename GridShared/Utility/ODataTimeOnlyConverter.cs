#if ! NETSTANDARD2_1 && !NET5_0
using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GridShared.Utility
{
    public class ODataTimeOnlyConverter : JsonConverter<TimeOnly>
    {
        private static readonly string _TimeOnly = "HH':'mm':'sszzz";

        public override TimeOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Debug.Assert(typeToConvert == typeof(TimeOnly));
            return TimeOnly.Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, TimeOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(ToTimeOnlyString(value));
        }

        public static string ToTimeOnlyString(TimeOnly time)
        {
            return time.ToString(_TimeOnly);
        }
    }
}
#endif
