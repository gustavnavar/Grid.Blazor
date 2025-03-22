#if ! NETSTANDARD2_1 && !NET5_0
using System;
using System.Diagnostics;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GridShared.Utility
{
    public class ODataDateOnlyConverter : JsonConverter<DateOnly>
    {
        private static readonly string _DateOnly = "yyyy'-'MM'-'dd";

        public override DateOnly Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            Debug.Assert(typeToConvert == typeof(DateOnly));
            return DateOnly.Parse(reader.GetString());
        }

        public override void Write(Utf8JsonWriter writer, DateOnly value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(ToDateOnlyString(value));
        }

        public static string ToDateOnlyString(DateOnly date)
        {
            return date.ToString(_DateOnly);
        }
    }
}
#endif
