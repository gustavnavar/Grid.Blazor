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
using System.Threading.Tasks;

namespace GridBlazor
{
    /// <summary>
    ///     Grid.Mvc base class
    /// </summary>
    public class CGrid<T> : ICGrid<T>
    {
        private Func<T, string> _rowCssClassesContraint;

        private IQueryDictionary<StringValues> _query;
        private IGridSettingsProvider _settings;
        private readonly IGridAnnotaionsProvider _annotations;
        private readonly IColumnBuilder<T> _columnBuilder;
        private readonly GridColumnCollection<T> _columnsCollection;
        private int _displayingItemsCount = -1; // count of displaying items (if using pagination)
        private bool _enablePaging;
        private IGridPager _pager;

        private readonly Func<QueryDictionary<StringValues>, ItemsDTO<T>> _dataService;

        public CGrid(string url, IQueryDictionary<StringValues> query, bool renderOnlyRows,
            Action<IGridColumnCollection<T>> columns = null, CultureInfo cultureInfo = null)
        {
            _dataService = null;

            Items = new List<T>();

            Url = url;
            _query = query;

            //set up sort settings:
            _settings = new QueryStringGridSettingsProvider(_query);
            Sanitizer = new Sanitizer();
            if(cultureInfo != null)
                Strings.CultureInfo = cultureInfo;
            EmptyGridText = Strings.DefaultGridEmptyText;
            Language = Strings.Lang;

            _annotations = new GridAnnotaionsProvider();

            //Set up column collection:
            _columnBuilder = new DefaultColumnBuilder<T>(this, _annotations);
            _columnsCollection = new GridColumnCollection<T>(_columnBuilder, _settings.SortSettings);
            ComponentOptions = new GridOptions();

            ApplyGridSettings();

            Pager = new GridPager(query);

            ComponentOptions.RenderRowsOnly = renderOnlyRows;
            columns?.Invoke(Columns);      
        }

        public CGrid(Func<QueryDictionary<StringValues>, ItemsDTO<T>> dataService,
            QueryDictionary<StringValues> query, bool renderOnlyRows, 
            Action<IGridColumnCollection<T>> columns = null, CultureInfo cultureInfo = null)
        {
            _dataService = dataService;

            Items = new List<T>(); //response.Items;

            Url = null;
            _query = query;

            //set up sort settings:
            _settings = new QueryStringGridSettingsProvider(_query);
            Sanitizer = new Sanitizer();
            if (cultureInfo != null)
                Strings.CultureInfo = cultureInfo;
            EmptyGridText = Strings.DefaultGridEmptyText;
            Language = Strings.Lang;

            _annotations = new GridAnnotaionsProvider();

            //Set up column collection:
            _columnBuilder = new DefaultColumnBuilder<T>(this, _annotations);
            _columnsCollection = new GridColumnCollection<T>(_columnBuilder, _settings.SortSettings);
            ComponentOptions = new GridOptions();

            ApplyGridSettings();

            Pager = new GridPager(query);

            ComponentOptions.RenderRowsOnly = renderOnlyRows;
            columns?.Invoke(Columns);
        }

        /// <summary>
        /// Total count of items in the grid
        /// </summary>
        public int ItemsCount { get { return _pager.ItemsCount; } }

        public bool SearchingEnabled { get; set; }

        public bool SearchingOnlyTextColumns { get; set; }

        /// <summary>
        ///     Items, displaying in the grid view
        /// </summary>
        public IEnumerable<object> ItemsToDisplay
        {
            get { return (IEnumerable<object>)GetItemsToDisplay(); }
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
        public IEnumerable<T> Items { get; set; }

        /// <summary>
        ///     Provides settings, using by the grid
        /// </summary>
        public IGridSettingsProvider Settings
        {
            get { return _settings; }
            set
            {
                _query = value.ToQuery();
                if (_pager.CurrentPage > 0)
                    _query.Add(((GridPager)_pager).ParameterName, _pager.CurrentPage.ToString());
                _settings = new QueryStringGridSettingsProvider(_query);
                _columnsCollection.SortSettings = _settings.SortSettings;
                ((GridPager)_pager).Query = _query;
            }
        }

        /// <summary>
        ///     Provides url, using by the grid
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        ///     Provides query, using by the grid
        /// </summary>
        public IQueryDictionary<StringValues> Query
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
            get { return _pager ?? (_pager = new GridPager(_query)); }
            set { _pager = value; }
        }

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
                _query.Remove(parameterName);

            _settings = new QueryStringGridSettingsProvider(_query);
            _columnsCollection.SortSettings = _settings.SortSettings;
            _columnsCollection.UpdateColumnsSorting();
            ((GridPager)_pager).Query = _query;
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

        public async Task UpdateGrid()
        {
            try
            {
                ItemsDTO<T> response;
                if (_dataService == null)
                {
                    string urlParameters = ((GridPager)_pager).GetLink();
                    HttpClient httpClient = new HttpClient();
                    response = await httpClient.GetJsonAsync<ItemsDTO<T>>(Url + urlParameters);
                }
                else
                {
                    response = _dataService((QueryDictionary<StringValues>)_query);
                }
                if (response != null && response.Items != null && response.Pager != null)
                {
                    Items = response.Items;
                    EnablePaging = response.Pager.EnablePaging;
                    _pager = new GridPager(_query, response.Pager.CurrentPage);
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