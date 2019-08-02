using Microsoft.Extensions.Primitives;
using Newtonsoft.Json;
using System;
using System.Text;

namespace GridShared.Utility
{
    public static class StringExtensions
    {
        public static QueryDictionary<StringValues> GetQuery(string gridState)
        {
            string queryString = gridState.GridStateDecode();
            return JsonConvert.DeserializeObject<QueryDictionary<StringValues>>(queryString, new StringValuesConverter());
        }

        public static string GridStateEncode(this string text)
        {
            var plainTextBytes = Encoding.UTF8.GetBytes(text);
            string base64Str = Convert.ToBase64String(plainTextBytes);
            base64Str = base64Str.Replace('+', '.');
            base64Str = base64Str.Replace('/', '_');
            base64Str = base64Str.Replace('=', '-');
            return base64Str;
        }

        public static string GridStateDecode(this string text)
        {
            text = text.Replace('.', '+');
            text = text.Replace('_', '/');
            text = text.Replace('-', '=');
            var base64EncodedBytes = Convert.FromBase64String(text);
            return Encoding.UTF8.GetString(base64EncodedBytes);
        }
    }
}
