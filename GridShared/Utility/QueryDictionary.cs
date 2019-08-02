using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;

namespace GridShared.Utility
{
    public class QueryDictionary<T> : Dictionary<string, T>, IQueryDictionary<T>
    {
        public T Get(string key)
        {
            T val;
            TryGetValue(key, out val);
            return val;
        }

        public void AddParameter(string parameterName, T parameterValue)
        {
            if (string.IsNullOrEmpty(parameterName))
                throw new ArgumentException("parameterName");

            if (ContainsKey(parameterName))
                this[parameterName] = parameterValue;
            else
                Add(parameterName, parameterValue);
        }

        public static QueryDictionary<StringValues> Convert(IQueryCollection collection)
        {
            QueryDictionary<StringValues> dictionary = new QueryDictionary<StringValues>();
            foreach (var element in collection)
            {
                if (dictionary.ContainsKey(element.Key))
                    dictionary[element.Key] = StringValues.Concat(dictionary[element.Key], element.Value);
                else
                    dictionary.Add(element.Key, element.Value);
            }
            return dictionary;
        }
    }
}
