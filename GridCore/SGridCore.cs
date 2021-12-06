﻿using GridCore.Columns;
using GridCore.DataAnnotations;
using GridCore.Filtering;
using GridCore.Pagination;
using GridCore.Resources;
using GridCore.Searching;
using GridCore.Sorting;
using GridCore.Totals;
using GridShared;
using GridShared.Columns;
using GridShared.DataAnnotations;
using GridShared.Grouping;
using GridShared.Totals;
using GridShared.Utility;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.Json;

namespace GridCore
{
    /// <summary>
    ///     Grid.Mvc base class
    /// </summary>
    public class SGridCore<T> : GridBase<T>, ISGrid<T>
    {
        internal IGridAnnotationsProvider _annotations;
        protected IColumnBuilder<T> _columnBuilder;
        protected GridColumnCollection<T> _columnsCollection;
        internal SearchGridItemsProcessor<T> _currentSearchItemsProcessor;
        internal FilterGridItemsProcessor<T> _currentFilterItemsProcessor;
        internal SortGridItemsProcessor<T> _currentSortItemsProcessor;
        internal TotalsGridItemsProcessor<T> _currentTotalsItemsProcessor;
        protected int _displayingItemsCount = -1; // count of displaying items (if using pagination)
        protected bool _enablePaging;
        protected IGridPager _pager;

        protected IGridItemsProcessor<T> _pagerProcessor;
        protected IGridSettingsProvider _settings;

        public SGridCore()
        { }

        public SGridCore(IEnumerable<T> items, IQueryCollection query, bool renderOnlyRows,
            string pagerViewName = GridPager.DefaultPagerViewName, IColumnBuilder<T> columnBuilder = null)
            : this(items, QueryDictionary<StringValues>.Convert(query), renderOnlyRows, pagerViewName, columnBuilder)
        {
        }

        public SGridCore(IEnumerable<T> items, QueryDictionary<StringValues> query, bool renderOnlyRows,
            string pagerViewName = GridPager.DefaultPagerViewName, IColumnBuilder<T> columnBuilder = null)
            : this(items, query, columnBuilder)
        {
            var urlParameters = CustomQueryStringBuilder.Convert(query);
            string pageParameter = urlParameters[((GridPager)Pager).ParameterName];
            int page = 0;
            if (pageParameter != null)
                int.TryParse(pageParameter, out page);
            if (page == 0)
                page++;
            ((GridPager)_pager).CurrentPage = page;
            ((GridPager)_pager).TemplateName = pagerViewName;
            RenderOptions.RenderRowsOnly = renderOnlyRows;
        }

        public SGridCore(IEnumerable<T> items, IQueryCollection query, IColumnBuilder<T> columnBuilder = null)
            : this(items, QueryDictionary<StringValues>.Convert(query), columnBuilder)
        { }

        public SGridCore(IEnumerable<T> items, QueryDictionary<StringValues> query, IColumnBuilder<T> columnBuilder = null)
            : this()
        {
            BeforeItems = items.AsQueryable();

            #region init default properties

            Query = query;

            //set up sort settings:
            _settings = new QueryStringGridSettingsProvider(Query);

            Sanitizer = new Sanitizer();
            EmptyGridText = Strings.DefaultGridEmptyText;
            Language = Strings.Lang;

            _currentSortItemsProcessor = new SortGridItemsProcessor<T>(this, _settings.SortSettings);
            _currentFilterItemsProcessor = new FilterGridItemsProcessor<T>(this, _settings.FilterSettings);
            _currentSearchItemsProcessor = new SearchGridItemsProcessor<T>(this, _settings.SearchSettings);
            _currentTotalsItemsProcessor = new TotalsGridItemsProcessor<T>(this);
            AddItemsPreProcessor(_currentFilterItemsProcessor);
            AddItemsPreProcessor(_currentSearchItemsProcessor);
            InsertItemsProcessor(0, _currentSortItemsProcessor);
            SetTotalsProcessor(_currentTotalsItemsProcessor);

            #endregion init default properties

            _annotations = new GridCoreAnnotationsProvider();

            //Set up column collection:
            if (columnBuilder == null)
                _columnBuilder = new DefaultColumnCoreBuilder<T>(this, _annotations);
            else
                _columnBuilder = columnBuilder;
            _columnsCollection = new GridColumnCollection<T>(this, _columnBuilder, _settings.SortSettings);
            RenderOptions = new GridRenderOptions();

            ApplyGridSettings();
        }

        /// <summary>
        ///     Grid columns collection
        /// </summary>
        public IGridColumnCollection<T> Columns
        {
            get { return _columnsCollection; }
        }

        public bool SearchingEnabled { get; set; }

