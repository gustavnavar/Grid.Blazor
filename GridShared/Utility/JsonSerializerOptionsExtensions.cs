using System;
using System.Linq;
using System.Text.Json;

namespace GridShared.Utility
{
    public static class JsonSerializerOptionsExtensions
    {
        public static JsonSerializerOptions AddOdataSupport(this JsonSerializerOptions jsonOptions)
        {
            jsonOptions.IgnoreNullValues = true;
            // required for Blazor WA
            var converters = jsonOptions.Converters.Where(r => r.CanConvert(typeof(DateTime)));
            if (converters != null)
                for (int i = converters.Count() - 1; i >= 0; i--)
                {
                    jsonOptions.Converters.Remove(converters.ElementAt(i));
                }
            // required for Blazor WA
            jsonOptions.Converters.Add(new ODataDateTimeConverter());
            return jsonOptions;
        }
    }
}
