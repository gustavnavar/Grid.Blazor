using GridBlazor.Columns;
using GridBlazor.DataAnnotations;
using GridBlazor.Filtering;
using GridBlazor.Pagination;
using GridBlazor.Resources;
using GridShared;
using GridShared.Columns;
using GridShared.DataAnnotations;
using GridShared.Filtering;
using GridShared.Utility;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text.Json;
using System.Threading.Tasks;

namespace GridBlazor
{
    /// <summary>
    ///     Grid.Mvc base class
    /// </summary>
    public class CGrid<T> : ICGrid
    {
        private Func<T, string> _rowCssClassesContraint;

        private QueryDictionary<StringValues> _query;
        private IGridSettingsProvider _settings;
        private readonly IGridAnnotationsProvider _annotations;
        private readonly IColumnBuilder<T> _columnBuilder;
        private readonly GridColumnCollection<T> _columnsCollection;
        private IEnumerable<T> _items;
        private IEnumerable<object> _selectedItems;
        private int _displayingItemsCount = -1; // count of displaying items (if using pagination)
        private bool _enablePaging;
        private IGridPager _pager;
        private HttpClient _httpClient;

        private readonly Func<QueryDictionary<StringValues>, ItemsDTO<T>> _dataService;
        private readonly Func<QueryDictionary<StringValues>, Task<ItemsDTO<T>>> _dataServiceAsync;
        private ICrudDataService<T> _crudDataService;

        public CGrid(HttpClient httpClient, string url, IQueryDictionary<StringValues> query, bool renderOnlyRows,
            Action<IGridColumnCollection<T>> columns = null, CultureInfo cultureInfo = null)
        {
            _dataService = null;
            _dataServiceAsync = null;

            Items = new List<T>();
            _selectedItems = new List<object>();
            _httpClient = httpClient;
            Url = url;
            _query = query as QueryDictionary<StringValues>;

            //set up sort settings:
            _settings = new QueryStringGridSettingsProvider(_query);
            Sanitizer = new Sanitizer();
            if (cultureInfo != null)
                Strings.Culture = cultureInfo;
            EmptyGridText = Strings.DefaultGridEmptyText;
            Language = Strings.Lang;

            _annotations = new GridAnnotationsProvider();

            //Set up column collection:
            _columnBuilder = new DefaultColumnBuilder<T>(this, _annotations);
            _columnsCollection = new GridColumnCollection<T>(this, _columnBuilder, _settings.SortSettings);
            ComponentOptions = new GridOptions();

            ApplyGridSettings();

            Pager = new GridPager(query);

            ComponentOptions.RenderRowsOnly = renderOnlyRows;
            columns?.Invoke(Columns);

            Mode = GridMode.Grid;
            CreateEnabled = false;
            ReadEnabled = false;
            UpdateEnabled = false;
            DeleteEnabled = false;
        }

        [Obsolete("This constructor is obsolete. Use one including an HttpClient parameter.", false)]
        public CGrid(string url, IQueryDictionary<StringValues> query, bool renderOnlyRows,
            Action<IGridColumnCollection<T>> columns = null, CultureInfo cultureInfo = null)
        {
            _dataService = null;
            _dataServiceAsync = null; 

            Items = new List<T>();
            _selectedItems = new List<object>();

            _httpClient = null;
            Url = url;
            _query = query as QueryDictionary<StringValues>;

            //set up sort settings:
            _settings = new QueryStringGridSettingsProvider(_query);
            Sanitizer = new Sanitizer();
            if (cultureInfo != null)
                Strings.Culture = cultureInfo;
            EmptyGridText = Strings.DefaultGridEmptyText;
            Language = Strings.Lang;

            _annotations = new GridAnnotationsProvider();

            //Set up column collection:
            _columnBuilder = new DefaultColumnBuilder<T>(this, _annotations);
            _columnsCollection = new GridColumnCollection<T>(this, _columnBuilder, _settings.SortSettings);
            ComponentOptions = new GridOptions();

            ApplyGridSettings();

            Pager = new GridPager(query);

            ComponentOptions.RenderRowsOnly = renderOnlyRows;
            columns?.Invoke(Columns);

            Mode = GridMode.Grid;
            CreateEnabled = false;
            ReadEnabled = false;
            UpdateEnabled = false;
            DeleteEnabled = false;
        }

