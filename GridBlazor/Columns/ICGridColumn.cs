using GridShared;
using GridShared.Utility;
using System;
using System.Threading.Tasks;

namespace GridBlazor.Columns
{
    public interface ICGridColumn
    {
        bool IsSumEnabled { get; }

        string SumString { get; set; }

        bool IsAverageEnabled { get; }

        string AverageString { get; set; }

        bool IsMaxEnabled { get; }

        string MaxString { get; set; }

        bool IsMinEnabled { get; }

        string MinString { get; set; }

        /// <summary>
        ///     Keys for subgrid
        /// </summary>
        string[] SubGridKeys { get; }

        /// <summary>
        ///     Subgrid clients
        /// </summary>
        Func<object[], bool, bool, bool, bool, Task<IGrid>> SubGrids { get; }

        /// <summary>
        ///     Get foreign key values for subgrid records
        /// </summary>
        QueryDictionary<object> GetSubGridKeyValues(object item);
    }
}
