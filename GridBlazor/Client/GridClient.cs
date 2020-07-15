using GridBlazor.Pages;
using GridBlazor.Pagination;
using GridBlazor.Resources;
using GridShared;
using GridShared.Columns;
using GridShared.Utility;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;

namespace GridBlazor
{
    /// <summary>
    ///     Grid adapter for html helper
    /// </summary>
    public class GridClient<T> : IGridClient<T>
    {
        protected readonly CGrid<T> _source;

        public GridClient(HttpClient httpClient, string url, IQueryDictionary<StringValues> query, bool renderOnlyRows,
            string gridName, Action<IGridColumnCollection<T>> columns = null, CultureInfo cultureInfo = null,
            IColumnBuilder<T> columnBuilder = null)
        {
            _source = new CGrid<T>(httpClient, url, query, renderOnlyRows, columns, cultureInfo, columnBuilder);
            Named(gridName);
            //WithPaging(_source.Pager.PageSize);
        }

        [Obsolete("This constructor is obsolete. Use one including an HttpClient parameter.", false)]
        public GridClient(string url, IQueryDictionary<StringValues> query, bool renderOnlyRows, 
            string gridName, Action<IGridColumnCollection<T>> columns = null, CultureInfo cultureInfo = null,
            IColumnBuilder<T> columnBuilder = null)
        {
            _source =  new CGrid<T>(url, query, renderOnlyRows, columns, cultureInfo, columnBuilder);
            Named(gridName);
            //WithPaging(_source.Pager.PageSize);
        }

        public GridClient(Func<QueryDictionary<StringValues>, ItemsDTO<T>> dataService, 
            QueryDictionary<StringValues> query, bool renderOnlyRows, string gridName, 
            Action<IGridColumnCollection<T>> columns = null, CultureInfo cultureInfo = null,
            IColumnBuilder<T> columnBuilder = null)
        {
            _source = new CGrid<T>(dataService, query, renderOnlyRows, columns, cultureInfo, columnBuilder);
            Named(gridName);
            //WithPaging(_source.Pager.PageSize);
        }

        public GridClient(Func<QueryDictionary<StringValues>, Task<ItemsDTO<T>>> dataServiceAsync,
            QueryDictionary<StringValues> query, bool renderOnlyRows, string gridName,
            Action<IGridColumnCollection<T>> columns = null, CultureInfo cultureInfo = null,
            IColumnBuilder<T> columnBuilder = null)
        {
            _source = new CGrid<T>(dataServiceAsync, query, renderOnlyRows, columns, cultureInfo, columnBuilder);
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
            return Searchable(enable, onlyTextColumns, false);
        }

