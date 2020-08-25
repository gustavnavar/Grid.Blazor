using GridBlazor.Pagination;
using GridBlazor.Resources;
using GridBlazor.Searching;
using GridShared;
using GridShared.Columns;
using GridShared.Events;
using GridShared.Filtering;
using GridShared.OData;
using GridShared.Pagination;
using GridShared.Sorting;
using GridShared.Utility;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace GridBlazor.Pages
{
    public partial class GridComponent<T>
    {
        private int _sequence = 0;
        private bool _fromCrud = false;
        private bool _shouldRender = false;
        protected bool _hasSubGrid = false;
        protected bool _hasTotals = false;
        protected bool _requiredTotalsColumn = false;
        protected string _changePageSizeUrl;
        protected int _pageSize;
        internal bool[] IsSubGridVisible;
        internal bool[] InitSubGrid;
        protected IQueryDictionary<Type> _filterComponents;
        protected T _item;

        // browser input type support
        internal bool _isDateTimeLocalSupported = false;
        internal bool _isWeekSupported = false;
        internal bool _isMonthSupported = false;

        // CRUD buttons on the header
        internal bool HeaderCrudButtons = false;

        protected ElementReference gridmvc;

        public event Func<object, SortEventArgs, Task> SortChanged;
        public event Func<object, ExtSortEventArgs, Task> ExtSortChanged;
        public event Func<object, FilterEventArgs, Task<bool>> BeforeFilterChanged;
        public event Func<object, FilterEventArgs, Task> FilterChanged;
        public event Func<object, SearchEventArgs, Task> SearchChanged;
        public event Func<object, PagerEventArgs, Task> PagerChanged;

        public event Func<GridCreateComponent<T>, T, Task<bool>> BeforeInsert;
        public event Func<GridUpdateComponent<T>, T, Task<bool>> BeforeUpdate;
        public event Func<GridDeleteComponent<T>, T, Task<bool>> BeforeDelete;

        public event Func<GridCreateComponent<T>, T, Task> AfterInsert;
        public event Func<GridUpdateComponent<T>, T, Task> AfterUpdate;
        public event Func<GridDeleteComponent<T>, T, Task> AfterDelete;

        public event Func<Task> BeforeRefreshGrid;
        public event Func<Task> AfterRefreshGrid;

        public event Func<CheckboxEventArgs<T>, Task> HeaderCheckboxChanged;
        public event Func<CheckboxEventArgs<T>, Task> RowCheckboxChanged;

        internal event Action FilterButtonClicked;

        [Inject]
        private IJSRuntime jSRuntime { get; set; }

        internal int SelectedRow { get; set; } = -1;

        internal List<int> SelectedRows { get; set; } = new List<int>();

        [Obsolete("This property is obsolete. Use the new Checkboxes parameter.", true)]
        public QueryDictionary<List<int>> CheckedRows { get; internal set; } = new QueryDictionary<List<int>>();

        public QueryDictionary<Dictionary<int, CheckboxComponent<T>>> Checkboxes { get; private set; }

        public QueryDictionary<QueryDictionary<bool>> ExceptCheckedRows { get; private set; } = new QueryDictionary<QueryDictionary<bool>>();

        public QueryDictionary<GridHeaderComponent<T>> HeaderComponents { get; private set; }

        public ElementReference PageSizeInput { get; private set; }

        public GridSearchComponent<T> SearchComponent { get; private set; }

        public GridCreateComponent<T> CreateComponent { get; private set; }

        public GridReadComponent<T> ReadComponent { get; private set; }

        public GridUpdateComponent<T> UpdateComponent { get; private set; }

        public GridDeleteComponent<T> DeleteComponent { get; private set; }

        internal IGridColumn<T> FirstColumn { get; set; }

        internal ColumnOrderValue Payload { get; set; }

        protected RenderFragment CrudRender { get; set; }

        public string Error { get; set; } = "";

        [Parameter]
        public ICGrid Grid { get; set; }

        [Parameter]
        public Action<object> OnRowClicked { get; set; }

        [Parameter]
        public IEnumerable<Action<object>> OnRowClickedActions { get; set; }

        [Parameter]
        public IQueryDictionary<Type> CustomFilters { get; set; }

        [Parameter]
        public GridMode Mode { get; set; }

        [Parameter]
        public object[] Keys { get; set; }

        [Parameter]
        public string GridMvcCssClass { get; set; } = "grid-mvc";

        [Parameter]
        public string GridWrapCssClass { get; set; } = "grid-wrap container";

        [Parameter]
        public string GridFooterCssClass { get; set; } = "grid-footer";

        [Parameter]
        public string GridItemsCountCssClass { get; set; } = "grid-itemscount";

        [Parameter]
        public string TableCssClass { get; set; } = "table grid-table";
        [Parameter]
        public string TableWrapCssClass { get; set; } = "table-wrap";

        [Parameter]
        public string GridHeaderCssClass { get; set; } = "grid-header";

        [Parameter]
        public string GridCellCssClass { get; set; } = "grid-cell";

        [Parameter]
        public string GridButtonCellCssClass { get; set; } = "grid-button-cell";

        [Parameter]
        public string GridSubGridCssClass { get; set; } = "grid-subgrid";

        [Parameter]
        public string GridEmptyTextCssClass { get; set; } = "grid-empty-text";

        [Parameter]
        public ScreenPosition ScreenPosition { get; set; }

        protected override void OnParametersSet()
        {
            _filterComponents = new QueryDictionary<Type>();
            _filterComponents.Add("System.String", typeof(TextFilterComponent<T>));
            _filterComponents.Add("System.Guid", typeof(TextFilterComponent<T>));
            _filterComponents.Add("System.Int32", typeof(NumberFilterComponent<T>));
            _filterComponents.Add("System.Double", typeof(NumberFilterComponent<T>));
            _filterComponents.Add("System.Decimal", typeof(NumberFilterComponent<T>));
            _filterComponents.Add("System.Byte", typeof(NumberFilterComponent<T>));
            _filterComponents.Add("System.Single", typeof(NumberFilterComponent<T>));
            _filterComponents.Add("System.Float", typeof(NumberFilterComponent<T>));
            _filterComponents.Add("System.Int64", typeof(NumberFilterComponent<T>));
            _filterComponents.Add("System.Int16", typeof(NumberFilterComponent<T>));
            _filterComponents.Add("System.UInt64", typeof(NumberFilterComponent<T>));
            _filterComponents.Add("System.UInt32", typeof(NumberFilterComponent<T>));
            _filterComponents.Add("System.UInt16", typeof(NumberFilterComponent<T>));
            _filterComponents.Add("System.DateTime", typeof(DateTimeFilterComponent<T>));
            _filterComponents.Add("System.Date", typeof(DateTimeFilterComponent<T>));
            _filterComponents.Add("System.DateTimeOffset", typeof(DateTimeFilterComponent<T>));
            _filterComponents.Add("DateTimeLocal", typeof(DateTimeLocalFilterComponent<T>));
            _filterComponents.Add("Week", typeof(WeekFilterComponent<T>));
            _filterComponents.Add("Month", typeof(MonthFilterComponent<T>));
            _filterComponents.Add("System.Boolean", typeof(BooleanFilterComponent<T>));

            if (CustomFilters == null)
            {
                CustomFilters = new QueryDictionary<Type>();
            }
            if (CustomFilters.Any(r => r.Key.Equals(SelectItem.ListFilter)))
            {
                CustomFilters.Remove(SelectItem.ListFilter);
            }
            CustomFilters.Add(SelectItem.ListFilter, typeof(ListFilterComponent<T>));
            foreach (var widget in CustomFilters)
            {
                if (_filterComponents.ContainsKey(widget.Key))
                    _filterComponents[widget.Key] = widget.Value;
                else
                    _filterComponents.Add(widget);
            }

            FirstColumn = (IGridColumn<T>)Grid.Columns.FirstOrDefault();

            if(OnRowClickedActions != null && OnRowClickedActions.Count() > 0)
            {
                OnRowClicked = OnRowClickedActions.First();
            }

            _hasSubGrid = Grid.SubGridKeys != null && Grid.SubGridKeys.Length > 0;
            _hasTotals = Grid.IsSumEnabled || Grid.IsAverageEnabled || Grid.IsMaxEnabled || Grid.IsMinEnabled;
            _requiredTotalsColumn = _hasTotals
                && FirstColumn != null
                && (FirstColumn.IsSumEnabled || FirstColumn.IsAverageEnabled
                    || FirstColumn.IsMaxEnabled || FirstColumn.IsMinEnabled);

            HeaderCrudButtons = Grid.HeaderCrudButtons
                && Grid.ComponentOptions.Selectable
                && !Grid.ComponentOptions.MultiSelectable;

            HeaderComponents = new QueryDictionary<GridHeaderComponent<T>>();

            InitCheckboxAndSubGridVars();
            InitCheckedKeys();

            var queryBuilder = new CustomQueryStringBuilder(Grid.Settings.SearchSettings.Query);
            var exceptQueryParameters = new List<string> { GridPager.DefaultPageSizeQueryParameter };
            _changePageSizeUrl = queryBuilder.GetQueryStringExcept(exceptQueryParameters);
            _pageSize = Grid.Pager.ChangePageSize && Grid.Pager.QueryPageSize > 0 ? Grid.Pager.QueryPageSize : Grid.Pager.PageSize;

            _shouldRender = true;
        }

        private void InitCheckboxAndSubGridVars()
        {
            Checkboxes = new QueryDictionary<Dictionary<int, CheckboxComponent<T>>>();
            if (_hasSubGrid)
            {
                IsSubGridVisible = new bool[Grid.Pager.PageSize];
                for (int i = 0; i < IsSubGridVisible.Length; i++)
                {
                    IsSubGridVisible[i] = false;
                }
            }
            if (_hasSubGrid)
            {
                InitSubGrid = new bool[Grid.Pager.PageSize];
                for (int i = 0; i < InitSubGrid.Length; i++)
                {
                    InitSubGrid[i] = true;
                }
            }
        }

        private void InitCheckedKeys()
        {
            // checked keys must be initialized only on component creation or after a filter or search change
            ExceptCheckedRows = new QueryDictionary<QueryDictionary<bool>>();
        }

        protected override bool ShouldRender()
        {
            return _shouldRender;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _isDateTimeLocalSupported = await jSRuntime.InvokeAsync<bool>("gridJsFunctions.isDateTimeLocalSupported");
                _isWeekSupported = await jSRuntime.InvokeAsync<bool>("gridJsFunctions.isWeekSupported");
                _isMonthSupported = await jSRuntime.InvokeAsync<bool>("gridJsFunctions.isMonthSupported");
            }
            
            if ((firstRender || _fromCrud) && gridmvc.Id != null && Grid.Keyboard)
            {
                _fromCrud = false;
                await SetFocus(gridmvc);
            }

            if (ScreenPosition == null && firstRender && gridmvc.Id != null)
            {
                ScreenPosition = await jSRuntime.InvokeAsync<ScreenPosition>("gridJsFunctions.getPosition", gridmvc);
            }

            if (Grid.ComponentOptions.Selectable && Grid.ComponentOptions.InitSelection
                && Grid.ItemsToDisplay.Count() > 0 && SelectedRow == -1 && SelectedRows.Count == 0)
            {
                MouseEventArgs mouseEventArgs;
                if (Grid.ModifierKey == ModifierKey.CtrlKey)
                    mouseEventArgs = new MouseEventArgs { CtrlKey = false };
                else if (Grid.ModifierKey == ModifierKey.AltKey)
                    mouseEventArgs = new MouseEventArgs { AltKey = false };
                else if (Grid.ModifierKey == ModifierKey.ShiftKey)
                    mouseEventArgs = new MouseEventArgs { ShiftKey = false };
                else if (Grid.ModifierKey == ModifierKey.MetaKey)
                    mouseEventArgs = new MouseEventArgs { MetaKey = false };
                else
                    mouseEventArgs = new MouseEventArgs { CtrlKey = false };

                RowClicked(0, Grid.ItemsToDisplay.First(), mouseEventArgs) ;
            }

            if (firstRender)
            {
                await GoToCrudView();
            }

            _shouldRender = false;

            if(((CGrid<T>)Grid).OnAfterRender != null)
                await ((CGrid<T>)Grid).OnAfterRender.Invoke(this, firstRender);
        }

        private async Task GoToCrudView()
        {
            if (Keys != null)
            {
                if (Mode == GridMode.Create)
                {
                    await CreateHandler();
                }
                else if (Mode == GridMode.Read)
                {
                    var item = await ((CGrid<T>)Grid).CrudDataService.Get(Keys);
                    ReadHandler(item);
                }
                else if (Mode == GridMode.Update)
                {
                    await UpdateHandler(Keys);
                }
                else if (Mode == GridMode.Delete)
                {
                    var item = await ((CGrid<T>)Grid).CrudDataService.Get(Keys);
                    DeleteHandler(item);
                }
            }
        }

        internal void RowClicked(int i, object item, MouseEventArgs args)
        {
            //If user clicked on a row withouth Control key, unselect all rows
            if ((Grid.ModifierKey == ModifierKey.CtrlKey && !args.CtrlKey)
                || (Grid.ModifierKey == ModifierKey.AltKey && !args.AltKey)
                || (Grid.ModifierKey == ModifierKey.ShiftKey && !args.ShiftKey)
                || (Grid.ModifierKey == ModifierKey.MetaKey && !args.MetaKey))
            {
                SelectedRows.Clear();
                Grid.SelectedItems = new List<object>();
            }

            //If Grid is MultiSelectable, add selected row to list of rows
            if (Grid.ComponentOptions.MultiSelectable)
            {
                SelectedRow = -1;
                //If selected row is already part of collection, remove it
                if (SelectedRows.Contains(i))
                {
                    SelectedRows.Remove(i);
                    Grid.SelectedItems = Grid.SelectedItems.Except(new[] { item });
                }
                else
                {
                    SelectedRows.Add(i);
                    Grid.SelectedItems = Grid.SelectedItems.Concat(new[] { item });
                }
            }
            else
            {
                SelectedRow = i;
                Grid.SelectedItems = Grid.SelectedItems.Concat(new[] { item });
            }
            if (OnRowClicked != null)
                OnRowClicked.Invoke(item);

            _shouldRender = true;
            StateHasChanged();
        }

        public void SelectRow(int i)
        {
            MouseEventArgs mouseEventArgs;
            if (Grid.ModifierKey == ModifierKey.CtrlKey)
                mouseEventArgs = new MouseEventArgs { CtrlKey = false };
            else if (Grid.ModifierKey == ModifierKey.AltKey)
                mouseEventArgs = new MouseEventArgs { AltKey = false };
            else if (Grid.ModifierKey == ModifierKey.ShiftKey)
                mouseEventArgs = new MouseEventArgs { ShiftKey = false };
            else if (Grid.ModifierKey == ModifierKey.MetaKey)
                mouseEventArgs = new MouseEventArgs { MetaKey = false };
            else
                mouseEventArgs = new MouseEventArgs { CtrlKey = false };

            RowClicked(i, Grid.ItemsToDisplay.ElementAt(i), mouseEventArgs);
        }

        public async Task GoTo(int page)
        {
            if (Grid.ServerAPI == ServerAPI.OData)
                ((GridPager)Grid.Pager).CurrentPage = page;
            Grid.AddQueryParameter(((GridPager)Grid.Pager).ParameterName, page.ToString());
            await UpdateGrid();

            if (Grid.ComponentOptions.Selectable && Grid.ComponentOptions.InitSelection
                && Grid.ItemsToDisplay.Count() > 0 && SelectedRow == -1 && SelectedRows.Count == 0)
            {
                MouseEventArgs mouseEventArgs;
                if (Grid.ModifierKey == ModifierKey.CtrlKey)
                    mouseEventArgs = new MouseEventArgs { CtrlKey = false };
                else if (Grid.ModifierKey == ModifierKey.AltKey)
                    mouseEventArgs = new MouseEventArgs { AltKey = false };
                else if (Grid.ModifierKey == ModifierKey.ShiftKey)
                    mouseEventArgs = new MouseEventArgs { ShiftKey = false };
                else if (Grid.ModifierKey == ModifierKey.MetaKey)
                    mouseEventArgs = new MouseEventArgs { MetaKey = false };
                else
                    mouseEventArgs = new MouseEventArgs { CtrlKey = false };

                RowClicked(0, Grid.ItemsToDisplay.First(), mouseEventArgs);
            }

            await OnPagerChanged();
        }

        protected virtual async Task OnPagerChanged()
        {
            PagerEventArgs args = new PagerEventArgs();
            PagerDTO pagerDTO = new PagerDTO(Grid.EnablePaging, Grid.Pager.PageSize, Grid.Pager.CurrentPage, 
                Grid.Pager.ItemsCount);
            args.Pager = pagerDTO;

            if (PagerChanged != null)
            {
                await PagerChanged.Invoke(this, args);
            }
        }

        public async Task GetSortUrl(string columnQueryParameterName, string columnName,
            string directionQueryParameterName, string direction)
        {
            Grid.AddQueryParameter(columnQueryParameterName, columnName);
            Grid.AddQueryParameter(directionQueryParameterName, direction);
            await UpdateGrid();
            await OnSortChanged();
        }

        protected virtual async Task OnSortChanged()
        {
            SortEventArgs args = new SortEventArgs();
            args.ColumnName = Grid.Settings.SortSettings.ColumnName;
            args.Direction = Grid.Settings.SortSettings.Direction;

            if (SortChanged != null)
            {
                await SortChanged.Invoke(this, args);
            }
        }

        public async Task AddFilter(IGridColumn column, FilterCollection filters)
        {
            bool isValid = await OnBeforeFilterChanged();
            if (isValid)
            {
                Grid.AddFilterParameter(column, filters);
                await UpdateGrid();
                await OnFilterChanged();
            }      
        }

        protected virtual async Task<bool> OnBeforeFilterChanged()
        {
            FilterEventArgs args = new FilterEventArgs();
            args.FilteredColumns = Grid.Settings.FilterSettings.FilteredColumns;

            if (BeforeFilterChanged != null)
            {
                return await BeforeFilterChanged.Invoke(this, args);
            }
            return true;
        }
        
        protected virtual async Task OnFilterChanged()
        {
            // Filter changes must not init checked keys
            //InitCheckedKeys();

            FilterEventArgs args = new FilterEventArgs();
            args.FilteredColumns = Grid.Settings.FilterSettings.FilteredColumns;

            if (FilterChanged != null)
            {
                await FilterChanged.Invoke(this, args);
            }
        }

        public async Task RemoveFilter(IGridColumn column)
        {
            bool isValid = await OnBeforeFilterChanged();
            if (isValid)
            {
                Grid.RemoveFilterParameter(column);
                await UpdateGrid();
                await OnFilterChanged();
            }
        }

        public async Task RemoveAllFilters()
        {
            bool isValid = await OnBeforeFilterChanged();
            if (isValid)
            {
                Grid.RemoveAllFilters();
                await UpdateGrid();
                await OnFilterChanged();
            }
        }

        public async Task AddSearch(string searchValue)
        {
            Grid.AddQueryParameter(QueryStringSearchSettings.DefaultSearchQueryParameter, searchValue);
            await UpdateGrid();
            await OnSearchChanged();
        }

        protected virtual async Task OnSearchChanged()
        {
            // Filter changes must not init checked keys
            //InitCheckedKeys();

            SearchEventArgs args = new SearchEventArgs();
            args.SearchValue = Grid.Settings.SearchSettings.SearchValue;
            
            if (SearchChanged != null)
            {
                await SearchChanged.Invoke(this, args);
            }
        }

        public async Task RemoveSearch()
        {
            Grid.RemoveQueryParameter(QueryStringSearchSettings.DefaultSearchQueryParameter);
            await UpdateGrid();
            await OnSearchChanged();
        }

        public async Task AddExtSorting()
        {
            Grid.AddQueryString(ColumnOrderValue.DefaultSortingQueryParameter, Payload.ToString());
            await UpdateGrid();
            await OnExtSortChanged();
        }

        protected virtual async Task OnExtSortChanged()
        {
            ExtSortEventArgs args = new ExtSortEventArgs();
            args.SortValues = Grid.Settings.SortSettings.SortValues;
            
            if (ExtSortChanged != null)
            {
                await ExtSortChanged.Invoke(this, args);
            }
        }

        public async Task ChangeExtSorting(ColumnOrderValue column)
        {
            var newColumnOrderValue = new ColumnOrderValue
            {
                ColumnName = column.ColumnName,
                Direction = column.Direction == GridSortDirection.Ascending ? GridSortDirection.Descending
                    : GridSortDirection.Ascending,
                Id = column.Id
            };
            Grid.ChangeQueryString(ColumnOrderValue.DefaultSortingQueryParameter, column.ToString(),
                newColumnOrderValue.ToString());
            await UpdateGrid();

            await OnExtSortChanged();
        }

        public async Task RemoveExtSorting(ColumnOrderValue column)
        {
            Grid.RemoveQueryString(ColumnOrderValue.DefaultSortingQueryParameter, column.ToString());
            await UpdateGrid();
            await OnExtSortChanged();
        }

        internal void FilterIconClicked()
        {
            FilterButtonClicked.Invoke();
        }

        public async Task CreateHandler()
        {
            await SetSelectFields();
            _item = (T)Activator.CreateInstance(typeof(T));
            if (Grid.FixedValues != null)
            {
                foreach (var fixValue in Grid.FixedValues)
                {
                    _item.GetType().GetProperty(fixValue.Key).SetValue(_item, fixValue.Value);
                }
            }
            ((CGrid<T>)Grid).Mode = GridMode.Create;
            if (Grid.CreateComponent != null)
                CrudRender = CreateCrudComponent();
            else
                CrudRender = null;

            _shouldRender = true;
            StateHasChanged();
        }

        public void ReadHandler(object item)
        {
            _item = (T)item;
            ((CGrid<T>)Grid).Mode = GridMode.Read;
            if (Grid.ReadComponent != null)
                CrudRender = ReadCrudComponent();
            else
                CrudRender = null;

            _shouldRender = true;
            StateHasChanged();
        }

        public void ReadSelectedHandler()
        {
            if (SelectedRow != -1)
            {
                var item = Grid.ItemsToDisplay.ElementAt(SelectedRow);
                ReadHandler(item);
            }
            else
                ShowError(Strings.SelectionReadError);
        }

        public void ShowError(string error)
        {
            Error = error;
            var timer = new Timer((_) => {
                InvokeAsync(OnTimerEvent);
            }, null, 3000, -1);
            _shouldRender = true;
        }

        private void OnTimerEvent()
        {
            Error = "";
            _shouldRender = true;
            StateHasChanged();
        }

        public async Task UpdateHandler(object item)
        {
            var keys = Grid.GetPrimaryKeyValues(item);
            await UpdateHandler(keys);
        }

        public async Task UpdateHandler(object[] keys)
        {
            await SetSelectFields();
            try
            {
                _item = await ((CGrid<T>)Grid).CrudDataService.Get(keys);
                ((CGrid<T>)Grid).Mode = GridMode.Update;
                if (Grid.UpdateComponent != null)
                    CrudRender = UpdateCrudComponent();
                else
                    CrudRender = null;

                _shouldRender = true;
                StateHasChanged();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ((CGrid<T>)Grid).Mode = GridMode.Grid;
                throw;
            }
        }

        public async Task UpdateSelectedHandler()
        {
            if (SelectedRow != -1)
            {
                var item = Grid.ItemsToDisplay.ElementAt(SelectedRow);
                await UpdateHandler(item);
            }
            else
                ShowError(Strings.SelectionUpdateError);
        }

        public void DeleteHandler(object item)
        {
            _item = (T)item;
            ((CGrid<T>)Grid).Mode = GridMode.Delete;
            if (Grid.DeleteComponent != null)
                CrudRender = DeleteCrudComponent();
            else
                CrudRender = null;

            _shouldRender = true;
            StateHasChanged();
        }

        public void DeleteSelectedHandler()
        {
            if (SelectedRow != -1)
            {
                var item = Grid.ItemsToDisplay.ElementAt(SelectedRow);
                DeleteHandler(item);
            }
            else
                ShowError(Strings.SelectionDeleteError);
        }

        public async Task ExcelHandler()
        {
            if(string.IsNullOrWhiteSpace(Grid.ExcelExportFileName))
                await Grid.DownloadExcel(jSRuntime, Grid.ComponentOptions.GridName + ".xlsx");
            else
                await Grid.DownloadExcel(jSRuntime, Grid.ExcelExportFileName + ".xlsx");
        }

        public void ButtonComponentHandler(string key)
        {
            var buttonComponent = Grid.ButtonComponents.Get(key);
            StartFormComponent(buttonComponent.Label, buttonComponent.ComponentType, buttonComponent.Actions,
                    buttonComponent.Functions, buttonComponent.Object);
        }

        public void StartFormComponent<TFormComponent>(string label, IList<Action<object>> actions,
            IList<Func<object, Task>> functions, object obj)
        {
            StartFormComponent(label, typeof(TFormComponent), actions, functions, obj);
        }

        public void StartFormComponent(string label, Type componentType, IList<Action<object>> actions, 
            IList<Func<object, Task>> functions, object obj)
        {
            ((CGrid<T>)Grid).Mode = GridMode.Form;
            if (componentType != null)
                CrudRender = FormComponent(label, componentType, actions, functions, obj);
            else
                CrudRender = null;

            _shouldRender = true;
            StateHasChanged();
        }

        protected async Task SetSelectFields()
        {
            foreach (var column in Grid.Columns)
            {
                var isSelectField = ((IGridColumn<T>)column).IsSelectField;
                if (isSelectField.IsSelectKey)
                {
                    try
                    {
                        if (isSelectField.SelectItemExpr != null)
                        {
                            ((IGridColumn<T>)column).SelectItems = isSelectField.SelectItemExpr.Invoke();
                        }
                        else if (isSelectField.SelectItemExprAsync != null)
                        {
                            ((IGridColumn<T>)column).SelectItems = await isSelectField.SelectItemExprAsync.Invoke();
                        }
                        else
                        {
                            var selectItems = await Grid.HttpClient.GetFromJsonAsync<SelectItem[]>(isSelectField.Url);
                            ((IGridColumn<T>)column).SelectItems = selectItems.ToList();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        ((IGridColumn<T>)column).SelectItems = new List<SelectItem>();
                    }
                }
            }
        }

        protected RenderFragment CreateCrudComponent() => builder =>
        {
            builder.OpenComponent<CascadingValue<GridComponent<T>>>(++_sequence);
            builder.AddAttribute(++_sequence, "Value", this);
            builder.AddAttribute(++_sequence, "Name", "GridComponent");
            builder.AddAttribute(++_sequence, "ChildContent", CreateCrudChildComponent());
            builder.CloseComponent();
        };

        private RenderFragment CreateCrudChildComponent() => builder =>
        {
            var componentType = Grid.CreateComponent;
            builder.OpenComponent(++_sequence, componentType);
            builder.AddAttribute(++_sequence, "Item", _item);
            var gridProperty = componentType.GetProperty("Grid");
            if (gridProperty != null && gridProperty.PropertyType == typeof(CGrid<T>))
                builder.AddAttribute(++_sequence, "Grid", (CGrid<T>)Grid);
            gridProperty = componentType.GetProperty("Actions");
            if (gridProperty != null)
                builder.AddAttribute(++_sequence, "Actions", Grid.CreateActions);
            gridProperty = componentType.GetProperty("Functions");
            if (gridProperty != null)
                builder.AddAttribute(++_sequence, "Functions", Grid.CreateFunctions);
            gridProperty = componentType.GetProperty("Object");
            if (gridProperty != null)
                builder.AddAttribute(++_sequence, "Object", Grid.CreateObject);
            builder.CloseComponent();
        };

        protected RenderFragment ReadCrudComponent() => builder =>
        {
            builder.OpenComponent<CascadingValue<GridComponent<T>>>(++_sequence);
            builder.AddAttribute(++_sequence, "Value", this);
            builder.AddAttribute(++_sequence, "Name", "GridComponent");
            builder.AddAttribute(++_sequence, "ChildContent", ReadCrudChildComponent());
            builder.CloseComponent();
        };

        private RenderFragment ReadCrudChildComponent() => builder =>
        {
            var componentType = Grid.ReadComponent;
            builder.OpenComponent(++_sequence, componentType);
            builder.AddAttribute(++_sequence, "Item", _item);
            var gridProperty = componentType.GetProperty("Grid");
            if (gridProperty != null && gridProperty.PropertyType == typeof(CGrid<T>))
                builder.AddAttribute(++_sequence, "Grid", (CGrid<T>)Grid);
            gridProperty = componentType.GetProperty("Actions");
            if (gridProperty != null)
                builder.AddAttribute(++_sequence, "Actions", Grid.ReadActions);
            gridProperty = componentType.GetProperty("Functions");
            if (gridProperty != null)
                builder.AddAttribute(++_sequence, "Functions", Grid.ReadFunctions);
            gridProperty = componentType.GetProperty("Object");
            if (gridProperty != null)
                builder.AddAttribute(++_sequence, "Object", Grid.ReadObject);
            builder.CloseComponent();
        };

        protected RenderFragment UpdateCrudComponent() => builder =>
        {
            builder.OpenComponent<CascadingValue<GridComponent<T>>>(++_sequence);
            builder.AddAttribute(++_sequence, "Value", this);
            builder.AddAttribute(++_sequence, "Name", "GridComponent");
            builder.AddAttribute(++_sequence, "ChildContent", UpdateCrudChildComponent());
            builder.CloseComponent();
        };

        private RenderFragment UpdateCrudChildComponent() => builder =>
        {
            var componentType = Grid.UpdateComponent;
            builder.OpenComponent(++_sequence, componentType);
            builder.AddAttribute(++_sequence, "Item", _item);
            var gridProperty = componentType.GetProperty("Grid");
            if (gridProperty != null && gridProperty.PropertyType == typeof(CGrid<T>))
                builder.AddAttribute(++_sequence, "Grid", (CGrid<T>)Grid);
            gridProperty = componentType.GetProperty("Actions");
            if (gridProperty != null)
                builder.AddAttribute(++_sequence, "Actions", Grid.UpdateActions);
            gridProperty = componentType.GetProperty("Functions");
            if (gridProperty != null)
                builder.AddAttribute(++_sequence, "Functions", Grid.UpdateFunctions);
            gridProperty = componentType.GetProperty("Object");
            if (gridProperty != null)
                builder.AddAttribute(++_sequence, "Object", Grid.UpdateObject);
            builder.CloseComponent();
        };

        protected RenderFragment DeleteCrudComponent() => builder =>
        {
            builder.OpenComponent<CascadingValue<GridComponent<T>>>(++_sequence);
            builder.AddAttribute(++_sequence, "Value", this);
            builder.AddAttribute(++_sequence, "Name", "GridComponent");
            builder.AddAttribute(++_sequence, "ChildContent", DeleteCrudChildComponent());
            builder.CloseComponent();
        };

        private RenderFragment DeleteCrudChildComponent() => builder =>
        {
            var componentType = Grid.DeleteComponent;
            builder.OpenComponent(++_sequence, componentType);
            builder.AddAttribute(++_sequence, "Item", _item);
            var gridProperty = componentType.GetProperty("Grid");
            if (gridProperty != null && gridProperty.PropertyType == typeof(CGrid<T>))
                builder.AddAttribute(++_sequence, "Grid", (CGrid<T>)Grid);
            gridProperty = componentType.GetProperty("Actions");
            if (gridProperty != null)
                builder.AddAttribute(++_sequence, "Actions", Grid.DeleteActions);
            gridProperty = componentType.GetProperty("Functions");
            if (gridProperty != null)
                builder.AddAttribute(++_sequence, "Functions", Grid.DeleteFunctions);
            gridProperty = componentType.GetProperty("Object");
            if (gridProperty != null)
                builder.AddAttribute(++_sequence, "Object", Grid.DeleteObject);
            builder.CloseComponent();
        };

        protected RenderFragment FormComponent(string label, Type componentType, IList<Action<object>> actions, 
            IList<Func<object, Task>> functions, object obj) => builder =>
        {
            builder.OpenComponent<CascadingValue<GridComponent<T>>>(++_sequence);
            builder.AddAttribute(++_sequence, "Value", this);
            builder.AddAttribute(++_sequence, "Name", "GridComponent");
            builder.AddAttribute(++_sequence, "ChildContent", FormChildComponent(label, componentType, actions,
                functions, obj));
            builder.CloseComponent();
        };

        private RenderFragment FormChildComponent(string label, Type componentType, IList<Action<object>> actions,
            IList<Func<object, Task>> functions, object obj) => builder =>
        {
            builder.OpenComponent(++_sequence, componentType);
            var gridProperty = componentType.GetProperty("Grid");
            if (gridProperty != null && gridProperty.PropertyType == typeof(CGrid<T>))
                builder.AddAttribute(++_sequence, "Grid", (CGrid<T>)Grid);
            gridProperty = componentType.GetProperty("Label");
            if (gridProperty != null)
                builder.AddAttribute(++_sequence, "Label", label);
            gridProperty = componentType.GetProperty("Actions");
            if (gridProperty != null)
                builder.AddAttribute(++_sequence, "Actions", actions);
            gridProperty = componentType.GetProperty("Functions");
            if (gridProperty != null)
                builder.AddAttribute(++_sequence, "Functions", functions);
            gridProperty = componentType.GetProperty("Object");
            if (gridProperty != null)
                builder.AddAttribute(++_sequence, "Object", obj);
            builder.CloseComponent();
        };

        public void BackButton()
        {
            ((CGrid<T>)Grid).Mode = GridMode.Grid;
            CrudRender = null;
            _fromCrud = true;

            _shouldRender = true;
            StateHasChanged();
        }

        public async Task CreateItem(GridCreateComponent<T> component)
        {
            try
            {
                bool isValid = await OnBeforeInsert(component);
                if (isValid)
                {
                    if (Grid.ServerAPI == ServerAPI.OData)
                        _item = await ((ICrudODataService<T>)((CGrid<T>)Grid).CrudDataService).Add(_item);
                    else
                        await ((CGrid<T>)Grid).CrudDataService.Insert(_item);
                    if(((CGrid<T>)Grid).CrudFileService != null)
                        await ((CGrid<T>)Grid).CrudFileService.InsertFiles(_item, component.Files);
                    await OnAfterInsert(component);
                    ((CGrid<T>)Grid).Mode = GridMode.Grid;
                    CrudRender = null;
                    _fromCrud = true;
                    await UpdateGrid();
                }     
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        protected virtual async Task<bool> OnBeforeInsert(GridCreateComponent<T> component)
        {
            if (BeforeInsert != null)
            {
                return await BeforeInsert.Invoke(component, _item);
            }
            return true;
        }

        protected virtual async Task OnAfterInsert(GridCreateComponent<T> component)
        {
            if (AfterInsert != null)
            {
                await AfterInsert.Invoke(component, _item);
            }
        }

        public async Task UpdateItem(GridUpdateComponent<T> component)
        {
            try
            {
                bool isValid = await OnBeforeUpdate(component);
                if (isValid)
                {
                    if (((CGrid<T>)Grid).CrudFileService != null)
                        _item = await((CGrid<T>)Grid).CrudFileService.UpdateFiles(_item, component.Files);
                    await ((CGrid<T>)Grid).CrudDataService.Update(_item);
                    await OnAfterUpdate(component);
                    ((CGrid<T>)Grid).Mode = GridMode.Grid;
                    CrudRender = null;
                    _fromCrud = true;
                    await UpdateGrid();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        protected virtual async Task<bool> OnBeforeUpdate(GridUpdateComponent<T> component)
        {
            if (BeforeUpdate != null)
            {
                return await BeforeUpdate.Invoke(component, _item);
            }
            return true;
        }

        protected virtual async Task OnAfterUpdate(GridUpdateComponent<T> component)
        {
            if (AfterUpdate != null)
            {
                await AfterUpdate.Invoke(component, _item);
            }
        }

        public async Task DeleteItem(GridDeleteComponent<T> component)
        {
            try
            {
                bool isValid = await OnBeforeDelete(component);
                if (isValid)
                {
                    var keys = Grid.GetPrimaryKeyValues(_item);
                    if (((CGrid<T>)Grid).CrudFileService != null)
                        await ((CGrid<T>)Grid).CrudFileService.DeleteFiles(keys);
                    await ((CGrid<T>)Grid).CrudDataService.Delete(keys);
                    await OnAfterDelete(component);
                    ((CGrid<T>)Grid).Mode = GridMode.Grid;
                    CrudRender = null;
                    _fromCrud = true;
                    await UpdateGrid();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                throw;
            }
        }

        protected virtual async Task<bool> OnBeforeDelete(GridDeleteComponent<T> component)
        {
            if (BeforeDelete != null)
            {
                return await BeforeDelete.Invoke(component, _item);
            }
            return true;
        }

        protected virtual async Task OnAfterDelete(GridDeleteComponent<T> component)
        {
            if (AfterDelete != null)
            {
                await AfterDelete.Invoke(component, _item);
            }
        }

        public async Task InputPageSizeKeyup(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                await InputPageSizeBlur();
            }
        }

        public async Task InputPageSizeBlur()
        {
            if (_pageSize > 0)
            {
                Grid.Pager.PageSize = _pageSize;
                Grid.AddQueryParameter(GridPager.DefaultPageSizeQueryParameter, _pageSize.ToString());
                await UpdateGrid();
                await OnPagerChanged();
            }
            else
            {
                _pageSize = Grid.Pager.PageSize;
                _shouldRender = true;
                StateHasChanged();
            }
        }

        public async Task ChangePageSize(int pageSize)
        {
            if (pageSize > 0)
            {
                _pageSize = pageSize;
                Grid.Pager.PageSize = pageSize;
                Grid.AddQueryParameter(GridPager.DefaultPageSizeQueryParameter, pageSize.ToString());
                await UpdateGrid();
                await OnPagerChanged();
            }
        }

        public async Task SetGridFocus()
        {
            if (gridmvc.Id != null && Grid.Keyboard)
            {
                await SetFocus(gridmvc);
            }
        }

        public async Task SetFocus(ElementReference element)
        {
            await jSRuntime.InvokeVoidAsync("gridJsFunctions.focusElement", element);
        }

        public async Task GridComponentKeyup(KeyboardEventArgs e)
        {
            if (Grid.Keyboard
                && ((Grid.ModifierKey == ModifierKey.CtrlKey && e.CtrlKey)
                || (Grid.ModifierKey == ModifierKey.AltKey && e.AltKey)
                || (Grid.ModifierKey == ModifierKey.ShiftKey && e.ShiftKey)
                || (Grid.ModifierKey == ModifierKey.MetaKey && e.MetaKey)))
            {
                if (e.Key == "ArrowLeft" && Grid.Pager.CurrentPage > 1)
                {
                    await GoTo(Grid.Pager.CurrentPage - 1);
                }
                else if (e.Key == "ArrowRight" && Grid.Pager.CurrentPage < ((GridPager)Grid.Pager).PageCount)
                {
                    await GoTo(Grid.Pager.CurrentPage + 1);
                }
                else if (e.Key == "Home")
                {
                    await GoTo(1);
                }
                else if (e.Key == "End")
                {
                    await GoTo(((GridPager)Grid.Pager).PageCount);
                }
                else if (e.Key == "ArrowUp" && Grid.ComponentOptions.Selectable && SelectedRow > 0)
                {
                    MouseEventArgs mouseEventArgs;
                    if (Grid.ModifierKey == ModifierKey.CtrlKey)
                        mouseEventArgs = new MouseEventArgs { CtrlKey = false };
                    else if (Grid.ModifierKey == ModifierKey.AltKey)
                        mouseEventArgs = new MouseEventArgs { AltKey = false };
                    else if (Grid.ModifierKey == ModifierKey.ShiftKey)
                        mouseEventArgs = new MouseEventArgs { ShiftKey = false };
                    else if (Grid.ModifierKey == ModifierKey.MetaKey)
                        mouseEventArgs = new MouseEventArgs { MetaKey = false };
                    else
                        mouseEventArgs = new MouseEventArgs { CtrlKey = false };

                    int selectedRow = SelectedRow - 1;
                    RowClicked(selectedRow, Grid.ItemsToDisplay.ElementAt(selectedRow), mouseEventArgs);
                }
                else if (e.Key == "ArrowDown" && Grid.ComponentOptions.Selectable && SelectedRow != -1 && SelectedRow < Grid.DisplayingItemsCount - 1)
                {
                    MouseEventArgs mouseEventArgs;
                    if (Grid.ModifierKey == ModifierKey.CtrlKey)
                        mouseEventArgs = new MouseEventArgs { CtrlKey = false };
                    else if (Grid.ModifierKey == ModifierKey.AltKey)
                        mouseEventArgs = new MouseEventArgs { AltKey = false };
                    else if (Grid.ModifierKey == ModifierKey.ShiftKey)
                        mouseEventArgs = new MouseEventArgs { ShiftKey = false };
                    else if (Grid.ModifierKey == ModifierKey.MetaKey)
                        mouseEventArgs = new MouseEventArgs { MetaKey = false };
                    else
                        mouseEventArgs = new MouseEventArgs { CtrlKey = false };

                    int selectedRow = SelectedRow + 1;
                    RowClicked(selectedRow, Grid.ItemsToDisplay.ElementAt(selectedRow), mouseEventArgs);
                }
                else if (e.Key == "Backspace" && Grid.ClearFiltersButtonEnabled)
                {
                    await RemoveAllFilters();
                }
            }
        }

        internal virtual async Task OnHeaderCheckboxChanged(CheckboxEventArgs<T> args)
        {
            if (HeaderCheckboxChanged != null)
            {
                await HeaderCheckboxChanged.Invoke(args);
            }
        }

        internal virtual async Task OnRowCheckboxChanged(CheckboxEventArgs<T> args)
        {
            if (RowCheckboxChanged != null)
            {
                await RowCheckboxChanged.Invoke(args);
            }
        }

        public async Task UpdateGrid(bool ReloadData = true)
        {
            await OnBeforeRefreshGrid();
            
            if (ReloadData) await Grid.UpdateGrid();
            SelectedRow = -1;
            SelectedRows.Clear();
            InitCheckboxAndSubGridVars();

            _shouldRender = true;
            StateHasChanged();

            await SetGridFocus();

            await OnAfterRefreshGrid();
        }

        protected virtual async Task OnBeforeRefreshGrid()
        {
            if (BeforeRefreshGrid != null)
            {
                await BeforeRefreshGrid.Invoke();
            }
        }

        protected virtual async Task OnAfterRefreshGrid()
        {
            if (AfterRefreshGrid != null)
            {
                await AfterRefreshGrid.Invoke();
            }
        }
    }
}