        public CGrid(Func<QueryDictionary<StringValues>, ItemsDTO<T>> dataService,
            QueryDictionary<StringValues> query, bool renderOnlyRows,
            Action<IGridColumnCollection<T>> columns = null, CultureInfo cultureInfo = null)
        {
            _dataService = dataService;
            _dataServiceAsync = null;
            _selectedItems = new List<object>();

            Items = new List<T>(); //response.Items;

            Url = null;
            _httpClient = null;
            _query = query;

            //set up sort settings:
            _settings = new QueryStringGridSettingsProvider(_query);
            Sanitizer = new Sanitizer();
            if (cultureInfo != null)
                Strings.Culture = cultureInfo;
            EmptyGridText = Strings.DefaultGridEmptyText;
            Language = Strings.Lang;

            _annotations = new GridAnnotationsProvider();

            //Set up column collection:
            _columnBuilder = new DefaultColumnBuilder<T>(this, _annotations);
            _columnsCollection = new GridColumnCollection<T>(this, _columnBuilder, _settings.SortSettings);
            ComponentOptions = new GridOptions();

            ApplyGridSettings();

            Pager = new GridPager(query);

            ComponentOptions.RenderRowsOnly = renderOnlyRows;
            columns?.Invoke(Columns);

            Mode = GridMode.Grid;
            CreateEnabled = false;
            ReadEnabled = false;
            UpdateEnabled = false;
            DeleteEnabled = false;
        }

        public CGrid(Func<QueryDictionary<StringValues>, Task<ItemsDTO<T>>> dataServiceAsync,
            QueryDictionary<StringValues> query, bool renderOnlyRows,
            Action<IGridColumnCollection<T>> columns = null, CultureInfo cultureInfo = null)
        {
            _dataServiceAsync = dataServiceAsync;
            _dataService = null;
            _selectedItems = new List<object>();
            Items = new List<T>(); //response.Items;

            Url = null;
            _httpClient = null;
            _query = query;

            //set up sort settings:
            _settings = new QueryStringGridSettingsProvider(_query);
            Sanitizer = new Sanitizer();
            if (cultureInfo != null)
                Strings.Culture = cultureInfo;
            EmptyGridText = Strings.DefaultGridEmptyText;
            Language = Strings.Lang;

            _annotations = new GridAnnotationsProvider();

            //Set up column collection:
            _columnBuilder = new DefaultColumnBuilder<T>(this, _annotations);
            _columnsCollection = new GridColumnCollection<T>(this, _columnBuilder, _settings.SortSettings);
            ComponentOptions = new GridOptions();

            ApplyGridSettings();

            Pager = new GridPager(query);

            ComponentOptions.RenderRowsOnly = renderOnlyRows;
            columns?.Invoke(Columns);

            Mode = GridMode.Grid;
            CreateEnabled = false;
            ReadEnabled = false;
            UpdateEnabled = false;
            DeleteEnabled = false;
        }

        /// <summary>
        /// Total count of items in the grid
        /// </summary>
        public int ItemsCount { get { return _pager.ItemsCount; } }

        public bool SearchingEnabled { get; set; }

        public bool SearchingOnlyTextColumns { get; set; }

        public bool SearchingHiddenColumns { get; set; }

        public bool ExtSortingEnabled { get; set; }

        public bool GroupingEnabled { get; set; }

        public bool ClearFiltersButtonEnabled { get; set; } = false;

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

        internal IGridColumnCollection<T> Columns
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
            set
            {
                _query = value.ToQuery() as QueryDictionary<StringValues>;
                if (_pager.CurrentPage > 0)
                    _query.Add(((GridPager)_pager).ParameterName, _pager.CurrentPage.ToString());
                _settings = new QueryStringGridSettingsProvider(_query);
                _columnsCollection.SortSettings = _settings.SortSettings;
                ((GridPager)_pager).Query = _query;
            }
        }

        /// <summary>
        ///     Provides url used by the grid
        /// </summary>
        public string Url { get; internal set; }

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
        public Func<QueryDictionary<StringValues>, ItemsDTO<T>> DataService { get { return _dataService; } }