        public bool SearchingOnlyTextColumns { get; set; }

        public bool SearchingHiddenColumns { get; set; }

        public bool ExtSortingEnabled { get; set; }

        public bool HiddenExtSortingHeader { get; set; } = false;

        public bool GroupingEnabled { get; set; }

        public bool ClearFiltersButtonEnabled { get; set; } = false;

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

        public GridRenderOptions RenderOptions { get; set; }

        /// <summary>
        ///     Provides settings, using by the grid
        /// </summary>
        public override IGridSettingsProvider Settings
        {
            get { return _settings; }
            set
            {
                _settings = value;
                _currentSortItemsProcessor.UpdateSettings(_settings.SortSettings);
                _currentFilterItemsProcessor.UpdateSettings(_settings.FilterSettings);
                _currentSearchItemsProcessor.UpdateSettings(_settings.SearchSettings);
            }
        }

        public MethodInfo RemoveDiacritics { get; set; } = null;

        /// <summary>
        ///     Items, displaying in the grid view
        /// </summary>
        public IEnumerable<object> ItemsToDisplay
        {
            get { return (IEnumerable<object>)GetItemsToDisplay(); }
        }

        /// <summary>
        ///     Provides query, using by the grid
        /// </summary>
        public QueryDictionary<StringValues> Query { get; internal protected set; }

        #region IGrid Members

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
                if (_enablePaging)
                {
                    if (_pagerProcessor == null)
                        _pagerProcessor = new PagerGridItemsProcessor<T>(Pager);
                    AddItemsProcessor(_pagerProcessor);
                }
                else
                {
                    RemoveItemsProcessor(_pagerProcessor);
                }
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
        public GridMode Mode { get { return GridMode.Grid; } }

        /// <summary>
        ///     Get value for creating items
        /// </summary>
        public bool CreateEnabled { get { return false; } }

        /// <summary>
        ///     Get value for reading items
        /// </summary>
        public bool ReadEnabled { get { return false; } }

        /// <summary>
        ///     Get value for updating items
        /// </summary>
        public bool UpdateEnabled { get { return false; } }

        /// <summary>
        ///     Get value for deleting items
        /// </summary>
        public bool DeleteEnabled { get { return false; } }

        /// <summary>
        ///     Manage pager properties
        /// </summary>
        public IGridPager Pager
        {
            get { return _pager ?? (_pager = new GridPager(Query)); }
            set { _pager = value; }
        }

        /// <summary>
        ///     Keys for subgrid
        /// </summary>
        public string[] SubGridKeys { get; set; }

        /// <summary>
        ///     Get foreign key values for subgrid records
        /// </summary>
        public string[] GetSubGridKeyValues(object item)
        {
            List<string> values = new List<string>();
            foreach (var key in SubGridKeys)
            {
                string value = item.GetType().GetProperty(key).GetValue(item).ToString();
                values.Add(value);
            }
            return values.ToArray();
        }

        IGridColumnCollection IGrid.Columns
        {
            get { return Columns; }
        }

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

        #endregion IGrid Members

        /// <summary>
        ///     Applies data annotations settings
        /// </summary>
        protected internal void ApplyGridSettings()
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
        ///     Methods returns items that will need to be displayed
        /// </summary>
        public virtual IEnumerable<T> GetItemsToDisplay()
        {
            PrepareItemsToDisplay();
            return AfterItems;
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

        public TotalsDTO GetTotals()
        {
            var totals = new TotalsDTO();
            if (IsSumEnabled)
                foreach (ITotalsColumn column in Columns)
                {
                    if (column.IsSumEnabled)
                        totals.Sum.Add(((IGridColumn)column).Name, column.SumString);
                }

            if (IsAverageEnabled)
                foreach (ITotalsColumn column in Columns)
                {
                    if (column.IsAverageEnabled)
                        totals.Average.Add(((IGridColumn)column).Name, column.AverageString);
                }

            if (IsMaxEnabled)
                foreach (ITotalsColumn column in Columns)
                {
                    if (column.IsMaxEnabled)
                        totals.Max.Add(((IGridColumn)column).Name, column.MaxString);
                }

            if (IsMinEnabled)
                foreach (ITotalsColumn column in Columns)
                {
                    if (column.IsMinEnabled)
                        totals.Min.Add(((IGridColumn)column).Name, column.MinString);
                }

            return totals;
        }

        public string GetState()
        {
            var jsonOptions = new JsonSerializerOptions();
            jsonOptions.Converters.Add(new StringValuesConverter());
            string jsonQuery = JsonSerializer.Serialize(Query, jsonOptions);
            return jsonQuery.GridStateEncode();
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
            if (group == null)
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
    }
}