using System;
using System.Collections.Generic;

namespace GridShared.Utility
{
    public class QueryDictionary<T> : Dictionary<string, T>, IQueryDictionary<T>
    {
        T IQueryDictionary<T>.Get(string key)
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
    }
}