        public Func<QueryDictionary<StringValues>, Task<ItemsDTO<T>>> DataServiceAsync { get { return _dataServiceAsync; } }

        /// <summary>
        ///     Provides CrudDataService used by the grid
        /// </summary>
        public ICrudDataService<T> CrudDataService 
        { 
            get { return _crudDataService; }
            set { _crudDataService = value; }
        }

        /// <summary>
        ///     Provides query, using by the grid
        /// </summary>
        public QueryDictionary<StringValues> Query
        {
            get { return _query; }
            set
            {
                _query = value;
                _settings = new QueryStringGridSettingsProvider(_query);
                _columnsCollection.SortSettings = _settings.SortSettings;
                ((GridPager)_pager).Query = _query;
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
        public bool EnablePaging
        {
            get { return _enablePaging; }
            set
            {
                if (_enablePaging == value) return;
                _enablePaging = value;
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
        ///     Get value for creating items
        /// </summary>
        public bool CreateEnabled { get; internal set; }

        /// <summary>
        ///     Get value for reading items
        /// </summary>
        public bool ReadEnabled { get; internal set; }

        /// <summary>
        ///     Get value for updating items
        /// </summary>
        public bool UpdateEnabled { get; internal set; }

        /// <summary>
        ///     Get value for deleting items
        /// </summary>
        public bool DeleteEnabled { get; internal set; }

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

        public bool Keyboard { get; internal set; } = false;

        public ModifierKey ModifierKey { get; internal set; } = ModifierKey.CtrlKey;

        /// <summary>
        ///     Sum enabled for some columns
        /// </summary>
        public bool IsSumEnabled { get { return Columns.Any(r => ((ICGridColumn)r).IsSumEnabled); } }

        /// <summary>
        ///     Average enabled for some columns
        /// </summary>
        public bool IsAverageEnabled { get { return Columns.Any(r => ((ICGridColumn)r).IsAverageEnabled); } }

        /// <summary>
        ///     Max enabled for some columns
        /// </summary>
        public bool IsMaxEnabled { get { return Columns.Any(r => ((ICGridColumn)r).IsMaxEnabled); } }

        /// <summary>
        ///     Min enabled for some columns
        /// </summary>
        public bool IsMinEnabled { get { return Columns.Any(r => ((ICGridColumn)r).IsMinEnabled); } }

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
        ///     Fixed column values for the grid
        /// </summary>
        public QueryDictionary<object> FixedValues { get; set; } = null;

        /// <summary>
        ///     Applies data annotations settings
        /// </summary>
        private void ApplyGridSettings()
        {
            GridTableAttribute opt = _annotations.GetAnnotationForTable<T>();
            if (opt == null) return;
            EnablePaging = opt.PagingEnabled;
            if (opt.PageSize > 0)
                Pager.PageSize = opt.PageSize;

            if (opt.PagingMaxDisplayedPages > 0 && Pager is GridPager)
            {
                (Pager as GridPager).MaxDisplayedPages = opt.PagingMaxDisplayedPages;
            }
        }

        /// <summary>
        ///     Generates columns for all properties of the model
        /// </summary>
        public virtual void AutoGenerateColumns()
        {
            //TODO add support order property
            PropertyInfo[] properties = typeof(T).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            foreach (PropertyInfo pi in properties)
            {
                if (pi.CanRead)
                    ((IGridColumnCollection<T>)Columns).Add(pi);
            }
        }

        /// <summary>
        ///     Text in empty grid (no items for display)
        /// </summary>
        public string EmptyGridText { get; set; }

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

            _settings = new QueryStringGridSettingsProvider(_query);
            _columnsCollection.SortSettings = _settings.SortSettings;
            _columnsCollection.UpdateColumnsSorting();
            ((GridPager)_pager).Query = _query;
        }

        public void RemoveQueryParameter(string parameterName)
        {
            if (string.IsNullOrEmpty(parameterName))
                throw new ArgumentException("parameterName");

            if (_query.ContainsKey(parameterName))
            {
                _query.Remove(parameterName);

                _settings = new QueryStringGridSettingsProvider(_query);
                _columnsCollection.SortSettings = _settings.SortSettings;
                _columnsCollection.UpdateColumnsSorting();
                ((GridPager)_pager).Query = _query;
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

            _settings = new QueryStringGridSettingsProvider(_query);
            _columnsCollection.SortSettings = _settings.SortSettings;
            _columnsCollection.UpdateColumnsSorting();
            ((GridPager)_pager).Query = _query;
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

                _settings = new QueryStringGridSettingsProvider(_query);
                _columnsCollection.SortSettings = _settings.SortSettings;
                _columnsCollection.UpdateColumnsSorting();
                ((GridPager)_pager).Query = _query;
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

                _settings = new QueryStringGridSettingsProvider(_query);
                _columnsCollection.SortSettings = _settings.SortSettings;
                _columnsCollection.UpdateColumnsSorting();
                ((GridPager)_pager).Query = _query;
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

            _settings = new QueryStringGridSettingsProvider(_query);
            _columnsCollection.SortSettings = _settings.SortSettings;
            _columnsCollection.UpdateColumnsSorting();
            ((GridPager)_pager).Query = _query;
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

            _settings = new QueryStringGridSettingsProvider(_query);
            _columnsCollection.SortSettings = _settings.SortSettings;
            _columnsCollection.UpdateColumnsSorting();
            ((GridPager)_pager).Query = _query;
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

        public IList<object> GetValuesToDisplay(string columnName, IEnumerable<object> items)
        {
            var column = Columns.SingleOrDefault(r => r.Name == columnName);
            if (column == null)
                return new List<object>();
            return ((IGridColumn<T>)column).Group.GetColumnValues((items as IEnumerable<T>).AsQueryable()).ToList();
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

        public async Task UpdateGrid()
        {
            try
            {
                ItemsDTO<T> response;
                if (_dataService != null)
                {
                    response = _dataService((QueryDictionary<StringValues>)_query);
                }
                else if (_dataServiceAsync != null)
                {
                    response = await _dataServiceAsync((QueryDictionary<StringValues>)_query);
                }
                else
                {
                    string urlParameters = ((GridPager)_pager).GetLink();
                    if (Url.Contains("?"))
                        urlParameters = urlParameters.Replace("?", "&");
                    response = await HttpClient.GetJsonAsync<ItemsDTO<T>>(Url + urlParameters);       
                }
                if (response != null && response.Items != null && response.Pager != null)
                {
                    Items = response.Items;
                    EnablePaging = response.Pager.EnablePaging;
                    ((GridPager)_pager).CurrentPage = response.Pager.CurrentPage;
                    ((GridPager)_pager).PageSize = response.Pager.PageSize;
                    ((GridPager)_pager).ItemsCount = response.Pager.ItemsCount;

                    if (response.Totals != null)
                    {
                        if (response.Totals.Sum != null)
                            foreach (var keyValue in response.Totals.Sum)
                            {
                                var column = (GridColumnBase<T>)Columns.SingleOrDefault(r => r.Name != null && r.Name.Equals(keyValue.Key));
                                if (column != null && column.IsSumEnabled)
                                    column.SumString = keyValue.Value;
                            }

                        if (response.Totals.Average != null)
                            foreach (var keyValue in response.Totals.Average)
                            {
                                var column = (GridColumnBase<T>)Columns.SingleOrDefault(r => r.Name != null && r.Name.Equals(keyValue.Key));
                                if (column != null && column.IsAverageEnabled)
                                    column.AverageString = keyValue.Value;
                            }

                        if (response.Totals.Max != null)
                            foreach (var keyValue in response.Totals.Max)
                            {
                                var column = (GridColumnBase<T>)Columns.SingleOrDefault(r => r.Name != null && r.Name.Equals(keyValue.Key));
                                if (column != null && column.IsMaxEnabled)
                                    column.MaxString = keyValue.Value;
                            }

                        if (response.Totals.Min != null)
                            foreach (var keyValue in response.Totals.Min)
                            {
                                var column = (GridColumnBase<T>)Columns.SingleOrDefault(r => r.Name != null && r.Name.Equals(keyValue.Key));
                                if (column != null && column.IsMinEnabled)
                                    column.MinString = keyValue.Value;
                            }
                    }          
                }
                else
                    Console.WriteLine("Response is null");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }
    }
}