using GridShared;
using GridShared.Sorting;
using GridShared.Utility;
using System;
using System.Threading.Tasks;

namespace GridBlazor.Columns
{
    public interface ICGridColumn
    {
        /// <summary>
        ///     Keys for subgrid
        /// </summary>
        (string,string)[] SubGridKeys { get; }

        /// <summary>
        ///     Subgrid clients
        /// </summary>
        Func<object[], bool, bool, bool, bool, Task<IGrid>> SubGrids { get; }

        /// <summary>
        ///     Show subgrid clients on Create form
        /// </summary>
        bool ShowCreateSubGrids { get; }

        GridSortDirection? InitialDirection { get; }

        /// <summary>
        ///     Get foreign key values for subgrid records
        /// </summary>
        QueryDictionary<object> GetSubGridKeyValues(object item);

        string GetFormatedDateTime(object value, string type);
    }
}