        public IGridClient<T> Searchable(bool enable, bool onlyTextColumns, bool hiddenColumns)
        {
            _source.SearchingEnabled = enable;
            _source.SearchingOnlyTextColumns = onlyTextColumns;
            _source.SearchingHiddenColumns = hiddenColumns;
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

        public IGridClient<T> ClearFiltersButton(bool enable)
        {
            _source.ClearFiltersButtonEnabled = enable;
            return this;
        }

        public IGridClient<T> Selectable(bool enable)
        {
            return Selectable(enable, false);
        }

        public IGridClient<T> MultiSelectable(bool multiSelectable)
        {
            return Selectable(true, false, multiSelectable);
        }

        public IGridClient<T> Selectable(bool enable, bool initSelection)
        {
            return Selectable(enable, initSelection, false);
        }

        public IGridClient<T> Selectable(bool enable, bool initSelection, bool multiSelectable)
        {
            _source.ComponentOptions.Selectable = enable;
            _source.ComponentOptions.InitSelection = initSelection;
            _source.ComponentOptions.MultiSelectable = multiSelectable;
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
            _source.DeleteEnabled = deleteEnabled;
            _source.CrudDataService = crudDataService;
            return this;
        }

        public IGridClient<T> Crud(bool createEnabled, Func<T, bool> enabled, ICrudDataService<T> crudDataService)
        {
            return Crud(createEnabled, enabled, enabled, enabled, crudDataService);
        }

        public IGridClient<T> Crud(bool createEnabled, Func<T, bool> readEnabled, Func<T, bool> updateEnabled,
            Func<T, bool> deleteEnabled, ICrudDataService<T> crudDataService)
        {
            _source.CreateEnabled = createEnabled;
            _source.FuncReadEnabled = readEnabled;
            _source.FuncUpdateEnabled = updateEnabled;
            _source.FuncDeleteEnabled = deleteEnabled;
            _source.CrudDataService = crudDataService;
            return this;
        }

        public IGridClient<T> ODataCrud(bool enabled)
        {
            return ODataCrud(enabled, enabled, enabled, enabled);
        }

        public IGridClient<T> ODataCrud(bool createEnabled, bool readEnabled, bool updateEnabled, bool deleteEnabled)
        {
            _source.CreateEnabled = createEnabled;
            _source.ReadEnabled = readEnabled;
            _source.UpdateEnabled = updateEnabled;
            _source.DeleteEnabled = deleteEnabled;
            return this;
        }

        public IGridClient<T> ODataCrud(bool createEnabled, Func<T, bool> enabled)
        {
            return ODataCrud(createEnabled, enabled, enabled, enabled);
        }

        public IGridClient<T> ODataCrud(bool createEnabled, Func<T, bool> readEnabled, 
            Func<T, bool> updateEnabled, Func<T, bool> deleteEnabled)
        {
            _source.CreateEnabled = createEnabled;
            _source.FuncReadEnabled = readEnabled;
            _source.FuncUpdateEnabled = updateEnabled;
            _source.FuncDeleteEnabled = deleteEnabled;
            return this;
        }

        public IGridClient<T> SetCreateComponent<TComponent>()
        {
            return SetCreateComponent<TComponent>(null, null, null);
        }

        public IGridClient<T> SetCreateComponent<TComponent>(IList<Action<object>> actions)
        {
            return SetCreateComponent<TComponent>(actions, null, null);
        }

        public IGridClient<T> SetCreateComponent<TComponent>(object obj)
        {
            return SetCreateComponent<TComponent>(null, null, obj);
        }

        public IGridClient<T> SetCreateComponent<TComponent>(IList<Action<object>> actions, Object obj)
        {
            return SetCreateComponent<TComponent>(actions, null, obj);
        }

        public IGridClient<T> SetCreateComponent<TComponent>(IList<Func<object, Task>> functions)
        {
            return SetCreateComponent<TComponent>(null, functions, null);
        }

        public IGridClient<T> SetCreateComponent<TComponent>(IList<Action<object>> actions, 
            IList<Func<object, Task>> functions)
        {
            return SetCreateComponent<TComponent>(actions, functions, null);
        }

        public IGridClient<T> SetCreateComponent<TComponent>(IList<Func<object, Task>> functions, object obj)
        {
            return SetCreateComponent<TComponent>(null, functions, obj);
        }

        public IGridClient<T> SetCreateComponent<TComponent>(IList<Action<object>> actions, 
            IList<Func<object, Task>> functions, object obj)
        {
            Type readComponent = typeof(TComponent);
            if (readComponent != null && readComponent.IsSubclassOf(typeof(GridCreateComponent<T>)))
            {
                _source.CreateComponent = readComponent;
                _source.CreateActions = actions;
                _source.CreateFunctions = functions;
                _source.CreateObject = obj;
            }
            return this;
        }

        public IGridClient<T> SetReadComponent<TComponent>()
        {
            return SetReadComponent<TComponent>(null, null, null);
        }

        public IGridClient<T> SetReadComponent<TComponent>(IList<Action<object>> actions)
        {
            return SetReadComponent<TComponent>(actions, null, null);
        }

        public IGridClient<T> SetReadComponent<TComponent>(object obj)
        {
            return SetReadComponent<TComponent>(null, null, obj);
        }

        public IGridClient<T> SetReadComponent<TComponent>(IList<Action<object>> actions, object obj)
        {
            return SetReadComponent<TComponent>(actions, null, obj);
        }

        public IGridClient<T> SetReadComponent<TComponent>(IList<Func<object, Task>> functions)
        {
            return SetReadComponent<TComponent>(null, functions, null);
        }

        public IGridClient<T> SetReadComponent<TComponent>(IList<Func<object, Task>> functions, object obj)
        {
            return SetReadComponent<TComponent>(null, functions, obj);
        }

        public IGridClient<T> SetReadComponent<TComponent>(IList<Action<object>> actions,
            IList<Func<object, Task>> functions)
        {
            return SetReadComponent<TComponent>(actions, functions, null);
        }

        public IGridClient<T> SetReadComponent<TComponent>(IList<Action<object>> actions, 
            IList<Func<object, Task>> functions, object obj)
        {
            Type readComponent = typeof(TComponent);
            if (readComponent != null && readComponent.IsSubclassOf(typeof(GridReadComponent<T>)))
            {
                _source.ReadComponent = readComponent;
                _source.ReadActions = actions;
                _source.ReadFunctions = functions;
                _source.ReadObject = obj;
            }
            return this;
        }

        public IGridClient<T> SetUpdateComponent<TComponent>()
        {
            return SetUpdateComponent<TComponent>(null, null, null);
        }

        public IGridClient<T> SetUpdateComponent<TComponent>(IList<Action<object>> actions)
        {
            return SetUpdateComponent<TComponent>(actions, null, null);
        }

        public IGridClient<T> SetUpdateComponent<TComponent>(object obj)
        {
            return SetUpdateComponent<TComponent>(null, null, obj);
        }

        public IGridClient<T> SetUpdateComponent<TComponent>(IList<Func<object, Task>> functions)
        {
            return SetUpdateComponent<TComponent>(null, functions, null);
        }

        public IGridClient<T> SetUpdateComponent<TComponent>(IList<Action<object>> actions,
            IList<Func<object, Task>> functions)
        {
            return SetUpdateComponent<TComponent>(actions, functions, null);
        }

        public IGridClient<T> SetUpdateComponent<TComponent>(IList<Func<object, Task>> functions, object obj)
        {
            return SetUpdateComponent<TComponent>(null, functions, obj);
        }

        public IGridClient<T> SetUpdateComponent<TComponent>(IList<Action<object>> actions, object obj)
        {
            return SetUpdateComponent<TComponent>(actions, null, obj);
        }

        public IGridClient<T> SetUpdateComponent<TComponent>(IList<Action<object>> actions,
            IList<Func<object, Task>> functions, object obj)
        {
            Type updateComponent = typeof(TComponent);
            if (updateComponent != null && updateComponent.IsSubclassOf(typeof(GridUpdateComponent<T>)))
            {
                _source.UpdateComponent = updateComponent;
                _source.UpdateActions = actions;
                _source.UpdateFunctions = functions;
                _source.UpdateObject = obj;
            }
            return this;
        }

        public IGridClient<T> SetDeleteComponent<TComponent>()
        {
            return SetDeleteComponent<TComponent>(null, null, null);
        }

        public IGridClient<T> SetDeleteComponent<TComponent>(IList<Action<object>> actions)
        {
            return SetDeleteComponent<TComponent>(actions, null, null);
        }

        public IGridClient<T> SetDeleteComponent<TComponent>(object obj)
        {
            return SetDeleteComponent<TComponent>(null, null, obj);
        }

        public IGridClient<T> SetDeleteComponent<TComponent>(IList<Func<object, Task>> functions)
        {
            return SetDeleteComponent<TComponent>(null, functions, null);
        }

        public IGridClient<T> SetDeleteComponent<TComponent>(IList<Action<object>> actions,
            IList<Func<object, Task>> functions)
        {
            return SetDeleteComponent<TComponent>(actions, functions, null);
        }

        public IGridClient<T> SetDeleteComponent<TComponent>(IList<Func<object, Task>> functions, object obj)
        {
            return SetDeleteComponent<TComponent>(null, functions, obj);
        }

        public IGridClient<T> SetDeleteComponent<TComponent>(IList<Action<object>> actions, object obj)
        {
            return SetDeleteComponent<TComponent>(actions, null, obj);
        }

        public IGridClient<T> SetDeleteComponent<TComponent>(IList<Action<object>> actions,
            IList<Func<object, Task>> functions, object obj)
        {
            Type deleteComponent = typeof(TComponent);
            if (deleteComponent != null && deleteComponent.IsSubclassOf(typeof(GridDeleteComponent<T>)))
            {
                _source.DeleteComponent = deleteComponent;
                _source.DeleteActions = actions;
                _source.DeleteFunctions = functions;
                _source.DeleteObject = obj;
            }
            return this;
        }

        public IGridClient<T> AddButtonComponent<TComponent>(string name, string label)
        {
            return AddButtonComponent<TComponent>(name, label, null, null, null);
        }

        public IGridClient<T> AddButtonComponent<TComponent>(string name, string label, IList<Action<object>> actions)
        {
            return AddButtonComponent<TComponent>(name, label, actions, null, null);
        }

        public IGridClient<T> AddButtonComponent<TComponent>(string name, string label, object obj)
        {
            return AddButtonComponent<TComponent>(name, label, null, null, obj);
        }

        public IGridClient<T> AddButtonComponent<TComponent>(string name, string label, IList<Func<object, Task>> functions)
        {
            return AddButtonComponent<TComponent>(name, label, null, functions, null);
        }

        public IGridClient<T> AddButtonComponent<TComponent>(string name, string label, IList<Action<object>> actions,
            IList<Func<object, Task>> functions)
        {
            return AddButtonComponent<TComponent>(name, label, actions, functions, null);
        }

        public IGridClient<T> AddButtonComponent<TComponent>(string name, string label, IList<Func<object, Task>> functions, object obj)
        {
            return AddButtonComponent<TComponent>(name, label, null, functions, obj);
        }

        public IGridClient<T> AddButtonComponent<TComponent>(string name, string label, IList<Action<object>> actions, object obj)
        {
            return AddButtonComponent<TComponent>(name, label, actions, null, obj);
        }

        public IGridClient<T> AddButtonComponent<TComponent>(string name, string label, IList<Action<object>> actions,
            IList<Func<object, Task>> functions, object obj)
        {
            Type buttonComponent = typeof(TComponent);
            if (buttonComponent != null)
            {
                _source.ButtonComponents.Add(name, (label, buttonComponent, actions, functions, obj));
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
        ///     Enable or disable striped grid
        /// </summary>
        public IGridClient<T> SetStriped(bool enable)
        {
            _source.ComponentOptions.Striped = enable;
            return this;
        }

        /// <summary>
        ///    Allow grid to show a SubGrid
        /// </summary>
        [Obsolete("This method is obsolete. Use one including an '(string,string)[]' keys parameter.", false)]
        public IGridClient<T> SubGrid(Func<object[], Task<ICGrid>> subGrids, params string[] keys)
        {
            var tupleKeys = new (string, string)[keys.Length];
            for (int i = 0; i < keys.Length; i++)
            {
                tupleKeys[i] = (keys[i], keys[i]);
            }
            return SubGrid(subGrids, tupleKeys);
        }

        /// <summary>
        ///    Allow grid to show a SubGrid
        /// </summary>
        public IGridClient<T> SubGrid(Func<object[], Task<ICGrid>> subGrids, params (string, string)[] keys)
        {
            _source.SubGrids = subGrids;
            _source.SubGridKeys = keys;
            return this;
        }

        /// <summary>
        ///     Configure keyboard utilization
        /// </summary>
        public IGridClient<T> SetKeyboard(bool enable)
        {
            _source.Keyboard = enable;
            return this;
        }

        /// <summary>
        ///     Configure the modifier key
        /// </summary>
        public IGridClient<T> SetModifierKey(ModifierKey modifierKey)
        {
            _source.ModifierKey = modifierKey;
            return this;
        }

        /// <summary>
        ///     Allow grid to export to an Excel file
        /// </summary>
        public IGridClient<T> SetExcelExport(bool enable)
        {
            _source.ExcelExport = enable;
            return this;
        }

        /// <summary>
        ///     Configure the Server API
        /// </summary>
        public IGridClient<T> UseServerAPI(ServerAPI serverAPI)
        {
            _source.ServerAPI = serverAPI;
            return this;
        }

        /// <summary>
        ///     Use OData extend for columns
        /// </summary>
        public IGridClient<T> UseODataExpand(IEnumerable<string> oDataExpandList)
        {
            _source.ODataOverrideExpandList = false;
            _source.ODataExpandList = oDataExpandList;
            return this;
        }

        public IGridClient<T> OverrideODataExpand(IEnumerable<string> oDataExpandList)
        {
            _source.ODataOverrideExpandList = true;
            _source.ODataExpandList = oDataExpandList;
            return this;
        }

        public IGridClient<T> AddToOnAfterRender(Func<GridComponent<T>, bool, Task> OnAfterRender)
        {
            _source.OnAfterRender = OnAfterRender;
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