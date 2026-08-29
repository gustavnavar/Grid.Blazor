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

        /// <summary>
        ///     The ISO value for a native HTML date input, which the browser then renders in the
        ///     reader's locale. Client to server only — never show it to a reader.
        /// </summary>
        string GetFormatedDateTime(object value, string type);

        /// <summary>
        ///     The value written for a reader: the column's own format when it defines one, the
        ///     browser's locale otherwise.
        /// </summary>
        string GetDisplayDateTime(object value, string type);

        /// <summary>
        ///     The hint for a text input of this type, spelled the way this column writes its
        ///     values.
        /// </summary>
        string GetDateTimePlaceholder(string type);
    }
}
