using Microsoft.Extensions.Primitives;
using System;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace GridShared.Utility
{
    public static class StringExtensions
    {
        public static QueryDictionary<StringValues> GetQuery(string gridState)
        {
            string queryString = gridState.GridStateDecode();
            var jsonOptions = new JsonSerializerOptions();
            jsonOptions.Converters.Add(new StringValuesConverter());
            return JsonSerializer.Deserialize<QueryDictionary<StringValues>>(queryString, jsonOptions);
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

        public static string RandomString(int length)
        {
            Random random = new Random();
            const string chars = "abcdefghijklmnopqrstuvwxyz0123456789";
            return new string(Enumerable.Repeat(chars, length)
              .Select(s => s[random.Next(s.Length)]).ToArray());
        }
    }
}
