using GridShared.Utility;
using Microsoft.Extensions.Primitives;

namespace System.Collections.Generic
{
    public static class DictionaryExtensions
    {
        public static TValue Get<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key)
        {
            TValue val;
            source.TryGetValue(key, out val);
            return val;
        }

        public static void AddOrSet<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key, TValue value)
        {
            if (value == null)
                throw new ArgumentException("key");

            if (source.ContainsKey(key))
                source[key] = value;
            else
                source.Add(key, value);
        }

        public static QueryDictionary<StringValues> ToStringValuesDictionary(this QueryDictionary<string> query)
        {
            QueryDictionary<StringValues> dictionary = new QueryDictionary<StringValues>();

            if (query == null)
                return dictionary;

            foreach (var element in query)
            {
                string[] values = element.Value.Split('|');

                StringValues value = new StringValues();
                switch (values.Length)
                {
                    case 0: value = StringValues.Empty; break;
                    case 1: value = new StringValues(values[0]); break;
                    default: value = new StringValues(values); break;
                }

                if (dictionary.ContainsKey(element.Key))
                    dictionary[element.Key] = StringValues.Concat(dictionary[element.Key], value);
                else
                    dictionary.Add(element.Key, value);
            }
            return dictionary;
        }

        public static QueryDictionary<string> ToStringDictionary(this QueryDictionary<StringValues> query)
        {
            QueryDictionary<string> dictionary = new QueryDictionary<string>();

            if (query == null)
                return dictionary;

            foreach (var element in query)
            {
                string value = string.Join("|", element.Value.ToArray());
                dictionary.Add(element.Key, value);
            }
            return dictionary;
        }
    }
}
