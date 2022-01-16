using GridShared.Utility;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

namespace GridShared
{
    public interface IMemoryDataService<T> : ICrudDataService<T>
    {
        IList<T> Items { get; }
        ItemsDTO<T> GetGridRows(QueryDictionary<StringValues> query);
    }
}
