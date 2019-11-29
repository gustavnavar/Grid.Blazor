﻿using GridBlazor.Pagination;
using GridBlazor.Resources;
using GridShared;
using GridShared.Columns;
using GridShared.Utility;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace GridBlazor
{
    /// <summary>
    ///     Grid adapter for html helper
    /// </summary>
    public class GridClient<T> : IGridClient<T>
    {
        private readonly CGrid<T> _source;

        public GridClient(string url, IQueryDictionary<StringValues> query, bool renderOnlyRows,
            string gridName, Action<IGridColumnCollection<T>> columns = null, CultureInfo cultureInfo = null)
        {
            _source =  new CGrid<T>(url, query, renderOnlyRows, columns, cultureInfo);
            Named(gridName);
            //WithPaging(_source.Pager.PageSize);
        }

        public GridClient(Func<QueryDictionary<StringValues>, ItemsDTO<T>> dataService,
            QueryDictionary<StringValues> query, bool renderOnlyRows, string gridName,
            Action<IGridColumnCollection<T>> columns = null, CultureInfo cultureInfo = null)
        {
            _source = new CGrid<T>(dataService, query, renderOnlyRows, columns, cultureInfo);
            Named(gridName);
            //WithPaging(_source.Pager.PageSize);
        }

        #region IGridHtmlOptions<T> Members

        public IGridClient<T> WithGridItemsCount()
        {
            return WithGridItemsCount(string.Empty);
        }

        public IGridClient<T> Columns(Action<IGridColumnCollection<T>> columnBuilder)
        {
            columnBuilder((IGridColumnCollection<T>)_source.Columns);
            return this;
        }

        public IGridClient<T> ChangePageSize(bool enable)
        {
            _source.Pager.ChangePageSize = enable;
            return this;
        }

        public IGridClient<T> WithPaging(int pageSize)
        {
            return WithPaging(pageSize, 0);
        }

        public IGridClient<T> WithPaging(int pageSize, int maxDisplayedItems)
        {
            return WithPaging(pageSize, maxDisplayedItems, string.Empty);
        }

        public IGridClient<T> WithPaging(int pageSize, int maxDisplayedItems, string queryStringParameterName)
        {
            _source.EnablePaging = true;
            _source.Pager.PageSize = pageSize;

            var pager = _source.Pager as GridPager; //This setting can be applied only to default grid pager
            if (pager == null) return this;

            if (maxDisplayedItems > 0)
                pager.MaxDisplayedPages = maxDisplayedItems;
            if (!string.IsNullOrEmpty(queryStringParameterName))
                pager.ParameterName = queryStringParameterName;
            _source.Pager = pager;
            return this;
        }

        public IGridClient<T> Sortable()
        {
            return Sortable(true);
        }

        public IGridClient<T> Sortable(bool enable)
        {
            _source.DefaultSortEnabled = enable;
            foreach (IGridColumn column in _source.Columns)
            {
                var typedColumn = column as IGridColumn<T>;
                if (typedColumn == null) continue;
                typedColumn.Sortable(enable);
            }
            return this;
        }

        public IGridClient<T> Filterable()
        {
            return Filterable(true);
        }

        public IGridClient<T> Filterable(bool enable)
        {
            _source.DefaultFilteringEnabled = enable;
            foreach (IGridColumn column in _source.Columns)
            {
                var typedColumn = column as IGridColumn<T>;
                if (typedColumn == null) continue;
                typedColumn.Filterable(enable);
            }
            return this;
        }


        public IGridClient<T> Searchable()
        {
            return Searchable(true, true);
        }

        public IGridClient<T> Searchable(bool enable)
        {
            return Searchable(enable, true);
        }

        public IGridClient<T> Searchable(bool enable, bool onlyTextColumns)
        {
            _source.SearchingEnabled = enable;
            _source.SearchingOnlyTextColumns = onlyTextColumns;
            return this;
        }

        public IGridClient<T> ExtSortable()
        {
            return ExtSortable(true);
        }

        public IGridClient<T> ExtSortable(bool enable)
        {
            _source.ExtSortingEnabled = enable;
            return this;
        }

        public IGridClient<T> Groupable()
        {
            return Groupable(true);
        }

        public IGridClient<T> Groupable(bool enable)
        {
            _source.ExtSortingEnabled = enable;
            _source.GroupingEnabled = enable;
            return this;
        }

        public IGridClient<T> Selectable(bool set)
        {
            _source.ComponentOptions.Selectable = set;
            return this;
        }

        public IGridClient<T> Crud(bool enabled, ICrudDataService<T> crudDataService)
        {
            return Crud(enabled, enabled, enabled, enabled, crudDataService);
        }

        public IGridClient<T> Crud(bool createEnabled, bool readEnabled, bool updateEnabled, bool deleteEnabled,
            ICrudDataService<T> crudDataService)
        {
            _source.CreateEnabled = createEnabled;
            _source.ReadEnabled = readEnabled;
            _source.UpdateEnabled = updateEnabled;
            _source.DeleteEnabled = updateEnabled;
            _source.CrudDataService = crudDataService;
            return this;
        }

        public IGridClient<T> SetCreateComponent<TComponent>()
        {
            return SetCreateComponent<TComponent>(null, null);
        }

        public IGridClient<T> SetCreateComponent<TComponent>(IList<Action<object>> actions)
        {
            return SetCreateComponent<TComponent>(actions, null);
        }

        public IGridClient<T> SetCreateComponent<TComponent>(object obj)
        {
            return SetCreateComponent<TComponent>(null, obj);
        }

        public IGridClient<T> SetCreateComponent<TComponent>(IList<Action<object>> actions, object obj)
        {
            Type readComponent = typeof(TComponent);
            if (readComponent != null && readComponent.IsSubclassOf(typeof(GridCreateComponentBase<T>)))
            {
                _source.CreateComponent = readComponent;
                _source.CreateActions = actions;
                _source.CreateObject = obj;
            }
            return this;
        }

        public IGridClient<T> SetReadComponent<TComponent>()
        {
            return SetReadComponent<TComponent>(null, null);
        }

        public IGridClient<T> SetReadComponent<TComponent>(IList<Action<object>> actions)
        {
            return SetReadComponent<TComponent>(actions, null);
        }

        public IGridClient<T> SetReadComponent<TComponent>(object obj)
        {
            return SetReadComponent<TComponent>(null, obj);
        }

        public IGridClient<T> SetReadComponent<TComponent>(IList<Action<object>> actions, object obj)
        {
            Type readComponent = typeof(TComponent);
            if (readComponent != null && readComponent.IsSubclassOf(typeof(GridReadComponentBase<T>)))
            {
                _source.ReadComponent = readComponent;
                _source.ReadActions = actions;
                _source.ReadObject = obj;
            }
            return this;
        }

        public IGridClient<T> SetUpdateComponent<TComponent>()
        {
            return SetUpdateComponent<TComponent>(null, null);
        }

        public IGridClient<T> SetUpdateComponent<TComponent>(IList<Action<object>> actions)
        {
            return SetUpdateComponent<TComponent>(actions, null);
        }

        public IGridClient<T> SetUpdateComponent<TComponent>(object obj)
        {
            return SetUpdateComponent<TComponent>(null, obj);
        }

        public IGridClient<T> SetUpdateComponent<TComponent>(IList<Action<object>> actions, object obj)
        {
            Type updateComponent = typeof(TComponent);
            if (updateComponent != null && updateComponent.IsSubclassOf(typeof(GridUpdateComponentBase<T>)))
            {
                _source.UpdateComponent = updateComponent;
                _source.UpdateActions = actions;
                _source.UpdateObject = obj;
            }
            return this;
        }

        public IGridClient<T> SetDeleteComponent<TComponent>()
        {
            return SetDeleteComponent<TComponent>(null, null);
        }

        public IGridClient<T> SetDeleteComponent<TComponent>(IList<Action<object>> actions)
        {
            return SetDeleteComponent<TComponent>(actions, null);
        }

        public IGridClient<T> SetDeleteComponent<TComponent>(object obj)
        {
            return SetDeleteComponent<TComponent>(null, obj);
        }

        public IGridClient<T> SetDeleteComponent<TComponent>(IList<Action<object>> actions, object obj)
        {
            Type deleteComponent = typeof(TComponent);
            if (deleteComponent != null && deleteComponent.IsSubclassOf(typeof(GridDeleteComponentBase<T>)))
            {
                _source.DeleteComponent = deleteComponent;
                _source.DeleteActions = actions;
                _source.DeleteObject = obj;
            }
            return this;
        }

        public IGridClient<T> EmptyText(string text)
        {
            _source.EmptyGridText = text;
            return this;
        }

        public IGridClient<T> SetLanguage(string lang)
        {
            _source.Language = lang;
            return this;
        }

        public IGridClient<T> SetRowCssClasses(Func<T, string> contraint)
        {
            _source.SetRowCssClassesContraint(contraint);
            return this;
        }

        public IGridClient<T> Named(string gridName)
        {
            _source.ComponentOptions.GridName = gridName;
            return this;
        }

        /// <summary>
        ///     Generates columns for all properties of the model.
        ///     Use data annotations to customize columns
        /// </summary>
        public IGridClient<T> AutoGenerateColumns()
        {
            _source.AutoGenerateColumns();
            return this;
        }

        public IGridClient<T> WithMultipleFilters()
        {
            _source.ComponentOptions.AllowMultipleFilters = true;
            return this;
        }

        /// <summary>
        ///     Set to true if we want to show grid itmes count
        ///     - Author - Jeeva J
        /// </summary>
        public IGridClient<T> WithGridItemsCount(string gridItemsName)
        {
            if (string.IsNullOrWhiteSpace(gridItemsName))
                gridItemsName = Strings.Items;

            _source.ComponentOptions.GridCountDisplayName = gridItemsName;
            _source.ComponentOptions.ShowGridItemsCount = true;
            return this;
        }

        /// <summary>
        ///    Allow grid to show a SubGrid
        /// </summary>
        public IGridClient<T> SubGrid(Func<object[], Task<ICGrid>> subGrids, params string[] keys)
        {
            _source.SubGrids = subGrids;
            _source.SubGridKeys = keys;
            return this;
        }

        /// <summary>
        ///     Get grid object
        /// </summary>
        public CGrid<T> Grid
        {
            get { return _source; }
        }

        public async Task UpdateGrid()
        {
            await _source.UpdateGrid();
        }
    }
    #endregion
}
