using GridShared.Columns;
using GridShared.Utility;
using Microsoft.Extensions.Primitives;

namespace GridShared.Filtering
{
    public interface IGridFilterSettings
    {
        IQueryDictionary<StringValues> Query { get;  }
        IFilterColumnCollection FilteredColumns { get; }

        /// <summary>
        ///     Is filter settings int the init state
        /// </summary>
        bool IsInitState(IGridColumn column);
    }
}