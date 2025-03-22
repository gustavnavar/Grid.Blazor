using System;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace GridShared.Utility
{
    public static class JsonSerializerOptionsExtensions
    {
        public static JsonSerializerOptions AddOdataSupport(this JsonSerializerOptions jsonOptions)
        {
#if NETSTANDARD2_1
            jsonOptions.IgnoreNullValues = true;
#else
            jsonOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
#endif
            // required for Blazor WA
            var converters = jsonOptions.Converters.Where(r => r.CanConvert(typeof(DateTime)));
            if (converters != null)
                for (int i = converters.Count() - 1; i >= 0; i--)
                {
                    jsonOptions.Converters.Remove(converters.ElementAt(i));
                }
            // required for Blazor WA
            jsonOptions.Converters.Add(new ODataDateTimeConverter());
            jsonOptions.Converters.Add(new ODataTimeSpanConverter());
#if ! NETSTANDARD2_1 && !NET5_0
            jsonOptions.Converters.Add(new ODataDateOnlyConverter());
            jsonOptions.Converters.Add(new ODataTimeOnlyConverter());
#endif

            converters = jsonOptions.Converters.Where(r => r.CanConvert(typeof(Enum)));
            if (converters != null)
                for (int i = converters.Count() - 1; i >= 0; i--)
                {
                    jsonOptions.Converters.Remove(converters.ElementAt(i));
                }
            jsonOptions.Converters.Add(new JsonStringEnumConverter(null));
            return jsonOptions;
        }

        public static JsonSerializerOptions AddByteArraySupport(this JsonSerializerOptions jsonOptions)
        {
#if NETSTANDARD2_1
            jsonOptions.IgnoreNullValues = true;
#else
            jsonOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
#endif
            // required for Blazor WA
            var converters = jsonOptions.Converters.Where(r => r.CanConvert(typeof(byte[])));
            if (converters != null)
                for (int i = converters.Count() - 1; i >= 0; i--)
                {
                    jsonOptions.Converters.Remove(converters.ElementAt(i));
                }
            // required for Blazor WA
            jsonOptions.Converters.Add(new ByteArrayConverter());
            return jsonOptions;
        }
    }
}
