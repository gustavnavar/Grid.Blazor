using GridBlazor.Pagination;
using GridShared;
using GridShared.Columns;
using GridShared.Filtering;
using GridShared.Utility;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Primitives;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace GridBlazor
{
    public interface ICGrid<T> : ICGrid, IGrid<T>
    {
        /// <summary>
        ///     Function to init values for columns in the Create form
        /// </summary>
        Func<T, Task> InitCreateValues { get; set; }
    }

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
        (string, string)[] SubGridKeys { get; }

        /// <summary>
        ///     Subgrid clients
        /// </summary>
        Func<object[], Task<ICGrid>> SubGrids { get; }

        /// <summary>
        ///     Subgrids state
        /// </summary>
        bool SubGridsOpened { get; }

        /// <summary>
        ///     Set or get default value of rearrange column
        /// </summary>
        public bool RearrangeColumnEnabled { get; set; }

        Type Type { get; }

        string Url { get; }

        HttpClient HttpClient { get; }

        /// <summary>
        ///     Get foreign key values for subgrid records
        /// </summary>
        QueryDictionary<object> GetSubGridKeyValues(object item);

        /// <summary>
        ///     Get primary key values for CRUD
        /// </summary>
        object[] GetPrimaryKeyValues(object item);

        /// <summary>
        ///     Get primary keys for CRUD
        /// </summary>
        string[] GetPrimaryKeys();

        bool DataAnnotationsValidation { get; set; }

        IGridSettingsProvider Settings { get; }

        IEnumerable<object> SelectedItems { get; set; }

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

        void RemoveAllFilters();

        Task DownloadExcel(IJSRuntime js, string filename);

        /// <summary>
        /// Changes postion of instertingColumn to appear before targetColumn
        /// </summary>
        /// <param name="targetColumn">Column which will be moved</param>
        /// <param name="insertingColumn">Column before which it will be inserted</param>
        /// <returns>Retruns true if column was sucessfully inserted before target otherwise false</returns>
        Task<bool> InsertColumn(IGridColumn targetColumn, IGridColumn insertingColumn);

        /// <summary>
        ///     Get and set export to an Excel file
        /// </summary>
        bool ExcelExport { get; }

        /// <summary>
        ///     Get and set export all rows to an Excel file
        /// </summary>
        bool ExcelExportAllRows { get; }

        /// <summary>
        ///     Get and set Excel file name
        /// </summary>
        string ExcelExportFileName { get; }

        /// <summary>
        ///     Get and set custom create component
        /// </summary>
        Type CreateComponent { get; }

        /// <summary>
        ///     Get and set custom read component
        /// </summary>
        Type ReadComponent { get; }

        /// <summary>
        ///     Get and set custom update component
        /// </summary>
        Type UpdateComponent { get; }

        /// <summary>
        ///     Get and set custom Delete component
        /// </summary>
        Type DeleteComponent { get; }

        /// <summary>
        ///     Get and set custom Button components dictionary
        /// </summary>
        QueryDictionary<(string Label, Nullable<MarkupString> Content, Type ComponentType, IList<Action<object>> Actions, IList<Func<object, Task>> Functions, object Object)> ButtonComponents { get; }

        /// <summary>
        ///     Get and set custom create component actions
        /// </summary>
        IList<Action<object>> CreateActions { get;  }

        /// <summary>
        ///     Get and set custom create component functions
        /// </summary>
        IList<Func<object,Task>> CreateFunctions { get; }

        /// <summary>
        ///     Get and set custom create component object
        /// </summary>
        object CreateObject { get; }

        /// <summary>
        ///     Get and set custom read component actions
        /// </summary>
        IList<Action<object>> ReadActions { get; }

        /// <summary>
        ///     Get and set custom read component functions
        /// </summary>
        IList<Func<object, Task>> ReadFunctions { get; }

        /// <summary>
        ///     Get and set custom read component object
        /// </summary>
        object ReadObject { get; }

        /// <summary>
        ///     Get and set custom update component actions
        /// </summary>
        IList<Action<object>> UpdateActions { get; }

        /// <summary>
        ///     Get and set custom update component functions
        /// </summary>
        IList<Func<object, Task>> UpdateFunctions { get; }

        /// <summary>
        ///     Get and set custom update component object
        /// </summary>
        object UpdateObject { get; }

        /// <summary>
        ///     Get and set custom delete component actions
        /// </summary>
        IList<Action<object>> DeleteActions { get; }

        /// <summary>
        ///     Get and set custom delete component functions
        /// </summary>
        IList<Func<object, Task>> DeleteFunctions { get; }

        /// <summary>
        ///     Get and set custom delete component object
        /// </summary>
        object DeleteObject { get; }

        /// <summary>
        ///     Get and set the modifier key
        /// </summary>
        ModifierKey ModifierKey { get; }


        /// <summary>
        ///     Get and set the modifier selection key
        /// </summary>
        Nullable<ModifierKey> SelectionKey { get;  }


        /// <summary>
        ///     Get and set keyboard utilization
        /// </summary>
        bool Keyboard { get; }

        /// <summary>
        ///     Get and set OData interface
        /// </summary>
        ServerAPI ServerAPI { get; }

        /// <summary>
        ///     Fixed column values for the grid
        /// </summary>
        QueryDictionary<object> FixedValues { get; set; }

        /// <summary>
        ///     Fixed column values for the OData url expand parameter
        /// </summary>
        IEnumerable<string> ODataExpandList { get; set; }

        /// <summary>
        ///     Override OData url expand parameter with list
        /// </summary>
        bool ODataOverrideExpandList { get; set; }

        /// <summary>
        ///     Get OData pre-processor parameters (expand and filter)
        /// </summary>
        string GetODataPreProcessorParameters();

        /// <summary>
        ///     Get OData processor parameters (sorting and paging)
        /// </summary>
        string GetODataProcessorParameters();

        /// <summary>
        ///     Get OData expand parameter
        /// </summary>
        string GetODataExpandParameters();

        /// <summary>
        ///     Get OData filter parameter
        /// </summary>
        string GetODataFilterParameters();

        /// <summary>
        ///     Get OData pager parameter
        /// </summary>
        string GetODataPagerParameters();

        /// <summary>
        ///     Get OData sort parameter
        /// </summary>
        string GetODataSortParameters();

        /// <summary>
        ///     Create button label
        /// </summary>
        string CreateLabel { get; set; }

        /// <summary>
        ///     Read button label
        /// </summary>
        string ReadLabel { get; set; }

        /// <summary>
        ///     Update button label
        /// </summary>
        string UpdateLabel { get; set; }

        /// <summary>
        ///     Delete button label
        /// </summary>
        string DeleteLabel { get; set; }

        /// <summary>
        ///     Create button tooltip
        /// </summary>
        string CreateTooltip { get; set; }

        /// <summary>
        ///     Read button tooltip
        /// </summary>
        string ReadTooltip { get; set; }

        /// <summary>
        ///     Update button tooltip
        /// </summary>
        string UpdateTooltip { get; set; }

        /// <summary>
        ///     Delete button tooltip
        /// </summary>
        string DeleteTooltip { get; set; }

        /// <summary>
        ///     Create form label
        /// </summary>
        public string CreateFormLabel { get; set; }

        /// <summary>
        ///     Read form label
        /// </summary>
        public string ReadFormLabel { get; set; }

        /// <summary>
        ///     Update form label
        /// </summary>
        public string UpdateFormLabel { get; set; }

        /// <summary>
        ///     Delete form label
        /// </summary>
        public string DeleteFormLabel { get; set; }

        // <summary>
        ///     Create CRUD confirmation fields
        /// </summary>
        bool CreateConfirmation { get; set; }

        int CreateConfirmationWidth { get; set; }

        int CreateConfirmationLabelWidth { get; set; }

        /// <summary>
        ///     Update CRUD confirmation fields
        /// </summary>
        bool UpdateConfirmation { get; set; }

        int UpdateConfirmationWidth { get; set; }

        int UpdateConfirmationLabelWidth { get; set; }

        /// <summary>
        ///     Delete CRUD confirmation fields
        /// </summary>
        bool DeleteConfirmation { get; set; }

        int DeleteConfirmationWidth { get; set; }

        int DeleteConfirmationLabelWidth { get; set; }

        /// <summary>
        ///     Header CRUD buttons
        /// </summary>
        bool HeaderCrudButtons { get; set; }

        /// <summary>
        ///     Show errors on Grid when getting data
        /// </summary>
        bool ShowErrorsOnGrid { get; set; }

        /// <summary>
        ///     Throw exceptions when getting data
        /// </summary>
        bool ThrowExceptions { get; set; }

        /// <summary>
        ///     Error string to be shown on the view
        /// </summary>
        string Error { get; set; }

        /// <summary>
        ///     Go to Edit form after insert row
        /// </summary>
        bool EditAfterInsert { get; set; }

        /// <summary>
        ///     Get column values to display
        /// </summary>
        [Obsolete("This method is obsolete. Use the new async GetGroupValues() method.", false)]
        IList<object> GetValuesToDisplay(string columnName, IEnumerable<object> items);
    }
}