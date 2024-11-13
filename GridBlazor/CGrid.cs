using GridBlazor.Columns;
using GridBlazor.DataAnnotations;
using GridBlazor.Filtering;
using GridBlazor.OData;
using GridBlazor.Pages;
using GridBlazor.Pagination;
using GridBlazor.Resources;
using GridBlazor.Searching;
using GridBlazor.Sorting;
using GridShared;
using GridShared.Columns;
using GridShared.DataAnnotations;
using GridShared.Filtering;
using GridShared.Grouping;
using GridShared.Pagination;
using GridShared.Sorting;
using GridShared.Style;
using GridShared.Totals;
using GridShared.Utility;
using Microsoft.AspNetCore.Components;
#if ! NETSTANDARD2_1
using Microsoft.AspNetCore.Components.Web.Virtualization;
#endif
using Microsoft.Extensions.Primitives;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace GridBlazor
{
    /// <summary>
    ///     Grid.Mvc base class
    /// </summary>
    public class CGrid<T> : ICGrid<T>
    {
        private Func<T, string> _rowCssClassesContraint;

        private QueryDictionary<StringValues> _query;
        private IGridSettingsProvider _settings;
        private readonly IGridAnnotationsProvider _annotations;
        private readonly IColumnBuilder<T> _columnBuilder;
        private readonly GridColumnCollection<T> _columnsCollection;
        private readonly PagerGridODataProcessor<T> _currentPagerODataProcessor;
        private readonly FilterGridODataProcessor<T> _currentFilterODataProcessor;
        private readonly SortGridODataProcessor<T> _currentSortODataProcessor;
        private readonly SearchGridODataProcessor<T> _currentSearchODataProcessor;
        private readonly ExpandGridODataProcessor<T> _currentExpandODataProcessor;
        private IEnumerable<T> _items;
        private IEnumerable<object> _selectedItems;
        private int _displayingItemsCount = -1; // count of displaying items (if using pagination
        private bool _noTotals = false; // controls calls to the back-end for virtualized grids to include totals or not
        private PagingType _pagingType;
        private IGridPager _pager;
        private HttpClient _httpClient; 

        private Func<QueryDictionary<StringValues>, ItemsDTO<T>> _dataService;
        private Func<QueryDictionary<StringValues>, Task<ItemsDTO<T>>> _dataServiceAsync;
        private Func<QueryDictionary<string>, Task<ItemsDTO<T>>> _grpcService;
        private ICrudDataService<T> _crudDataService;
        private IMemoryDataService<T> _memoryDataService;

        public CGrid(HttpClient httpClient, string url, IQueryDictionary<StringValues> query, bool renderOnlyRows,
                   Action<IGridColumnCollection<T>> columns = null, CultureInfo cultureInfo = null,
                   IColumnBuilder<T> columnBuilder = null)
                   : this(httpClient, url, null, null, null, null, query, renderOnlyRows, columns, cultureInfo, columnBuilder)
        {
        }

        [Obsolete("This constructor is obsolete. Use one including an HttpClient parameter.", true)]
        public CGrid(string url, IQueryDictionary<StringValues> query, bool renderOnlyRows,
            Action<IGridColumnCollection<T>> columns = null, CultureInfo cultureInfo = null,
            IColumnBuilder<T> columnBuilder = null)
            : this(null, url, null, null, null, null, query, renderOnlyRows, columns, cultureInfo, columnBuilder)
        {
        }

        public CGrid(Func<QueryDictionary<StringValues>, ItemsDTO<T>> dataService,
            IQueryDictionary<StringValues> query, bool renderOnlyRows,
            Action<IGridColumnCollection<T>> columns = null, CultureInfo cultureInfo = null,
            IColumnBuilder<T> columnBuilder = null)
            : this(null, null, dataService, null, null, null, query, renderOnlyRows, columns, cultureInfo, columnBuilder)
        {
        }

        public CGrid(Func<QueryDictionary<StringValues>, Task<ItemsDTO<T>>> dataServiceAsync,
            IQueryDictionary<StringValues> query, bool renderOnlyRows,
            Action<IGridColumnCollection<T>> columns = null, CultureInfo cultureInfo = null,
            IColumnBuilder<T> columnBuilder = null)
            : this(null, null, null, dataServiceAsync, null, null, query, renderOnlyRows, columns, cultureInfo, columnBuilder)
        {
        }

        public CGrid(Func<QueryDictionary<string>, Task<ItemsDTO<T>>> grpcService,
            QueryDictionary<string> query, bool renderOnlyRows,
            Action<IGridColumnCollection<T>> columns = null, CultureInfo cultureInfo = null,
            IColumnBuilder<T> columnBuilder = null)
            : this(null, null, null, null, grpcService, null, query.ToStringValuesDictionary(), renderOnlyRows, columns, cultureInfo, columnBuilder)
        {
        }

        private CGrid(HttpClient httpClient, string url,
            Func<QueryDictionary<StringValues>, ItemsDTO<T>> dataService,
            Func<QueryDictionary<StringValues>, Task<ItemsDTO<T>>> dataServiceAsync,
            IQueryDictionary<StringValues> query, bool renderOnlyRows,
            Action<IGridColumnCollection<T>> columns = null, CultureInfo cultureInfo = null,
            IColumnBuilder<T> columnBuilder = null)
            : this(httpClient, url, dataService, dataServiceAsync, null, null, query, renderOnlyRows, columns, cultureInfo, columnBuilder)
        {
        }

        public CGrid(HttpClient httpClient, string url, IMemoryDataService<T> memoryDataService, IQueryDictionary<StringValues> query,
            bool renderOnlyRows, Action<IGridColumnCollection<T>> columns = null, CultureInfo cultureInfo = null,
            IColumnBuilder<T> columnBuilder = null)
            : this(httpClient, url, null, null, null, memoryDataService, query, renderOnlyRows, columns, cultureInfo, columnBuilder)
        {
        }

        public CGrid(Func<QueryDictionary<StringValues>, ItemsDTO<T>> dataService,
            IMemoryDataService<T> memoryDataService,
            IQueryDictionary<StringValues> query, bool renderOnlyRows,
            Action<IGridColumnCollection<T>> columns = null, CultureInfo cultureInfo = null,
            IColumnBuilder<T> columnBuilder = null)
            : this(null, null, dataService, null, null, memoryDataService, query, renderOnlyRows, columns, cultureInfo, columnBuilder)
        {
        }

        public CGrid(Func<QueryDictionary<StringValues>, Task<ItemsDTO<T>>> dataServiceAsync,
            IMemoryDataService<T> memoryDataService,
            IQueryDictionary<StringValues> query, bool renderOnlyRows,
            Action<IGridColumnCollection<T>> columns = null, CultureInfo cultureInfo = null,
            IColumnBuilder<T> columnBuilder = null)
            : this(null, null, null, dataServiceAsync, null, memoryDataService, query, renderOnlyRows, columns, cultureInfo, columnBuilder)
        {
        }

        public CGrid(Func<QueryDictionary<string>, Task<ItemsDTO<T>>> grpcService,
            IMemoryDataService<T> memoryDataService,
            QueryDictionary<string> query, bool renderOnlyRows,
            Action<IGridColumnCollection<T>> columns = null, CultureInfo cultureInfo = null,
            IColumnBuilder<T> columnBuilder = null)
            : this(null, null, null, null, grpcService, memoryDataService, query.ToStringValuesDictionary(), renderOnlyRows, columns, cultureInfo, columnBuilder)
        {
        }

        private CGrid(HttpClient httpClient, string url,
            Func<QueryDictionary<StringValues>, ItemsDTO<T>> dataService,
            Func<QueryDictionary<StringValues>, Task<ItemsDTO<T>>> dataServiceAsync,
            Func<QueryDictionary<string>, Task<ItemsDTO<T>>> grpcService,
            IMemoryDataService<T> memoryDataService,
            IQueryDictionary<StringValues> query, bool renderOnlyRows,
            Action<IGridColumnCollection<T>> columns = null, CultureInfo cultureInfo = null,
            IColumnBuilder<T> columnBuilder = null)
        {
            _dataServiceAsync = dataServiceAsync;
            _dataService = dataService;
            _grpcService = grpcService;
            _memoryDataService = memoryDataService;
            _selectedItems = new List<object>();
            Items = new List<T>(); //response.Items;

            Url = url;
            _httpClient = httpClient;
            _query = query as QueryDictionary<StringValues>;

            //set up sort settings:
            _settings = new QueryStringGridSettingsProvider(_query);
            Sanitizer = new Sanitizer();
            if (cultureInfo != null)
                CultureInfo.CurrentCulture = cultureInfo;
            EmptyGridText = Strings.DefaultGridEmptyText;
            Language = Strings.Lang;

            _annotations = new GridAnnotationsProvider();

            //Set up column collection:
            if (columnBuilder == null)
                _columnBuilder = new DefaultColumnBuilder<T>(this, _annotations);
            else
                _columnBuilder = columnBuilder;
            _columnsCollection = new GridColumnCollection<T>(this, _columnBuilder, _settings.SortSettings);
            ComponentOptions = new GridOptions();

            Pager = new GridPager(this);

            ApplyGridSettings();
            SetInitSorting();

            _currentPagerODataProcessor = new PagerGridODataProcessor<T>(this);
            _currentSortODataProcessor = new SortGridODataProcessor<T>(this, _settings.SortSettings);
            _currentFilterODataProcessor = new FilterGridODataProcessor<T>(this, _settings.FilterSettings,
                _settings.SearchSettings);
            _currentSearchODataProcessor = new SearchGridODataProcessor<T>(this, _settings.SearchSettings);
            _currentExpandODataProcessor = new ExpandGridODataProcessor<T>(this);

            ComponentOptions.RenderRowsOnly = renderOnlyRows;
            columns?.Invoke(Columns);

            Mode = GridMode.Grid;
            CreateEnabled = false;
            ReadEnabled = false;
            UpdateEnabled = false;
            DeleteEnabled = false;

            ButtonComponents = new QueryDictionary<(string Label, Nullable<MarkupString> Content, Type ComponentType,
                IList<Action<object>> Actions, IList<Func<object, Task>> Functions, object Object)>();

            ButtonCrudComponents = new QueryDictionary<(string Label, Nullable<MarkupString> Content, Type ComponentType,
                GridMode GridMode, Func<T, bool> ReadMode, Func<T, bool> UpdateMode, Func<T, bool> DeleteMode,
                Func<T, Task<bool>> ReadModeAsync, Func<T, Task<bool>> UpdateModeAsync, Func<T, Task<bool>> DeleteModeAsync,
                IList<Action<object>> Actions, IList<Func<object, Task>> Functions, object Object)>();
        }

        public GridComponent<T> GridComponent { get; set; }

        /// <summary>
        /// Total count of items in the grid
        /// </summary>
        public int ItemsCount { get { return _pager.ItemsCount; } }

        public SearchOptions SearchOptions { get; set; } = new SearchOptions() { Enabled = false };

        public bool ExtSortingEnabled { get; set; }

        public bool HiddenExtSortingHeader { get; set; } = false;

        public bool GroupingEnabled { get; set; }

        public bool SyncButtonEnabled { get; set; } = false;

        public bool ClearFiltersButtonEnabled { get; set; } = false;
        
        public bool RearrangeColumnEnabled { get; set; }

        /// <summary>
        ///     Items, displaying in the grid view
        /// </summary>
        public IEnumerable<object> ItemsToDisplay
        {
            get { return (IEnumerable<object>)GetItemsToDisplay(); }
        }

        public IEnumerable<object> SelectedItems
        {
            get { return _selectedItems; }
            set { _selectedItems = value; }
        }

        /// <summary>
        ///     Methods returns items that will need to be displayed
        /// </summary>
        protected internal virtual IEnumerable<T> GetItemsToDisplay()
        {
            return Items;
        }

        public IGridColumnCollection<T> Columns
        {
            get { return _columnsCollection; }
        }

        IGridColumnCollection IGrid.Columns
        {
            get { return Columns; }
        }

        /// <summary>
        ///     Sets or get default value of sorting for all adding columns
        /// </summary>
        public bool DefaultSortEnabled
        {
            get { return _columnBuilder.DefaultSortEnabled; }
            set { _columnBuilder.DefaultSortEnabled = value; }
        }

        public GridSortMode GridSortMode
        {
            get { return _columnBuilder.DefaultGridSortMode; }
            set { _columnBuilder.DefaultGridSortMode = value; }
        }

        /// <summary>
        ///     Set or get default value of filtering for all adding columns
        /// </summary>
        public bool DefaultFilteringEnabled
        {
            get { return _columnBuilder.DefaultFilteringEnabled; }
            set { _columnBuilder.DefaultFilteringEnabled = value; }
        }

        public GridOptions ComponentOptions { get; set; }

        /// <summary>
        ///     items from server
        /// </summary>
        public IEnumerable<T> Items {
            get => _items;
            set {
                _displayingItemsCount = -1;
                _items = value;
            }
        }

        /// <summary>
        ///     Provides settings, using by the grid
        /// </summary>
        public IGridSettingsProvider Settings
        {
            get { return _settings; }
            /**
            set
            {
                _query = value.ToQuery() as QueryDictionary<StringValues>;
                if (_pager.CurrentPage > 0)
                    _query.Add(((GridPager)_pager).ParameterName, _pager.CurrentPage.ToString());

                UpdateQueryAndSettings();
            }
            */
        }

        public MethodInfo RemoveDiacritics { get; set; } = null;

        private void UpdateQueryAndSettings()
        {
            _settings = new QueryStringGridSettingsProvider(_query);
            SetInitSorting();
            _columnsCollection.SortSettings = _settings.SortSettings;
            _columnsCollection.UpdateColumnsSorting();
            ((GridPager)_pager).Query = _query;

            _currentSortODataProcessor.UpdateSettings(_settings.SortSettings);
            _currentFilterODataProcessor.UpdateSettings(_settings.FilterSettings, _settings.SearchSettings);
            _currentSearchODataProcessor.UpdateSettings(_settings.SearchSettings);
        }

        // keeps initial sorting on the client for OData grids
        private void SetInitSorting()
        {
            string[] sortings = Query.Get(((QueryStringSortSettings)_settings.SortSettings).ColumnQueryParameterName).Count > 0 ?
                Query.Get(((QueryStringSortSettings)_settings.SortSettings).ColumnQueryParameterName).ToArray() : null;

            if ((_settings.SortSettings.SortValues == null || _settings.SortSettings.SortValues.Count == 0)
                && (sortings == null || sortings.Length == 0)
                && string.IsNullOrWhiteSpace(_settings.SortSettings.ColumnName))
            {
                var column = _columnsCollection.FirstOrDefault(r => ((ICGridColumn)r).InitialDirection.HasValue);
                if (column != null)
                {
                    _settings.SortSettings.ColumnName = column.Name;
                    _settings.SortSettings.Direction = ((ICGridColumn)column).InitialDirection.Value;
                }
            }  
        }

        /// <summary>
        ///     Provides url used by the grid
        /// </summary>
        public string Url { get; set; }

        public HttpClient HttpClient 
        {
            get {
                if (_httpClient == null)
                    _httpClient = new HttpClient();
                return _httpClient; 
            }
        }

        /// <summary>
        ///     Provides DataService used by the grid
        /// </summary>
        public Func<QueryDictionary<StringValues>, ItemsDTO<T>> DataService { 
            get { return _dataService; }
            internal set { _dataService = value; }
        }

        public Func<QueryDictionary<StringValues>, Task<ItemsDTO<T>>> DataServiceAsync { 
            get { return _dataServiceAsync; }
            internal set { _dataServiceAsync = value; }
        }

        public Func<QueryDictionary<string>, Task<ItemsDTO<T>>> GrpcService
        {
            get { return _grpcService; }
            internal set { _grpcService = value; }
        }

        public ServerAPI ServerAPI { get; internal set; } = ServerAPI.ItemsDTO;

        internal ExpandGridODataProcessor<T> CurrentExpandODataProcessor { get { return _currentExpandODataProcessor; } }

        /// <summary>
        ///     Provides CrudDataService used by the grid
        /// </summary>
        public ICrudDataService<T> CrudDataService 
        { 
            get {
                if (ServerAPI == ServerAPI.OData && _crudDataService == null)
                    _crudDataService = new ODataService<T>(HttpClient, Url, this);
                return _crudDataService; 
            }
            set { _crudDataService = value; }
        }

        /// <summary>
        ///     Provides MemoryDataService used by the grid
        /// </summary>
        public IMemoryDataService<T> MemoryDataService
        {
            get { return _memoryDataService; }
            internal set { _memoryDataService = value; }
        }

        /// <summary>
        ///     Provides CrudFileService used by the grid
        /// </summary>
        public ICrudFileService<T> CrudFileService { get; set; }

        /// <summary>
        ///     Provides query, using by the grid
        /// </summary>
        public QueryDictionary<StringValues> Query
        {
            get { return _query; }
            set
            {
                _query = value;
                UpdateQueryAndSettings();
            }
        }

        /// <summary>
        ///     Count of current displaying items
        /// </summary>
        public virtual int DisplayingItemsCount
        {
            get
            {
                if (_displayingItemsCount >= 0)
                    return _displayingItemsCount;
                _displayingItemsCount = GetItemsToDisplay().Count();
                return _displayingItemsCount;
            }
        }

        /// <summary>
        ///     Enable or disable paging for the grid
        /// </summary>
        [Obsolete("This property is obsolete. Use PagingType property", true)]
        public bool EnablePaging
        {
            get { return _pagingType == PagingType.Pagination; }
            set { }
        }

        /// <summary>
        ///     Enable paging type for the grid
        /// </summary>
        public PagingType PagingType
        {
            get { return _pagingType; }
            set
            {
                if (_pagingType == value) return;
                _pagingType = value;
            }
        }

        public string Language { get; set; }

        /// <summary>
        ///     Gets or set Grid column values sanitizer
        /// </summary>
        public ISanitizer Sanitizer { get; set; }

        /// <summary>
        ///     Grid mode
        /// </summary>
        public GridMode Mode { get; internal set; }

        /// <summary>
        ///     Grid direction
        /// </summary>
        public GridDirection Direction { get; set; } = GridDirection.LTR;

        /// <summary>
        ///     Get value for table layout
        /// </summary>
        public TableLayout TableLayout { get; set; } = TableLayout.Auto;

        /// <summary>
        ///     Get value for table width
        /// </summary>
        public string Width { get; set; } = "auto";

        /// <summary>
        ///     Get value for table height
        /// </summary>
        public string Height { get; set; } = "auto";

        public bool ChangeVirtualizedHeight { get; set; } = false;

        public bool ModalForms { get; set; } = false;

        public string ModalWidth { get; set; }

        public string ModalHeight { get; set; }

        /// <summary>
        ///     Get and set export to an Excel file
        /// </summary>
        public bool ExcelExport { get; internal set; }

        /// <summary>
        ///     Get and set export all rows to an Excel file
        /// </summary>
        public bool ExcelExportAllRows { get; internal set; }

        /// <summary>
        ///     Get and set Excel file name
        /// </summary>
        public string ExcelExportFileName { get; internal set; }

        /// <summary>
        ///     Get value for creating items
        /// </summary>
        public bool CreateEnabled { get; internal set; }

        /// <summary>
        ///     Get value for reading items
        /// </summary>
        public bool ReadEnabled { get; internal set; }

        /// <summary>
        ///     Get value for reading items
        /// </summary>
        public Func<T, bool> FuncReadEnabled { get; internal set; }

        /// <summary>
        ///     Get value for updating items
        /// </summary>
        public bool UpdateEnabled { get; internal set; }

        /// <summary>
        ///     Get value for updating items
        /// </summary>
        public Func<T, bool> FuncUpdateEnabled { get; internal set; }

        /// <summary>
        ///     Get value for deleting items
        /// </summary>
        public bool DeleteEnabled { get; internal set; }

        /// <summary>
        ///     Get value for deleting items
        /// </summary>
        public Func<T, bool> FuncDeleteEnabled { get; internal set; }

        /// <summary>
        ///     Get and set custom create component
        /// </summary>
        public Type CreateComponent { get; internal set; }

        /// <summary>
        ///     Get and set custom read component
        /// </summary>
        public Type ReadComponent { get; internal set; }

        /// <summary>
        ///     Get and set custom update component
        /// </summary>
        public Type UpdateComponent { get; internal set; }

        /// <summary>
        ///     Get and set custom Delete component
        /// </summary>
        public Type DeleteComponent { get; internal set; }

        public IList<Action<object>> CreateActions { get; internal set; }

        public IList<Func<object,Task>> CreateFunctions { get; internal set; }

        public object CreateObject { get; internal set; }

        public IList<Action<object>> ReadActions { get; internal set; }

        public IList<Func<object, Task>> ReadFunctions { get; internal set; }

        public object ReadObject { get; internal set; }

        public IList<Action<object>> UpdateActions { get; internal set; }

        public IList<Func<object, Task>> UpdateFunctions { get; internal set; }

        public object UpdateObject { get; internal set; }

        public IList<Action<object>> DeleteActions { get; internal set; }

        public IList<Func<object, Task>> DeleteFunctions { get; internal set; }

        public object DeleteObject { get; internal set; }

        public QueryDictionary<(string Label, Nullable<MarkupString> Content, Type ComponentType, IList<Action<object>> Actions, IList<Func<object, Task>> Functions, object Object)> ButtonComponents { get; internal set; }

        public QueryDictionary<(string Label, Nullable<MarkupString> Content, Type ComponentType, GridMode GridMode, Func<T,bool> ReadMode, Func<T, bool> UpdateMode, Func<T, bool> DeleteMode, Func<T, Task<bool>> ReadModeAsync, Func<T, Task<bool>> UpdateModeAsync, Func<T, Task<bool>> DeleteModeAsync, IList<Action<object>> Actions, IList<Func<object, Task>> Functions, object Object)> ButtonCrudComponents { get; internal set; }
        
        public bool Keyboard { get; internal set; } = false;

        public ModifierKey ModifierKey { get; internal set; } = ModifierKey.CtrlKey;
        public Nullable<ModifierKey> SelectionKey { get; internal set; } = ModifierKey.ShiftKey;

        /// <summary>
        ///     Sum enabled for some columns
        /// </summary>
        public bool IsSumEnabled { get { return Columns.Any(r => ((ITotalsColumn)r).IsSumEnabled); } }

        /// <summary>
        ///     Average enabled for some columns
        /// </summary>
        public bool IsAverageEnabled { get { return Columns.Any(r => ((ITotalsColumn)r).IsAverageEnabled); } }

        /// <summary>
        ///     Max enabled for some columns
        /// </summary>
        public bool IsMaxEnabled { get { return Columns.Any(r => ((ITotalsColumn)r).IsMaxEnabled); } }

        /// <summary>
        ///     Min enabled for some columns
        /// </summary>
        public bool IsMinEnabled { get { return Columns.Any(r => ((ITotalsColumn)r).IsMinEnabled); } }

        /// <summary>
        ///     Calculation enabled for some columns
        /// </summary>
        public bool IsCalculationEnabled { get { return Columns.Any(r => ((ITotalsColumn)r).IsCalculationEnabled); } }

        /// <summary>
        ///     Manage pager properties
        /// </summary>
        public IGridPager Pager
        {
            get { return _pager; }
            set { _pager = value; }
        }

        /// <summary>
        ///     Keys for subgrid
        /// </summary>
        public (string, string)[] SubGridKeys { get; set; }

        /// <summary>
        ///     Subgrids
        /// </summary>
        public Func<object[], Task<ICGrid>> SubGrids { get; set;  }

        /// <summary>
        ///     Subgrids state
        /// </summary>
        public bool SubGridsOpened { get; set; } = false;

        public Type Type { get { return typeof(T); } }

        /// <summary>
        ///     Get foreign key values for subgrid records
        /// </summary>
        public QueryDictionary<object> GetSubGridKeyValues(object item)
        {
            QueryDictionary<object> values = new QueryDictionary<object>();
            foreach (var key in SubGridKeys)
            {
                var value = item.GetType().GetProperty(key.Item1).GetValue(item);
                values.Add(key.Item2, value);
            }
            return values;
        }

        /// <summary>
        ///     Get primary key values for CRUD
        /// </summary>
        public object[] GetPrimaryKeyValues(object item)
        {
            List<object> values = new List<object>();
            foreach (var column in Columns)
            {
                if (column.IsPrimaryKey)
                {
                    var value = item.GetType().GetProperty(column.FieldName).GetValue(item);
                    values.Add(value);
                }
            }
            return values.ToArray();
        }

        /// <summary>
        ///     Get primary keys for CRUD
        /// </summary>
        public string[] GetPrimaryKeys()
        {
            List<string> values = new List<string>();
            foreach (var column in Columns)
            {
                if (column.IsPrimaryKey)
                {
                    values.Add(column.FieldName);
                }
            }
            return values.ToArray();
        }

        public bool DataAnnotationsValidation { get; set; } = true;

        private static readonly Task<bool> InsertColumnSucceded = Task.FromResult(true);
        private static readonly Task<bool> InsertColumnFailed = Task.FromResult(false);
        /// <inheritdoc/>
        public Task<bool> InsertColumn(IGridColumn targetColumn, IGridColumn insertingColumn)
        {

            var currentPossition = _columnsCollection.IndexOf(insertingColumn);
            var targetPossition = _columnsCollection.IndexOf(targetColumn);
            if (currentPossition == -1 || targetPossition == -1 || currentPossition == targetPossition)
                return InsertColumnFailed;
            
            var index = currentPossition > targetPossition ? targetPossition : targetPossition - 1;
            var removed = _columnsCollection.Remove(insertingColumn);
            if (!removed)
                return InsertColumnFailed;

            _columnsCollection.Insert(index, insertingColumn);
            return InsertColumnSucceded;
        }

        /// <summary>
        ///     Fixed column values for the grid
        /// </summary>
        public QueryDictionary<object> FixedValues { get; set; } = null;

        /// <summary>
        ///     Function to init values for columns in the Create form
        /// </summary>
        public Func<T, Task> InitCreateValues { get; set; } = null;

        /// <summary>
        ///     Fixed column values for the OData url expand parameter
        /// </summary>
        public IEnumerable<string> ODataExpandList { get; set; }

        /// <summary>
        ///     Override OData url expand parameter with list
        /// </summary>
        public bool ODataOverrideExpandList { get; set; } = false;

        /// <summary>
        ///    Add code to the end of OnAfterRenderAsync method of the component
        /// </summary>
        public Func<GridComponent<T>, bool, Task> OnAfterRender { get; set; }

        /// <summary>
        ///     Applies data annotations settings
        /// </summary>
        private void ApplyGridSettings()
        {
            GridTableAttribute opt = _annotations.GetAnnotationForTable<T>();
            if (opt == null) return;
            PagingType = opt.PagingType;

            if (PagingType == PagingType.Pagination)
            {
                if (opt.PageSize > 0)
                    Pager.PageSize = opt.PageSize;

                if (opt.PagingMaxDisplayedPages > 0 && Pager is GridPager)
                {
                    (Pager as GridPager).MaxDisplayedPages = opt.PagingMaxDisplayedPages;
                }
            }
        }

        /// <summary>
        ///     Generates columns for all properties of the model
        /// </summary>
        public virtual void AutoGenerateColumns()
        {
            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);

            // if any property has a position attribute it's necessary 
            // to create a new array before adding columns to the collection
            if (properties.SelectMany(r => r.CustomAttributes).SelectMany(r => r.NamedArguments).Any(r => r.MemberName == "Position"))
            {
                PropertyInfo[] newProperties = new PropertyInfo[properties.Length];
                foreach (PropertyInfo pi in properties)
                {
                    int? position = null;
                    if (pi.CustomAttributes.Count() > 0)
                    {
                        foreach (var a in pi.CustomAttributes)
                        {
                            if (a.NamedArguments.Any(r => r.MemberName == "Position"))
                            {
                                position = (int)a.NamedArguments.First(r => r.MemberName == "Position").TypedValue.Value;
                            }
                        }
                    }
                    if (position.HasValue)
                        newProperties[position.Value] = pi;
                }
                properties = newProperties;
            }

            foreach (PropertyInfo pi in properties)
            {
                bool isKey = false;
                if (pi.CustomAttributes.Count() > 0)
                {
                    foreach (var a in pi.CustomAttributes)
                    {
                        if (a.AttributeType.Name.Equals("KeyAttribute"))
                        {
                            isKey = true;
                        }
                    }
                }

                if (pi.CanRead)
                {
                    if (isKey)
                    {
                        Columns.Add(pi).SetPrimaryKey(true);
                    }
                    else
                    {
                        Columns.Add(pi);
                    }
                }
            }
        }

        /// <summary>
        ///     Text in empty grid (no items for display)
        /// </summary>
        public string EmptyGridText { get; set; }

        /// <summary>
        ///     Create button label
        /// </summary>
        public string CreateLabel { get; set; }

        /// <summary>
        ///     Read button label
        /// </summary>
        public string ReadLabel { get; set; }

        /// <summary>
        ///     Update button label
        /// </summary>
        public string UpdateLabel { get; set; }

        /// <summary>
        ///     Delete button label
        /// </summary>
        public string DeleteLabel { get; set; }

        /// <summary>
        ///     Create button tooltip
        /// </summary>
        public string CreateTooltip { get; set; } = Strings.CreateItem;

        /// <summary>
        ///     Read button tooltip
        /// </summary>
        public string ReadTooltip { get; set; } = Strings.ReadItem;

        /// <summary>
        ///     Update button tooltip
        /// </summary>
        public string UpdateTooltip { get; set; } = Strings.UpdateItem;

        /// <summary>
        ///     Delete button tooltip
        /// </summary>
        public string DeleteTooltip { get; set; } = Strings.DeleteItem;

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

        /// <summary>
        ///     Create form button label
        /// </summary>
        public string CreateFormButtonLabel { get; set; }

        /// <summary>
        ///     Update form button label
        /// </summary>
        public string UpdateFormButtonLabel { get; set; }

        /// <summary>
        ///     Delete form button label
        /// </summary>
        public string DeleteFormButtonLabel { get; set; }

        // <summary>
        ///     Create CRUD confirmation fields
        /// </summary>
        public bool CreateConfirmation { get; set; } = false;

        public int CreateConfirmationWidth { get; set; } = 5;

        public int CreateConfirmationLabelWidth { get; set; } = 2;

        /// <summary>
        ///     Update CRUD confirmation fields
        /// </summary>
        public bool UpdateConfirmation { get; set; } = false;

        public int UpdateConfirmationWidth { get; set; } = 5;

        public int UpdateConfirmationLabelWidth { get; set; } = 2;

        /// <summary>
        ///     Delete CRUD confirmation fields
        /// </summary>
        public bool DeleteConfirmation { get; set; } = false;

        public int DeleteConfirmationWidth { get; set; } = 5;

        public int DeleteConfirmationLabelWidth { get; set; } = 2;

        public bool HeaderCrudButtons { get; set; }

        public bool ShowErrorsOnGrid { get; set; } = false;

        public bool ThrowExceptions { get; set; } = false;

        public string Error { get; set; } = "";

        public bool EditAfterInsert { get; set; } = false;

        public CssFramework CssFramework { get; set; }

        public HtmlClass HtmlClass { get; set; }

        #region Custom row css classes
        public void SetRowCssClassesContraint(Func<T, string> contraint)
        {
            _rowCssClassesContraint = contraint;
        }

        public string GetRowCssClasses(object item)
        {
            if (_rowCssClassesContraint == null)
                return string.Empty;
            var typed = (T)item;
            if (typed == null)
                throw new InvalidCastException(string.Format("The item must be of type '{0}'", typeof(T).FullName));
            return _rowCssClassesContraint(typed);
        }

        #endregion

        /// <summary>
        ///     Provides query, using by the grid
        /// </summary>
        public void AddQueryParameter(string parameterName, StringValues parameterValue)
        {
            if (string.IsNullOrEmpty(parameterName))
                throw new ArgumentException("parameterName");
            if (parameterName.Equals(QueryStringFilterSettings.DefaultTypeQueryParameter))
                throw new ArgumentException("parameterName cannot be " + QueryStringFilterSettings.DefaultTypeQueryParameter);

            if (_query.ContainsKey(parameterName))
                _query[parameterName] = parameterValue;
            else
                _query.Add(parameterName, parameterValue);

            UpdateQueryAndSettings();
        }

        public void RemoveQueryParameter(string parameterName)
        {
            if (string.IsNullOrEmpty(parameterName))
                throw new ArgumentException("parameterName");

            if (_query.ContainsKey(parameterName))
            {
                _query.Remove(parameterName);
                UpdateQueryAndSettings();
            }
        }

        public void AddQueryString(string parameterName, string parameterValue)
        {
            if (string.IsNullOrEmpty(parameterName))
                throw new ArgumentException("parameterName");
            if (parameterName.Equals(QueryStringFilterSettings.DefaultTypeQueryParameter))
                throw new ArgumentException("parameterName cannot be " + QueryStringFilterSettings.DefaultTypeQueryParameter);

            if (_query.ContainsKey(parameterName))
            {
                var parameterValues = _query[parameterName].ToList();
                parameterValues.Add(parameterValue);
                _query[parameterName] = new StringValues(parameterValues.ToArray());
            }
            else
                _query.Add(parameterName, parameterValue);

            UpdateQueryAndSettings();
        }

        public void ChangeQueryString(string parameterName, string oldParameterValue, string newParameterValue)
        {
            if (string.IsNullOrEmpty(parameterName))
                throw new ArgumentException("parameterName");
            if (parameterName.Equals(QueryStringFilterSettings.DefaultTypeQueryParameter))
                throw new ArgumentException("parameterName cannot be " + QueryStringFilterSettings.DefaultTypeQueryParameter);

            if (_query.ContainsKey(parameterName))
            {
                var parameterValues = _query[parameterName].ToList();
                parameterValues.Remove(oldParameterValue);
                parameterValues.Add(newParameterValue);
                _query[parameterName] = new StringValues(parameterValues.ToArray());

                UpdateQueryAndSettings();
            }
        }

        public void RemoveQueryString(string parameterName, string parameterValue)
        {
            if (string.IsNullOrEmpty(parameterName))
                throw new ArgumentException("parameterName");

            if (_query.ContainsKey(parameterName))
            {
                var parameterValues = _query[parameterName].ToList();
                parameterValues.Remove(parameterValue);
                _query[parameterName] = new StringValues(parameterValues.ToArray());

                UpdateQueryAndSettings();
            }             
        }

        public void AddFilterParameter(IGridColumn column, FilterCollection filters)
        {
            var columnFilters = _query.Get(QueryStringFilterSettings.DefaultTypeQueryParameter).ToArray();
            if (ComponentOptions.AllowMultipleFilters)
            {
                if (columnFilters == null)
                {
                    foreach (var filter in filters)
                    {
                        _query.Add(QueryStringFilterSettings.DefaultTypeQueryParameter,
                            column.Name + QueryStringFilterSettings.FilterDataDelimeter +
                            filter.Type + QueryStringFilterSettings.FilterDataDelimeter +
                            filter.Value);
                    }
                }
                else
                {
                    var newFilters = columnFilters.Where(r => !r.ToLower().StartsWith(column.Name.ToLower()
                        + QueryStringFilterSettings.FilterDataDelimeter)).ToList();
                    foreach (var filter in filters)
                    {
                        newFilters.Add(column.Name + QueryStringFilterSettings.FilterDataDelimeter +
                            filter.Type + QueryStringFilterSettings.FilterDataDelimeter + filter.Value);
                    }
                    _query[QueryStringFilterSettings.DefaultTypeQueryParameter] =
                        new StringValues(newFilters.ToArray());
                }
            }
            else
            {
                RemoveQueryParameter(QueryStringFilterSettings.DefaultTypeQueryParameter);

                if (filters.Count() > 0)
                {
                    _query.Add(QueryStringFilterSettings.DefaultTypeQueryParameter,
                        column.Name + QueryStringFilterSettings.FilterDataDelimeter +
                        filters.First().Type + QueryStringFilterSettings.FilterDataDelimeter +
                        filters.First().Value);
                }
            }

            AddClearInitFilters(column);

            UpdateQueryAndSettings();
        }

        private void AddClearInitFilters(IGridColumn column)
        {
            if (ComponentOptions.AllowMultipleFilters)
            {
                if (column.InitialFilterSettings != ColumnFilterValue.Null)
                {
                    if (_query.ContainsKey(QueryStringFilterSettings.DefaultClearInitFilterQueryParameter))
                    {
                        StringValues clearInitFilters = _query[QueryStringFilterSettings.DefaultClearInitFilterQueryParameter];
                        if (!clearInitFilters.Contains(column.Name))
                        {
                            clearInitFilters = StringValues.Concat(clearInitFilters, column.Name);
                            _query[QueryStringFilterSettings.DefaultClearInitFilterQueryParameter] = clearInitFilters;
                        }
                    }
                    else
                        _query.Add(QueryStringFilterSettings.DefaultClearInitFilterQueryParameter, column.Name);
                }
            }
            else
            {
                StringValues clearInitFilters = new StringValues();

                var columnsToAdd = Columns.Where(r => r.InitialFilterSettings != ColumnFilterValue.Null); 
                foreach (var columnToAdd in columnsToAdd)
                {
                    clearInitFilters = StringValues.Concat(clearInitFilters, columnToAdd.Name);
                }

                if (_query.ContainsKey(QueryStringFilterSettings.DefaultClearInitFilterQueryParameter))
                    _query[QueryStringFilterSettings.DefaultClearInitFilterQueryParameter] = clearInitFilters;
                else
                    _query.Add(QueryStringFilterSettings.DefaultClearInitFilterQueryParameter, clearInitFilters);
            }       
        }

        public void RemoveFilterParameter(IGridColumn column)
        {
            var filters = _query.Get(QueryStringFilterSettings.DefaultTypeQueryParameter).ToArray();
            if (filters != null)
            {
                var newFilters = filters.Where(r => !r.ToLower().StartsWith(column.Name.ToLower()
                    + QueryStringFilterSettings.FilterDataDelimeter));
                _query[QueryStringFilterSettings.DefaultTypeQueryParameter] =
                    new StringValues(newFilters.ToArray());
            }

            AddClearInitFilters(column);

            UpdateQueryAndSettings();
        }

        public void RemoveAllFilters()
        {
            RemoveQueryParameter(QueryStringFilterSettings.DefaultClearInitFilterQueryParameter);
            RemoveQueryParameter(QueryStringFilterSettings.DefaultTypeQueryParameter);
        }

        public string GetState()
        {
            var jsonOptions = new JsonSerializerOptions();
            jsonOptions.Converters.Add(new StringValuesConverter());
            string query = JsonSerializer.Serialize(Query, jsonOptions);
            return query.GridStateEncode();
        }

        public string GetLink()
        {
            return ((GridPager)_pager).GetLink(); ;
        }

        public IList<object> GetValuesToDisplay(string columnName, IEnumerable<object> items)
        {
            var column = Columns.SingleOrDefault(r => r.Name == columnName);
            if (column == null)
                return new List<object>();
            return ((IGridColumn<T>)column).Group.GetColumnValues((items as IEnumerable<T>).AsQueryable()).ToList();
        }

        public IList<object> GetGroupValues(IColumnGroup<T> group, IEnumerable<object> items)
        {
            if(group == null)
                return new List<object>();
            return group.GetColumnValues((items as IEnumerable<T>).AsQueryable()).ToList();
        }

        public IColumnGroup<T> GetGroup(string columnName)
        {
            var column = Columns.SingleOrDefault(r => r.Name == columnName);
            if (column == null)
                return null;
            return ((IGridColumn<T>)column).Group;
        }

        public IEnumerable<object> GetItemsToDisplay(IList<Tuple<string, object>> values, IEnumerable<object> items)
        {
            if (values.Count == 0)
                return items;

            var itms = (items as IEnumerable<T>).AsQueryable();
            foreach (var value in values)
            {
                var column = Columns.SingleOrDefault(r => r.Name == value.Item1);
                if (column == null)
                    continue;
                itms = ((IGridColumn<T>)column).Group.ApplyFilter(itms, value.Item2);
            }
            return (IEnumerable<object>)itms;
        }

        public async Task DownloadExcel(IJSRuntime js, string filename)
        {
            if (ExcelExport)
            {
                ExcelWriter excelWriter = new ExcelWriter();
                byte[] content;
                if (ExcelExportAllRows)
                {
                    if (PagingType == PagingType.Virtualization)
                    {
                        var oldStartIndex = Pager.StartIndex;
                        var oldVirtualizedCount = Pager.VirtualizedCount;
                        ((GridPager)Pager).StartIndex = 0;
                        AddQueryParameter(GridPager.DefaultStartIndexQueryParameter, "0");
                        ((GridPager)Pager).VirtualizedCount = ItemsCount;
                        AddQueryParameter(GridPager.DefaultVirtualizedCountQueryParameter, ItemsCount.ToString());
                        await UpdateGrid();
                        content = excelWriter.GenerateExcel(Columns, Items);
                        ((GridPager)Pager).StartIndex = oldStartIndex;
                        AddQueryParameter(GridPager.DefaultStartIndexQueryParameter, oldStartIndex.ToString());
                        ((GridPager)Pager).VirtualizedCount = oldVirtualizedCount;
                        AddQueryParameter(GridPager.DefaultVirtualizedCountQueryParameter, oldVirtualizedCount.ToString());
                        await UpdateGrid();
                    }
                    else
                    {
                        var oldPageSize = Pager.PageSize;
                        Pager.PageSize = ItemsCount;
                        AddQueryParameter(GridPager.DefaultPageSizeQueryParameter, ItemsCount.ToString());
                        await UpdateGrid();
                        content = excelWriter.GenerateExcel(Columns, Items);
                        Pager.PageSize = oldPageSize;
                        AddQueryParameter(GridPager.DefaultPageSizeQueryParameter, oldPageSize.ToString());
                        await UpdateGrid();
                    }
                }
                else
                    content = excelWriter.GenerateExcel(Columns, Items);
                await js.InvokeAsync<object>("gridJsFunctions.saveAsFile", filename, Convert.ToBase64String(content));
            }
        }

        public async Task UpdateGrid()
        {
            Error = "";

            if (PagingType == PagingType.Virtualization)
            {
                // Set NoTotals to false because the function call comes from a change on the grid component
                _noTotals = false;
                AddQueryParameter(GridPager.DefaultNoTotalsParameter, _noTotals.ToString());

                // The call to the back-end in not performed here for virtualizated grids
                // LoadItems method is in charge to make this call
            }
            else
            {
                if (ServerAPI == ServerAPI.OData && (GridComponent == null || !GridComponent.UseMemoryCrudDataService))
                    await GetOData();
                else
                    await GetItemsDTO();
            }
        }

        private async Task GetItemsDTO()
        {
            try
            {
                ItemsDTO<T> response;
                if (_dataServiceAsync != null)
                {
                    response = await _dataServiceAsync(_query);
                }
                else if (_dataService != null)
                {
                    response = _dataService(_query);
                }
                else if (_grpcService != null)
                {
                    response = await _grpcService(_query.ToStringDictionary());
                    if (response.Items == null && response.Pager.ItemsCount == 0)
                        response.Items = new List<T>();
                }
                else
                {
                    string urlParameters = GetLink();
                    if (Url.Contains("?"))
                        urlParameters = urlParameters.Replace("?", "&");
                    response = await HttpClient.GetFromJsonAsync<ItemsDTO<T>>(Url + urlParameters);       
                }
                if (response != null && response.Items != null && response.Pager != null)
                {
                    Items = response.Items;
                    if (PagingType != PagingType.Virtualization)
                    {
                        PagingType = response.Pager.PagingType;
                        ((GridPager)_pager).CurrentPage = response.Pager.CurrentPage;
                        AddQueryParameter(((GridPager)Pager).ParameterName, response.Pager.CurrentPage.ToString());
                        ((GridPager)_pager).PageSize = response.Pager.PageSize;
                    }
                    ((GridPager)_pager).ItemsCount = response.Pager.ItemsCount;

                    if (response.Totals != null)
                    {
                        if (response.Totals.Sum != null)
                            foreach (var keyValue in response.Totals.Sum)
                            {
                                var column = (ITotalsColumn)Columns.SingleOrDefault(r => r.Name != null && r.Name.Equals(keyValue.Key, StringComparison.CurrentCultureIgnoreCase));
                                if (column != null && column.IsSumEnabled)
                                    column.SumValue = keyValue.Value;
                            }

                        if (response.Totals.Average != null)
                            foreach (var keyValue in response.Totals.Average)
                            {
                                var column = (ITotalsColumn)Columns.SingleOrDefault(r => r.Name != null && r.Name.Equals(keyValue.Key, StringComparison.CurrentCultureIgnoreCase));
                                if (column != null && column.IsAverageEnabled)
                                    column.AverageValue = keyValue.Value;
                            }

                        if (response.Totals.Max != null)
                            foreach (var keyValue in response.Totals.Max)
                            {
                                var column = (ITotalsColumn)Columns.SingleOrDefault(r => r.Name != null && r.Name.Equals(keyValue.Key, StringComparison.CurrentCultureIgnoreCase));
                                if (column != null && column.IsMaxEnabled)
                                    column.MaxValue = keyValue.Value;
                            }

                        if (response.Totals.Min != null)
                            foreach (var keyValue in response.Totals.Min)
                            {
                                var column = (ITotalsColumn)Columns.SingleOrDefault(r => r.Name != null && r.Name.Equals(keyValue.Key, StringComparison.CurrentCultureIgnoreCase));
                                if (column != null && column.IsMinEnabled)
                                    column.MinValue = keyValue.Value;
                            }

                        if (response.Totals.Calculations != null)
                            foreach (var keyValue in response.Totals.Calculations)
                            {
                                var column = (ITotalsColumn)Columns.SingleOrDefault(r => r.Name != null && r.Name.Equals(keyValue.Key, StringComparison.CurrentCultureIgnoreCase));
                                if (column != null && column.IsCalculationEnabled)
                                    column.CalculationValues = keyValue.Value;
                            }
                    }          
                }
                else
                    Console.WriteLine("Response is null");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                if (ShowErrorsOnGrid)
                    Error = e.Message;

                if (ThrowExceptions)
                    throw;
            }
        }

        public string GetODataExpandParameters()
        {
            return _currentExpandODataProcessor.Process();
        }

        public string GetODataFilterParameters()
        {
            return _currentFilterODataProcessor.Process();
        }

        public string GetODataPagerParameters()
        {
            return _currentPagerODataProcessor.Process();
        }

        public string GetODataSortParameters()
        {
            return _currentSortODataProcessor.Process();
        }

        public string GetODataPreProcessorParameters()
        {
            // Preprocessor (filter and expand)
            string preProcessorParameters = GetODataExpandParameters();

            if (string.IsNullOrWhiteSpace(preProcessorParameters))
                preProcessorParameters = GetODataFilterParameters();
            else
            {
                string filterParameters = GetODataFilterParameters();
                if (!string.IsNullOrWhiteSpace(filterParameters))
                    preProcessorParameters += "&" + filterParameters;
            }

            // $search is not supported by OData WebApi
            /**
            string searchParameters = _currentSearchODataProcessor.Process();
            if (!string.IsNullOrWhiteSpace(searchParameters))
                preProcessorParameters += "&" + searchParameters;
            */

            return preProcessorParameters;
        }

        public string GetODataProcessorParameters()
        {
            // Processor parameters (paging and sorting)
            string processorParameters = "";
            if (string.IsNullOrWhiteSpace(processorParameters))
                processorParameters = GetODataPagerParameters();
            else
            {
                string pagerParameters = GetODataPagerParameters();
                if (!string.IsNullOrWhiteSpace(pagerParameters))
                    processorParameters += "&" + pagerParameters;
            }

            if (string.IsNullOrWhiteSpace(processorParameters))
                processorParameters = GetODataSortParameters();
            else
            {
                string sortParameters = GetODataSortParameters();
                if (!string.IsNullOrWhiteSpace(sortParameters))
                    processorParameters += "&" + sortParameters;
            }

            return processorParameters;
        }

        private async Task GetOData()
        {
            var jsonOptions = new JsonSerializerOptions().AddOdataSupport();
            try
            {
                // Preprocessor (filter and expand)
                string preProcessorParameters = GetODataPreProcessorParameters();

                //  get count of preprocessed items
                string allParameters;
                if (string.IsNullOrWhiteSpace(preProcessorParameters))
                    allParameters = "$count=true&$top=0&$skip=0";
                else
                    allParameters = preProcessorParameters + "&$count=true&$top=0&$skip=0";

                if (Url.Contains("?"))
                    allParameters = "&" + allParameters;
                else
                    allParameters = "?" + allParameters;
                ODataDTO<T> response = await HttpClient.GetFromJsonAsync<ODataDTO<T>>(Url + allParameters, jsonOptions);
                if (response == null)
                {
                    Console.WriteLine("Response is null");
                    return;
                }
                int itemsCount = response.ItemsCount;
                ((GridPager)_pager).ItemsCount = itemsCount;

                // Processor parameters (paging and sorting)
                string processorParameters = GetODataProcessorParameters();

                // All parameters
                allParameters = preProcessorParameters + "&" + processorParameters;
                allParameters = allParameters.TrimStart('&').TrimEnd('&');

                if (Url.Contains("?"))
                    allParameters = "&" + allParameters;
                else
                    allParameters = "?" + allParameters;

                //  get processed items
                response = await HttpClient.GetFromJsonAsync<ODataDTO<T>>(Url + allParameters, jsonOptions);
                if (response == null ||  response.Value == null)
                {
                    Console.WriteLine("Response is null");
                    return;
                }

                Items = response.Value;
                ((GridPager)_pager).ItemsCount = itemsCount;

                // total back-end queries must not be requested if _query contains a NoTotals parameter with a true value
                bool noTotals = false;
                if (_query.ContainsKey(GridPager.DefaultNoTotalsParameter))
                {
                    string noTotalsStr = _query[GridPager.DefaultNoTotalsParameter].FirstOrDefault();
                    bool.TryParse(noTotalsStr, out noTotals);
                }
                
                if (noTotals == false)
                {
                    foreach (IGridColumn<T> column in Columns)
                    {
                        if (column.IsSumEnabled || column.IsAverageEnabled || column.IsMaxEnabled || column.IsMinEnabled)
                        {
                            bool isNullable = column.Totals.IsNullable();
                            Type type = column.Totals.GetPropertyType(isNullable);

                            string filterParameters = GetODataFilterParameters();
                            if (string.IsNullOrWhiteSpace(filterParameters))
                                allParameters = "?$apply=";
                            else
                            {
                                filterParameters = filterParameters.Remove(0, 8);
                                allParameters = $"?$apply=filter({filterParameters})/";
                            }

                            var aggregates = new List<string>();
                            if (column.IsSumEnabled &&
                                (type == typeof(Single) || type == typeof(Int32) || type == typeof(Int64) || type == typeof(Double) || type == typeof(Decimal)))
                            {
                                aggregates.Add(column.Totals.GetFullName() + " with sum as Sum");
                            }
                            if (column.IsAverageEnabled &&
                                (type == typeof(Single) || type == typeof(Int32) || type == typeof(Int64) || type == typeof(Double) || type == typeof(Decimal)))
                            {
                                aggregates.Add(column.Totals.GetFullName() + " with average as Average");
                            }
                            if (column.IsMaxEnabled)
                            {
                                aggregates.Add(column.Totals.GetFullName() + " with max as Max");
                            }
                            if (column.IsMinEnabled)
                            {
                                aggregates.Add(column.Totals.GetFullName() + " with min as Min");
                            }
                            allParameters += $"aggregate({string.Join(",", aggregates)})";


                            if (type == typeof(Single) || type == typeof(Int32) || type == typeof(Int64) || type == typeof(Double) || type == typeof(Decimal))
                            {
                                var totalResponse = await HttpClient.GetFromJsonAsync<List<NumberTotals>>(Url + allParameters, jsonOptions);
                                if (totalResponse == null || totalResponse.Count > 0)
                                {
                                    column.SumValue = new Total(totalResponse.First().Sum);
                                    column.AverageValue = new Total(totalResponse.First().Average);
                                    column.MaxValue = new Total(totalResponse.First().Max);
                                    column.MinValue = new Total(totalResponse.First().Min);
                                }
                            }
                            else if (type == typeof(DateTime))
                            {
                                var totalResponse = await HttpClient.GetFromJsonAsync<List<DateTimeTotals>>(Url + allParameters, jsonOptions);
                                if (totalResponse == null || totalResponse.Count > 0)
                                {
                                    column.MaxValue = new Total(totalResponse.First().Max);
                                    column.MinValue = new Total(totalResponse.First().Min);
                                }
                            }
                            else if (type == typeof(string))
                            {
                                var totalResponse = await HttpClient.GetFromJsonAsync<List<StringTotals>>(Url + allParameters, jsonOptions);
                                if (totalResponse == null || totalResponse.Count > 0)
                                {
                                    column.MaxValue = new Total(totalResponse.First().Max);
                                    column.MinValue = new Total(totalResponse.First().Min);
                                }
                            }
                        }
                    }

                    foreach (IGridColumn<T> gridColumn in Columns.Where(r => ((IGridColumn<T>)r).Calculations.Any()))
                    {
                        foreach (var calculation in gridColumn.Calculations)
                        {
                            var value = calculation.Value(Columns);
                            Type type = value.GetType();

                            if (type == typeof(Single) || type == typeof(Int32) || type == typeof(Int64) || type == typeof(Double) || type == typeof(Decimal))
                            {
                                gridColumn.CalculationValues.AddParameter(calculation.Key, new Total((decimal?)value));
                            }
                            else if (type == typeof(DateTime))
                            {
                                gridColumn.CalculationValues.AddParameter(calculation.Key, new Total((DateTime)value));
                            }
                            else if (type == typeof(string))
                            {
                                gridColumn.CalculationValues.AddParameter(calculation.Key, new Total((string)value));
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);

                if (ShowErrorsOnGrid)
                    Error = e.Message;

                if (ThrowExceptions)
                    throw;
            }
        }

#if !NETSTANDARD2_1
        public async ValueTask<ItemsProviderResult<T>> LoadItems(ItemsProviderRequest request)
        {
            Error = "";

            AddQueryParameter(GridPager.DefaultNoTotalsParameter, _noTotals.ToString());
            // Set NoTotals to true for next calls
            // Only a change on the grid component will reset the value to false (e.g. a filter call)
            _noTotals = true;

            if (request.StartIndex < 0 || request.Count <= 0)
                return new ItemsProviderResult<T>(Items, ((GridPager)_pager).ItemsCount);

            AddQueryParameter(GridPager.DefaultStartIndexQueryParameter, request.StartIndex.ToString());
            AddQueryParameter(GridPager.DefaultVirtualizedCountQueryParameter, request.Count.ToString());

            _pager = new GridPager(this);

            Error = "";

            if (ServerAPI == ServerAPI.OData && (GridComponent == null || !GridComponent.UseMemoryCrudDataService))
                await GetOData();
            else
                await GetItemsDTO();

            if (GridComponent != null && GridComponent.CountComponent != null) 
            {
                GridComponent.CountComponent.Refresh();
            }

            if (GridComponent != null && GridComponent.TotalsComponent != null)
            {
                GridComponent.TotalsComponent.Refresh();
            }

            return new ItemsProviderResult<T>(Items, ((GridPager)_pager).ItemsCount);
        }
#endif
    }
}
