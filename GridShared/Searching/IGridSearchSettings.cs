using GridShared.Utility;
using Microsoft.Extensions.Primitives;

namespace GridShared.Searching
{
    public interface IGridSearchSettings
    {
        IQueryDictionary<StringValues> Query { get; }
        string SearchValue { get; }
    }
}
