using System.Collections.Generic;

namespace GridShared.Utility
{
    public interface IQueryDictionary<T> : IDictionary<string, T>
    {
        T Get(string key);
        void AddParameter(string parameterName, T parameterValue);
    }
}
