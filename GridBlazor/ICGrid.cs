using GridShared;
using GridBlazor.Pagination;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Primitives;
using GridShared.Columns;
using GridShared.Filtering;

namespace GridBlazor
{
    /// <summary>
    ///     Grid.Mvc interface
    /// </summary>
    public interface ICGrid : IGrid, IGridOptions
    {
        /// <summary>
        ///     Grid component options
        /// </summary>
        GridOptions ComponentOptions { get; }

        /// <summary>
        ///     Pager for the grid
        /// </summary>
        IGridPager Pager { get; }

        /// <summary>
        ///     Keys for subgrid
        /// </summary>
        string[] SubGridKeys { get; }

        /// <summary>
        ///     Subgrid clients
        /// </summary>
        Func<object[], Task<ICGrid>> SubGrids { get; }

        Type Type { get; }

        /// <summary>
        ///     Get foreign key values for subgrid records
        /// </summary>
        string[] GetSubGridKeyValues(object item);

        /// <summary>
        ///     Get primary key values for CRUD
        /// </summary>
        object[] GetPrimaryKeyValues(object item);

        IGridSettingsProvider Settings { get; }

        /// <summary>
        ///    Set items from the server api
        /// </summary>
        Task UpdateGrid();

        void AddQueryParameter(string parameterName, StringValues parameterValue);

        void RemoveQueryParameter(string parameterName);

        void AddQueryString(string parameterName, string parameterValue);

        void ChangeQueryString(string parameterName, string oldParameterValue, string newParameterValue);

        void RemoveQueryString(string parameterName, string parameterValue);

        void AddFilterParameter(IGridColumn column, FilterCollection filters);

        void RemoveFilterParameter(IGridColumn column);

    }
}