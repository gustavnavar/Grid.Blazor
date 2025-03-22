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
using GridShared.Style;
using GridShared.Utility;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
#if ! NETSTANDARD2_1
using Microsoft.AspNetCore.Components.CompilerServices;
using Microsoft.AspNetCore.Components.Web.Virtualization;
#endif
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
        private IGridBlazorService? _gridBlazorService;
        private bool _fromCrud = false;
        private bool _shouldRender = false;
        internal bool HasSubGrid = false;
        internal bool HasTotals = false;
        internal bool RequiredTotalsColumn = false;
        private string gridTableHead = Guid.NewGuid().ToString("N");
        private string gridTableBody = Guid.NewGuid().ToString("N");
        private string gridTableTotals = Guid.NewGuid().ToString("N");
        internal string ChangePageSizeUrl;
        internal int PageSize;
        internal int VirtualizedHeight;
        internal bool[] IsSubGridVisible;
        internal bool[] InitSubGrid;
        protected IQueryDictionary<Type> _filterComponents;

        // browser input type support
        internal bool IsDateTimeLocalSupported = false;
        internal bool IsWeekSupported = false;
        internal bool IsMonthSupported = false;

        // CRUD buttons on the header
        internal bool HeaderCrudButtons = false;

        internal ElementReference Gridmvc;
        internal ElementReference GridTable;
        internal ElementReference GridTableWrap;

        internal ScreenPosition gridComponentSP;
        internal ScreenPosition gridTableSP;

        internal ElementReference Spinner;
        internal ElementReference Content;

        public event Func<object, SortEventArgs, Task> SortChanged;
        public event Func<object, ExtSortEventArgs, Task> ExtSortChanged;
        public event Func<object, FilterEventArgs, Task<bool>> BeforeFilterChanged;
        public event Func<object, FilterEventArgs, Task> FilterChanged;
        public event Func<object, SearchEventArgs, Task> SearchChanged;
        public event Func<object, PagerEventArgs, Task> PagerChanged;

        public event Func<GridComponent<T>, T, Task<bool>> BeforeCreateForm;
        public event Func<GridComponent<T>, T, Task> AfterCreateForm;
        public event Func<GridComponent<T>, T, Task<bool>> BeforeReadForm;
        public event Func<GridComponent<T>, T, Task> AfterReadForm;
        public event Func<GridComponent<T>, T, Task<bool>> BeforeUpdateForm;
        public event Func<GridComponent<T>, T, Task> AfterUpdateForm;
        public event Func<GridComponent<T>, T, Task<bool>> BeforeDeleteForm;
        public event Func<GridComponent<T>, T, Task> AfterDeleteForm;

        public event Func<GridComponent<T>, T, Task<bool>> BeforeBack;
        public event Func<GridComponent<T>, T, Task> AfterBack;

        public event Func<GridCreateComponent<T>, T, Task<bool>> BeforeInsert;
        public event Func<GridCreateComponent<T>, T, Task> AfterInsert;
        public event Func<GridUpdateComponent<T>, T, Task<bool>> BeforeUpdate;
        public event Func<GridUpdateComponent<T>, T, Task> AfterUpdate;
        public event Func<GridDeleteComponent<T>, T, Task<bool>> BeforeDelete;
        public event Func<GridDeleteComponent<T>, T, Task> AfterDelete;     

        public event Func<Task> BeforeRefreshGrid;
        public event Func<Task> AfterRefreshGrid;

        public event Func<CheckboxEventArgs<T>, Task> HeaderCheckboxChanged;
        public event Func<CheckboxEventArgs<T>, Task> RowCheckboxChanged;

        internal event Action FilterButtonClicked;

        [Inject]
        private IJSRuntime jSRuntime { get; set; }

        [Inject]
        private IServiceProvider ServiceProvider { get; set; }

        public int SelectedRow { get; internal set; } = -1;

        public List<int> SelectedRows { get; internal set; } = new List<int>();

        [Obsolete("This property is obsolete. Use the new Checkboxes parameter.", true)]
        public QueryDictionary<List<int>> CheckedRows { get; internal set; } = new QueryDictionary<List<int>>();

        public QueryDictionary<Dictionary<int, CheckboxComponent<T>>> Checkboxes { get; private set; }

        public QueryDictionary<QueryDictionary<bool>> ExceptCheckedRows { get; private set; } = new QueryDictionary<QueryDictionary<bool>>();

        public QueryDictionary<GridHeaderComponent<T>> HeaderComponents { get; private set; }

        public ElementReference PageSizeInput { get; internal set; }

        public GridSearchComponent<T> SearchComponent { get; private set; }

        public GridCreateComponent<T> CreateComponent { get; private set; }

        public GridReadComponent<T> ReadComponent { get; private set; }

        public GridUpdateComponent<T> UpdateComponent { get; private set; }

        public GridDeleteComponent<T> DeleteComponent { get; private set; }

        public GridCountComponent<T> CountComponent { get; private set; }

        public GridTotalsComponent<T> TotalsComponent { get; private set; }

        public T Item { get; protected set; }

        internal IGridColumn<T> FirstColumn { get; set; }

        internal ColumnOrderValue Payload { get; set; }

        protected RenderFragment CrudRender { get; set; }

        public string Error { 
            get { return Grid.Error; } 
            set { Grid.Error = value; } 
        }

        internal RenderFragment VirtualizedRenderFragment;
        internal VariableReference VirtualizedComponent;

        [Parameter]
        public ICGrid Grid { get; set; }

        [Parameter]
        public Action<object> OnRowClicked { get; set; }

        [Parameter]
        public IEnumerable<Action<object>> OnRowClickedActions { get; set; }

        [Parameter]
        public IQueryDictionary<Type> CustomFilters { get; set; }

        // Mode parameter is only used to get an intial value for the grid.
        // Grid.Mode is the attribute that really controls the grid component visualization mode 
        [Parameter]
        public GridMode Mode { get; set; }

        [Parameter]
        public object[] Keys { get; set; }

        [Parameter]
        public bool UseMemoryCrudDataService { get; set; } = false;

        [Parameter]
        public CssFramework? CssFramework { get; set; }

        [Parameter]
        public string GridMvcCssClass { get; set; } = "grid-mvc";

        [Parameter]
        public string GridWrapCssClass { get; set; } = "grid-wrap";

        [Parameter]
        public string GridErrorCssClass { get; set; } = "grid-error";

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
        public string GridCrudHeaderCssClass { get; set; } = "grid-crud-header";

        [Parameter]
        public string GridCellCssClass { get; set; } = "grid-cell";

        [Parameter]
        public string GridButtonCellCssClass { get; set; } = "grid-button-cell";

        [Parameter]
        public string GridSubGridCssClass { get; set; } = "grid-subgrid";

        [Parameter]
        public string GridEmptyTextCssClass { get; set; } = "grid-empty-text";

        [Parameter]
        public string GridSumCssClass { get; set; } = "grid-cell";

        [Parameter]
        public string GridAverageCssClass { get; set; } = "grid-cell";

        [Parameter]
        public string GridMaxCssClass { get; set; } = "grid-cell";

        [Parameter]
        public string GridMinCssClass { get; set; } = "grid-cell";

        [Parameter]
        public string GridCalculationCssClass { get; set; } = "grid-cell";


        protected override void OnInitialized()
        {
            _gridBlazorService = (IGridBlazorService)ServiceProvider.GetService(typeof(IGridBlazorService));
            base.OnInitialized();
        }

        protected override void OnParametersSet()
        {
            ((CGrid<T>)Grid).GridComponent = this;

            if (!CssFramework.HasValue)
            {
                if (_gridBlazorService != null)
                    CssFramework = _gridBlazorService.Style;
                else
                    CssFramework = GridShared.Style.CssFramework.Bootstrap_4;
            }
            Grid.CssFramework = CssFramework.Value;
            Grid.HtmlClass = new HtmlClass(CssFramework.Value);

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
#if ! NETSTANDARD2_1 && !NET5_0
            _filterComponents.Add("System.DateOnly", typeof(DateOnlyFilterComponent<T>));
            _filterComponents.Add("System.TimeOnly", typeof(TimeOnlyFilterComponent<T>));
#endif
            _filterComponents.Add("System.Boolean", typeof(BooleanFilterComponent<T>));
            _filterComponents.Add("System.Collections.Generic.ICollection`1", typeof(CollectionFilterComponent<T>));

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

            HasSubGrid = Grid.SubGridKeys != null && Grid.SubGridKeys.Length > 0;
            HasTotals = Grid.IsSumEnabled || Grid.IsAverageEnabled || Grid.IsMaxEnabled || Grid.IsMinEnabled;
            RequiredTotalsColumn = HasTotals
                && FirstColumn != null
                && (FirstColumn.IsSumEnabled || FirstColumn.IsAverageEnabled
                    || FirstColumn.IsMaxEnabled || FirstColumn.IsMinEnabled);

            HeaderCrudButtons = Grid.HeaderCrudButtons
                && Grid.ComponentOptions.Selectable
                && !Grid.ComponentOptions.MultiSelectable;

            HeaderComponents = new QueryDictionary<GridHeaderComponent<T>>();

            InitCheckboxAndSubGridVars();
            // checked keys are already initialized 
            //InitCheckedKeys();

            var queryBuilder = new CustomQueryStringBuilder(Grid.Settings.SearchSettings.Query);
            var exceptQueryParameters = new List<string> { GridPager.DefaultPageSizeQueryParameter };
            ChangePageSizeUrl = queryBuilder.GetQueryStringExcept(exceptQueryParameters);
            if (Grid.PagingType == PagingType.Pagination)
                PageSize = Grid.Pager.ChangePageSize && Grid.Pager.QueryPageSize > 0 ? Grid.Pager.QueryPageSize : Grid.Pager.PageSize;
            else if (Grid.PagingType == PagingType.Virtualization && Grid.ChangeVirtualizedHeight)
            {
                string height = Grid.Height.Remove(Grid.Height.IndexOf('p'));
                int.TryParse(height, out VirtualizedHeight);
            }

            if (UseMemoryCrudDataService && ((CGrid<T>)Grid).MemoryDataService != null)
            {
                ((CGrid<T>)Grid).CrudDataService = ((CGrid<T>)Grid).MemoryDataService;
                ((CGrid<T>)Grid).DataService = ((CGrid<T>)Grid).MemoryDataService.GetGridRows;
                ((CGrid<T>)Grid).DataServiceAsync = null;
                ((CGrid<T>)Grid).GrpcService = null;
            }

            if (Grid.PagingType == PagingType.Virtualization)
            {
                VirtualizedComponent = new VariableReference();
                VirtualizedRenderFragment = CreateVirtualComponent(Grid, VirtualizedComponent);
            }

            _shouldRender = true;
        }

        private void InitCheckboxAndSubGridVars()
        {
            Checkboxes = new QueryDictionary<Dictionary<int, CheckboxComponent<T>>>();
            if (HasSubGrid)
            {
                IsSubGridVisible = new bool[Grid.Pager.PageSize];
                for (int i = 0; i < IsSubGridVisible.Length; i++)
                {
                    IsSubGridVisible[i] = Grid.SubGridsOpened;
                }
            }
            if (HasSubGrid)
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
            gridComponentSP = await jSRuntime.InvokeAsync<ScreenPosition>("gridJsFunctions.getPosition", Gridmvc);
            gridTableSP = await jSRuntime.InvokeAsync<ScreenPosition>("gridJsFunctions.getPosition", GridTable);
            
            if (firstRender)
            {
                IsDateTimeLocalSupported = await jSRuntime.InvokeAsync<bool>("gridJsFunctions.isDateTimeLocalSupported");
                IsWeekSupported = await jSRuntime.InvokeAsync<bool>("gridJsFunctions.isWeekSupported");
                IsMonthSupported = await jSRuntime.InvokeAsync<bool>("gridJsFunctions.isMonthSupported");
                if (Grid.TableLayout != TableLayout.Auto)
                    await jSRuntime.InvokeVoidAsync("gridJsFunctions.scrollFixedSizeTable", gridTableHead, gridTableBody, gridTableTotals);
            }
            
            if ((firstRender || _fromCrud) && Gridmvc.Id != null && Grid.Keyboard)
            {
                _fromCrud = false;
                await SetFocus(Gridmvc);
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


        private RenderFragment CreateVirtualComponent(ICGrid grid, VariableReference reference) => builder =>
        {
#if NETSTANDARD2_1
            builder.OpenElement(0, "p");
            builder.AddContent(1, Strings.DefaultGridEmptyText);
            builder.CloseElement();
#else
            builder.OpenComponent<Virtualize<T>>(0);
            builder.AddAttribute(1, "ItemsProvider", (ItemsProviderDelegate<T>)((CGrid<T>)Grid).LoadItems);
            builder.AddAttribute(2, "ItemContent", (RenderFragment<T>)(item => builder1 => {
                builder1.OpenComponent<CascadingValue<GridComponent<T>>>(3);
                builder1.AddAttribute(4, "Value", this);
                builder1.AddAttribute(5, "Name", "GridComponent");
                builder1.AddAttribute(6, "ChildContent", (RenderFragment)(builder2 => {
                    builder2.OpenComponent<GridVirtualRowComponent<T>>(7);
                    builder2.AddAttribute(8, "Grid", RuntimeHelpers.TypeCheck<ICGrid>(Grid));
                    builder2.AddAttribute(9, "HasSubGrid", RuntimeHelpers.TypeCheck<Boolean>(HasSubGrid));
                    builder2.AddAttribute(10, "RequiredTotalsColumn", RuntimeHelpers.TypeCheck<Boolean>(RequiredTotalsColumn));
                    builder2.AddAttribute(11, "Item", RuntimeHelpers.TypeCheck<Object>(item));
                    builder2.CloseComponent();
                }));
                builder1.CloseComponent();
            }
            ));
            builder.AddAttribute(12, "Placeholder", (RenderFragment<PlaceholderContext>)(item => builder1 => {
                builder1.OpenElement(13, "p");
                builder1.AddContent(14, Strings.DefaultGridEmptyText);
                builder1.CloseElement();
            }
            ));
            builder.AddComponentReferenceCapture(15, r => reference.Variable = r);
            builder.CloseComponent();
#endif
        };

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
                    await ReadHandler(item);
                }
                else if (Mode == GridMode.Update)
                {
                    await UpdateHandler(Keys);
                }
                else if (Mode == GridMode.Delete)
                {
                    var item = await ((CGrid<T>)Grid).CrudDataService.Get(Keys);
                    await DeleteHandler(item);
                }
            }
        }

        internal void RowClicked(int i, object item, MouseEventArgs args)
        {
            //If user clicked on a row withouth Control key, unselect all rows
            if ((!args.CtrlKey && !args.AltKey && !args.ShiftKey && !args.MetaKey)
                || ( !((Grid.ModifierKey == ModifierKey.CtrlKey || Grid.SelectionKey == ModifierKey.CtrlKey) && args.CtrlKey)
                    && !((Grid.ModifierKey == ModifierKey.AltKey || Grid.SelectionKey == ModifierKey.AltKey) && args.AltKey)
                    && !((Grid.ModifierKey == ModifierKey.ShiftKey || Grid.SelectionKey == ModifierKey.ShiftKey) && args.ShiftKey)
                    && !((Grid.ModifierKey == ModifierKey.MetaKey || Grid.SelectionKey == ModifierKey.MetaKey) && args.MetaKey)))
            {
                SelectedRows.Clear();
                Grid.SelectedItems = new List<object>();
            }

            //If Grid is MultiSelectable, add selected row to list of rows
            if (Grid.ComponentOptions.MultiSelectable)
            {
                //Multiple row selection using the SHIFT key
                if ((Grid.SelectionKey == ModifierKey.CtrlKey && args.CtrlKey)
                    || (Grid.SelectionKey == ModifierKey.AltKey && args.AltKey)
                    || (Grid.SelectionKey == ModifierKey.ShiftKey && args.ShiftKey)
                    || (Grid.SelectionKey == ModifierKey.MetaKey && args.MetaKey))
                {
                    // second row selection
                    if (SelectedRow != -1)
                    {
                        if (i > SelectedRow)
                        {
                            for (int j = SelectedRow; j <= i; j++)
                            {
                                SelectedRows.Add(j);
                                Grid.SelectedItems = Grid.SelectedItems.Concat(new[] { Grid.ItemsToDisplay.ElementAt(j) });
                            }
                        }
                        else
                        {
                            for (int j = i; j <= SelectedRow; j++)
                            {
                                SelectedRows.Add(j);
                                Grid.SelectedItems = Grid.SelectedItems.Concat(new[] { Grid.ItemsToDisplay.ElementAt(j) });
                            }
                        }
                        //reset first row selection
                        SelectedRow = -1;
                    }
                    // first row selection
                    else
                    {
                        SelectedRow = i;
                        SelectedRows.Clear();
                        Grid.SelectedItems = new List<object>();
                    }
                }
                //Multiple row selection clicking one by one
                else
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
            PagerDTO pagerDTO = new PagerDTO(Grid.PagingType, Grid.Pager.PageSize, Grid.Pager.CurrentPage, Grid.Pager.ItemsCount, Grid.Pager.StartIndex, Grid.Pager.VirtualizedCount);
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

        public async Task GetSortUrl(string columnQueryParameterName, string directionQueryParameterName)
        {
            Grid.RemoveQueryParameter(columnQueryParameterName);
            Grid.RemoveQueryParameter(directionQueryParameterName);
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

        public async Task InitGrouping(IList<ColumnOrderValue> payloads)
        {
            await InitExtSorting(payloads);
        }

        public async Task InitExtSorting(IList<ColumnOrderValue> payloads)
        {
            foreach (var payload in payloads)
            {
                Payload = payload;
                Grid.AddQueryString(ColumnOrderValue.DefaultSortingQueryParameter, Payload.ToString());
            }
            await UpdateGrid();
            await OnExtSortChanged();
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
            Item = (T)Activator.CreateInstance(typeof(T));
            await CreateHndlr();
        }

        public async Task CreateHandler(T item)
        {
            await SetSelectFields();
            Item = item;
            await CreateHndlr();
        }

        private async Task CreateHndlr()
        {
            bool isValid = await OnBeforeCreateForm();
            if (isValid)
            {
                if (((ICGrid<T>)Grid).InitCreateValues != null)
                {
                    await ((ICGrid<T>)Grid).InitCreateValues(Item);
                }
                if (Grid.FixedValues != null)
                {
                    foreach (var fixValue in Grid.FixedValues)
                    {
                        Item.GetType().GetProperty(fixValue.Key).SetValue(Item, fixValue.Value);
                    }
                }
                ((CGrid<T>)Grid).Mode = GridMode.Create;
                if (Grid.CreateComponent != null)
                    CrudRender = CreateCrudComponent();
                else
                    CrudRender = null;

                await OnAfterCreateForm();

                _shouldRender = true;
                StateHasChanged();
            }
        }

        protected virtual async Task<bool> OnBeforeCreateForm()
        {
            if (BeforeCreateForm != null)
            {
                return await BeforeCreateForm.Invoke(this, Item);
            }
            return true;
        }

        protected virtual async Task OnAfterCreateForm()
        {
            if (AfterCreateForm != null)
            {
                await AfterCreateForm.Invoke(this, Item);
            }
        }

        public async Task ReadHandler(object item)
        {
            var keys = Grid.GetPrimaryKeyValues(item);
            await ReadHandler(keys);
        }

        public async Task ReadHandler(object[] keys)
        {
            try
            {
                Item = await ((CGrid<T>)Grid).CrudDataService.Get(keys);
                bool isValid = await OnBeforeReadForm();
                if (isValid)
                {
                    ((CGrid<T>)Grid).Mode = GridMode.Read;
                    if (Grid.ReadComponent != null)
                        CrudRender = ReadCrudComponent();
                    else
                        CrudRender = null;

                    await OnAfterReadForm();

                    _shouldRender = true;
                    StateHasChanged();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ((CGrid<T>)Grid).Mode = GridMode.Grid;
                throw;
            }
        }

        public async Task ReadSelectedHandler()
        {
            if (SelectedRow != -1)
            {
                var item = Grid.ItemsToDisplay.ElementAt(SelectedRow);
                await ReadHandler(item);
            }
            else
                ShowError(Strings.SelectionReadError);
        }

        protected virtual async Task<bool> OnBeforeReadForm()
        {
            if (BeforeReadForm != null)
            {
                return await BeforeReadForm.Invoke(this, Item);
            }
            return true;
        }

        protected virtual async Task OnAfterReadForm()
        {
            if (AfterReadForm != null)
            {
                await AfterReadForm.Invoke(this, Item);
            }
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
                Item = await ((CGrid<T>)Grid).CrudDataService.Get(keys);
                bool isValid = await OnBeforeUpdateForm();
                if (isValid)
                {
                    ((CGrid<T>)Grid).Mode = GridMode.Update;
                    if (Grid.UpdateComponent != null)
                        CrudRender = UpdateCrudComponent();
                    else
                        CrudRender = null;

                    await OnAfterUpdateForm();

                    _shouldRender = true;
                    StateHasChanged();
                }
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

        protected virtual async Task<bool> OnBeforeUpdateForm()
        {
            if (BeforeUpdateForm != null)
            {
                return await BeforeUpdateForm.Invoke(this, Item);
            }
            return true;
        }

        protected virtual async Task OnAfterUpdateForm()
        {
            if (AfterUpdateForm != null)
            {
                await AfterUpdateForm.Invoke(this, Item);
            }
        }

        public async Task DeleteHandler(object item)
        {
            var keys = Grid.GetPrimaryKeyValues(item);
            await DeleteHandler(keys);
        }
        public async Task DeleteHandler(object[] keys)
        {
            try
            {
                Item = await ((CGrid<T>)Grid).CrudDataService.Get(keys);
                bool isValid = await OnBeforeDeleteForm();
                if (isValid)
                {
                    ((CGrid<T>)Grid).Mode = GridMode.Delete;
                    if (Grid.DeleteComponent != null)
                        CrudRender = DeleteCrudComponent();
                    else
                        CrudRender = null;

                    await OnAfterDeleteForm();

                    _shouldRender = true;
                    StateHasChanged();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ((CGrid<T>)Grid).Mode = GridMode.Grid;
                throw;
            }
        }

        public async Task DeleteSelectedHandler()
        {
            if (SelectedRow != -1)
            {
                var item = Grid.ItemsToDisplay.ElementAt(SelectedRow);
                await DeleteHandler(item);
            }
            else
                ShowError(Strings.SelectionDeleteError);
        }

        protected virtual async Task<bool> OnBeforeDeleteForm()
        {
            if (BeforeDeleteForm != null)
            {
                return await BeforeDeleteForm.Invoke(this, Item);
            }
            return true;
        }

        protected virtual async Task OnAfterDeleteForm()
        {
            if (AfterDeleteForm != null)
            {
                await AfterDeleteForm.Invoke(this, Item);
            }
        }
        
        public async Task HandleColumnRearranged(GridHeaderComponent<T> gridHeaderComponent)
        {
            if (Payload == ColumnOrderValue.Null)
                return;

            var payload = Payload;
            Payload = ColumnOrderValue.Null;
            if (gridHeaderComponent.Column.Name == payload.ColumnName)
                return;

            var source = Grid.Columns.FirstOrDefault(c => c.Name == payload.ColumnName);
            if (source is null)
                return;

            var updated = await Grid.InsertColumn(gridHeaderComponent.Column, source);
            if (!updated)
                return;

            _shouldRender = true;
            StateHasChanged();
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

        public void ButtonCrudComponentHandler(string key)
        {
            var buttonComponent = ((CGrid<T>)Grid).ButtonCrudComponents.Get(key);
            StartFormCrudComponent(buttonComponent.Label, buttonComponent.ComponentType, buttonComponent.Actions,
                    buttonComponent.Functions, buttonComponent.Object);
        }

        public void StartFormCrudComponent<TFormComponent>(string label, IList<Action<object>> actions,
            IList<Func<object, Task>> functions, object obj)
        {
            StartFormCrudComponent(label, typeof(TFormComponent), actions, functions, obj);
        }

        public void StartFormCrudComponent(string label, Type componentType, IList<Action<object>> actions,
            IList<Func<object, Task>> functions, object obj)
        {
            var returnMode = ((CGrid<T>)Grid).Mode;
            ((CGrid<T>)Grid).Mode = GridMode.Form;
            if (componentType != null)
                CrudRender = FormCrudComponent(label, componentType, returnMode, actions, functions, obj);
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
                var isSelectColumn = ((IGridColumn<T>)column).IsSelectColumn;
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
                            ((IGridColumn<T>)column).SelectItems = await Grid.HttpClient.GetFromJsonAsync<IEnumerable<SelectItem>>(isSelectField.Url);
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        ((IGridColumn<T>)column).SelectItems = new List<SelectItem>();
                    }
                }
                else if (isSelectColumn.IsSelectKey)
                {
                    try
                    {
                        if (isSelectColumn.SelectItemExpr != null)
                        {
                            ((IGridColumn<T>)column).SelectItemExpr = async c => await Task.FromResult(isSelectColumn.SelectItemExpr(c));
                        }
                        else if (isSelectColumn.SelectItemExprAsync != null)
                        {
                            ((IGridColumn<T>)column).SelectItemExpr = isSelectColumn.SelectItemExprAsync;
                        }
                        else
                        {
                            ((IGridColumn<T>)column).SelectItemExpr = async c => await Grid.HttpClient.GetFromJsonAsync<IEnumerable<SelectItem>>(isSelectColumn.Url(c));
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        ((IGridColumn<T>)column).SelectItemExpr = async c => await Task.FromResult(new List<SelectItem>());
                    }
                }
            }
        }

        protected RenderFragment CreateCrudComponent() => builder =>
        {
            builder.OpenComponent<CascadingValue<GridComponent<T>>>(0);
            builder.AddAttribute(1, "Value", this);
            builder.AddAttribute(2, "Name", "GridComponent");
            builder.AddAttribute(3, "ChildContent", CreateCrudChildComponent());
            builder.CloseComponent();
        };

        private RenderFragment CreateCrudChildComponent() => builder =>
        {
            var componentType = Grid.CreateComponent;
            builder.OpenComponent(0, componentType);
            builder.AddAttribute(1, "Item", Item);
            var gridProperty = componentType.GetProperty("Grid");
            if (gridProperty != null && gridProperty.PropertyType == typeof(CGrid<T>))
                builder.AddAttribute(2, "Grid", (CGrid<T>)Grid);
            gridProperty = componentType.GetProperty("Actions");
            if (gridProperty != null)
                builder.AddAttribute(3, "Actions", Grid.CreateActions);
            gridProperty = componentType.GetProperty("Functions");
            if (gridProperty != null)
                builder.AddAttribute(4, "Functions", Grid.CreateFunctions);
            gridProperty = componentType.GetProperty("Object");
            if (gridProperty != null)
                builder.AddAttribute(5, "Object", Grid.CreateObject);
            builder.CloseComponent();
        };

        protected RenderFragment ReadCrudComponent() => builder =>
        {
            builder.OpenComponent<CascadingValue<GridComponent<T>>>(0);
            builder.AddAttribute(1, "Value", this);
            builder.AddAttribute(2, "Name", "GridComponent");
            builder.AddAttribute(3, "ChildContent", ReadCrudChildComponent());
            builder.CloseComponent();
        };

        private RenderFragment ReadCrudChildComponent() => builder =>
        {
            var componentType = Grid.ReadComponent;
            builder.OpenComponent(0, componentType);
            builder.AddAttribute(1, "Item", Item);
            var gridProperty = componentType.GetProperty("Grid");
            if (gridProperty != null && gridProperty.PropertyType == typeof(CGrid<T>))
                builder.AddAttribute(2, "Grid", (CGrid<T>)Grid);
            gridProperty = componentType.GetProperty("Actions");
            if (gridProperty != null)
                builder.AddAttribute(3, "Actions", Grid.ReadActions);
            gridProperty = componentType.GetProperty("Functions");
            if (gridProperty != null)
                builder.AddAttribute(4, "Functions", Grid.ReadFunctions);
            gridProperty = componentType.GetProperty("Object");
            if (gridProperty != null)
                builder.AddAttribute(5, "Object", Grid.ReadObject);
            builder.CloseComponent();
        };

        protected RenderFragment UpdateCrudComponent() => builder =>
        {
            builder.OpenComponent<CascadingValue<GridComponent<T>>>(0);
            builder.AddAttribute(1, "Value", this);
            builder.AddAttribute(2, "Name", "GridComponent");
            builder.AddAttribute(3, "ChildContent", UpdateCrudChildComponent());
            builder.CloseComponent();
        };

        private RenderFragment UpdateCrudChildComponent() => builder =>
        {
            var componentType = Grid.UpdateComponent;
            builder.OpenComponent(0, componentType);
            builder.AddAttribute(1, "Item", Item);
            var gridProperty = componentType.GetProperty("Grid");
            if (gridProperty != null && gridProperty.PropertyType == typeof(CGrid<T>))
                builder.AddAttribute(2, "Grid", (CGrid<T>)Grid);
            gridProperty = componentType.GetProperty("Actions");
            if (gridProperty != null)
                builder.AddAttribute(3, "Actions", Grid.UpdateActions);
            gridProperty = componentType.GetProperty("Functions");
            if (gridProperty != null)
                builder.AddAttribute(4, "Functions", Grid.UpdateFunctions);
            gridProperty = componentType.GetProperty("Object");
            if (gridProperty != null)
                builder.AddAttribute(5, "Object", Grid.UpdateObject);
            builder.CloseComponent();
        };

        protected RenderFragment DeleteCrudComponent() => builder =>
        {
            builder.OpenComponent<CascadingValue<GridComponent<T>>>(0);
            builder.AddAttribute(1, "Value", this);
            builder.AddAttribute(2, "Name", "GridComponent");
            builder.AddAttribute(3, "ChildContent", DeleteCrudChildComponent());
            builder.CloseComponent();
        };

        private RenderFragment DeleteCrudChildComponent() => builder =>
        {
            var componentType = Grid.DeleteComponent;
            builder.OpenComponent(0, componentType);
            builder.AddAttribute(1, "Item", Item);
            var gridProperty = componentType.GetProperty("Grid");
            if (gridProperty != null && gridProperty.PropertyType == typeof(CGrid<T>))
                builder.AddAttribute(2, "Grid", (CGrid<T>)Grid);
            gridProperty = componentType.GetProperty("Actions");
            if (gridProperty != null)
                builder.AddAttribute(3, "Actions", Grid.DeleteActions);
            gridProperty = componentType.GetProperty("Functions");
            if (gridProperty != null)
                builder.AddAttribute(4, "Functions", Grid.DeleteFunctions);
            gridProperty = componentType.GetProperty("Object");
            if (gridProperty != null)
                builder.AddAttribute(5, "Object", Grid.DeleteObject);
            builder.CloseComponent();
        };

        protected RenderFragment FormComponent(string label, Type componentType, IList<Action<object>> actions, 
            IList<Func<object, Task>> functions, object obj) => builder =>
        {
            builder.OpenComponent<CascadingValue<GridComponent<T>>>(0);
            builder.AddAttribute(1, "Value", this);
            builder.AddAttribute(2, "Name", "GridComponent");
            builder.AddAttribute(3, "ChildContent", FormChildComponent(label, componentType, actions,
                functions, obj));
            builder.CloseComponent();
        };

        private RenderFragment FormChildComponent(string label, Type componentType, IList<Action<object>> actions,
            IList<Func<object, Task>> functions, object obj) => builder =>
        {
            builder.OpenComponent(0, componentType);
            var gridProperty = componentType.GetProperty("Grid");
            if (gridProperty != null && gridProperty.PropertyType == typeof(CGrid<T>))
                builder.AddAttribute(1, "Grid", (CGrid<T>)Grid);
            gridProperty = componentType.GetProperty("Label");
            if (gridProperty != null)
                builder.AddAttribute(2, "Label", label);
            gridProperty = componentType.GetProperty("Actions");
            if (gridProperty != null)
                builder.AddAttribute(3, "Actions", actions);
            gridProperty = componentType.GetProperty("Functions");
            if (gridProperty != null)
                builder.AddAttribute(4, "Functions", functions);
            gridProperty = componentType.GetProperty("Object");
            if (gridProperty != null)
                builder.AddAttribute(5, "Object", obj);
            builder.CloseComponent();
        };

        protected RenderFragment FormCrudComponent(string label, Type componentType, GridMode returnMode, 
            IList<Action<object>> actions, IList<Func<object, Task>> functions, object obj) => builder =>
            {
                builder.OpenComponent<CascadingValue<GridComponent<T>>>(0);
                builder.AddAttribute(1, "Value", this);
                builder.AddAttribute(2, "Name", "GridComponent");
                builder.AddAttribute(3, "ChildContent", FormCrudChildComponent(label, componentType, returnMode,
                    actions, functions, obj));
                builder.CloseComponent();
            };

        private RenderFragment FormCrudChildComponent(string label, Type componentType, GridMode returnMode,
            IList<Action<object>> actions, IList<Func<object, Task>> functions, object obj) => builder =>
            {
                builder.OpenComponent(0, componentType);
                var gridProperty = componentType.GetProperty("Grid");
                if (gridProperty != null && gridProperty.PropertyType == typeof(CGrid<T>))
                    builder.AddAttribute(1, "Grid", (CGrid<T>)Grid);
                gridProperty = componentType.GetProperty("Label");
                if (gridProperty != null)
                    builder.AddAttribute(2, "Label", label);
                gridProperty = componentType.GetProperty("Actions");
                if (gridProperty != null)
                    builder.AddAttribute(3, "Actions", actions);
                gridProperty = componentType.GetProperty("Functions");
                if (gridProperty != null)
                    builder.AddAttribute(4, "Functions", functions);
                gridProperty = componentType.GetProperty("Object");
                if (gridProperty != null)
                    builder.AddAttribute(5, "Object", obj);
                gridProperty = componentType.GetProperty("ReturnMode");
                if (gridProperty != null)
                    builder.AddAttribute(6, "ReturnMode", returnMode);
                gridProperty = componentType.GetProperty("Item");
                if (gridProperty != null)
                    builder.AddAttribute(7, "Item", Item);
                builder.CloseComponent();
            };

        [Obsolete("This method is obsolete. Use the new async Back() method.", true)]
        public void BackButton()
        {
            ((CGrid<T>)Grid).Mode = GridMode.Grid;
            CrudRender = null;
            _fromCrud = true;

            _shouldRender = true;
            StateHasChanged();
        }

        public async Task Back()
        {
            bool isValid = await OnBeforeBack();
            if (isValid)
            {
                ((CGrid<T>)Grid).Mode = GridMode.Grid;
                CrudRender = null;
                _fromCrud = true;

                await OnAfterBack();

                _shouldRender = true;
                StateHasChanged();
            }
        }

        protected virtual async Task<bool> OnBeforeBack()
        {
            if (BeforeBack != null)
            {
                return await BeforeBack.Invoke(this, Item);
            }
            return true;
        }

        protected virtual async Task OnAfterBack()
        {
            if (AfterBack != null)
            {
                await AfterBack.Invoke(this, Item);
            }
        }

        public async Task<bool> CreateItem(GridCreateComponent<T> component)
        {
            try
            {
                bool isValid = await OnBeforeInsert(component);
                if (isValid)
                {
                    await ShowSpinner();
                    Item = component.Item;
                    if (Grid.ServerAPI == ServerAPI.OData && !UseMemoryCrudDataService)
                        Item = await ((ICrudODataService<T>)((CGrid<T>)Grid).CrudDataService).Add(Item);
                    else
                        await ((CGrid<T>)Grid).CrudDataService.Insert(Item);
                    if(((CGrid<T>)Grid).CrudFileService != null)
                        await ((CGrid<T>)Grid).CrudFileService.InsertFiles(Item, component.Files);
                    await HideSpinner();
                    CrudRender = null;
                    if (Grid.EditAfterInsert)
                    {
                        await Grid.UpdateGrid();
                        // execute after grid update, but before update form render
                        await OnAfterInsert(component);
                        await UpdateHandler(Item);
                    }
                    else
                    {
                        ((CGrid<T>)Grid).Mode = GridMode.Grid;
                        _fromCrud = true;
                        await UpdateGrid(true, false);
                        // execute after grid update, but before grid component render
                        await OnAfterInsert(component);
                        _shouldRender = true;
                        StateHasChanged();
                    }
                }
                return isValid;
            }
            catch (Exception e)
            {
                await HideSpinner();
                Console.WriteLine(e.Message);
                throw;
            }
        }

        protected virtual async Task<bool> OnBeforeInsert(GridCreateComponent<T> component)
        {
            if (BeforeInsert != null)
            {
                return await BeforeInsert.Invoke(component, Item);
            }
            return true;
        }

        protected virtual async Task OnAfterInsert(GridCreateComponent<T> component)
        {
            if (AfterInsert != null)
            {
                await AfterInsert.Invoke(component, Item);
            }
        }

        public async Task<bool> UpdateItem(GridUpdateComponent<T> component)
        {
            try
            {
                bool isValid = await OnBeforeUpdate(component);
                if (isValid)
                {
                    await ShowSpinner();
                    Item = component.Item;
                    if (((CGrid<T>)Grid).CrudFileService != null)
                        Item = await ((CGrid<T>)Grid).CrudFileService.UpdateFiles(Item, component.Files);
                    await ((CGrid<T>)Grid).CrudDataService.Update(Item);
                    await HideSpinner();
                    ((CGrid<T>)Grid).Mode = GridMode.Grid;
                    CrudRender = null;
                    _fromCrud = true;
                    await UpdateGrid(true, false);
                    // execute after grid update, but before component render
                    await OnAfterUpdate(component);
                    _shouldRender = true;
                    StateHasChanged();
                }
                return isValid;
            }
            catch (Exception e)
            {
                await HideSpinner();
                Console.WriteLine(e.Message);
                throw;
            }
        }

        protected virtual async Task<bool> OnBeforeUpdate(GridUpdateComponent<T> component)
        {
            if (BeforeUpdate != null)
            {
                return await BeforeUpdate.Invoke(component, Item);
            }
            return true;
        }

        protected virtual async Task OnAfterUpdate(GridUpdateComponent<T> component)
        {
            if (AfterUpdate != null)
            {
                await AfterUpdate.Invoke(component, Item);
            }
        }

        public async Task<bool> DeleteItem(GridDeleteComponent<T> component)
        {
            try
            {
                bool isValid = await OnBeforeDelete(component);
                if (isValid)
                {
                    await ShowSpinner();
                    var keys = Grid.GetPrimaryKeyValues(Item);
                    if (((CGrid<T>)Grid).CrudFileService != null)
                        await ((CGrid<T>)Grid).CrudFileService.DeleteFiles(keys);
                    await ((CGrid<T>)Grid).CrudDataService.Delete(keys);
                    await HideSpinner();
                    ((CGrid<T>)Grid).Mode = GridMode.Grid;
                    CrudRender = null;
                    _fromCrud = true;
                    await UpdateGrid(true, false);
                    // execute after grid update, but before component render
                    await OnAfterDelete(component);
                    _shouldRender = true;
                    StateHasChanged();
                }
                return isValid;
            }
            catch (Exception e)
            {
                await HideSpinner();
                Console.WriteLine(e.Message);
                throw;
            }
        }

        protected virtual async Task<bool> OnBeforeDelete(GridDeleteComponent<T> component)
        {
            if (BeforeDelete != null)
            {
                return await BeforeDelete.Invoke(component, Item);
            }
            return true;
        }

        protected virtual async Task OnAfterDelete(GridDeleteComponent<T> component)
        {
            if (AfterDelete != null)
            {
                await AfterDelete.Invoke(component, Item);
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
            if (PageSize > 0)
            {
                Grid.Pager.PageSize = PageSize;
                Grid.AddQueryParameter(GridPager.DefaultPageSizeQueryParameter, PageSize.ToString());
                await UpdateGrid();
                await OnPagerChanged();
            }
            else
            {
                PageSize = Grid.Pager.PageSize;
                _shouldRender = true;
                StateHasChanged();
            }
        }

        public async Task ChangePageSize(int pageSize)
        {
            if (pageSize > 0)
            {
                PageSize = pageSize;
                Grid.Pager.PageSize = pageSize;
                Grid.AddQueryParameter(GridPager.DefaultPageSizeQueryParameter, pageSize.ToString());
                await UpdateGrid();
                await OnPagerChanged();
            }
        }

        public async Task InputVirtualizedHeightKeyup(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                await InputVirtualizedHeightBlur();
            }
        }

        public async Task InputVirtualizedHeightBlur()
        {
            Grid.Height = VirtualizedHeight + "px";
            await UpdateGrid();
            
        }

        public async Task ChangeVirtualizedHeight(int height)
        {
            if (height > 0)
            {
                VirtualizedHeight = height;
                Grid.Height = height + "px";
                await UpdateGrid();
            }
        }

        public async Task SetGridFocus()
        {
            if (Gridmvc.Id != null && Grid.Keyboard)
            {
                await SetFocus(Gridmvc);
            }
        }

        public async Task SetFocus(ElementReference element)
        {
            await jSRuntime.InvokeVoidAsync("gridJsFunctions.focusElement", element);
        }

        public async Task ShowSpinner()
        {
            await jSRuntime.InvokeVoidAsync("gridJsFunctions.hideElement", Content);
            await jSRuntime.InvokeVoidAsync("gridJsFunctions.showElement", Spinner);
        }

        public async Task HideSpinner()
        {
            await jSRuntime.InvokeVoidAsync("gridJsFunctions.hideElement", Spinner);
            await jSRuntime.InvokeVoidAsync("gridJsFunctions.showElement", Content);
        }

        public void ShowCrudButtons()
        {
            if (Grid.Mode == GridMode.Read && Grid.ReadComponent == null && ReadComponent != null
                && (Grid.ReadEnabled || (((CGrid<T>)Grid).FuncReadEnabled != null && ((CGrid<T>)Grid).FuncReadEnabled(Item))))
            {
                ReadComponent.ShowCrudButtons();
            }
            else if (Grid.Mode == GridMode.Update && Grid.UpdateComponent == null && UpdateComponent != null
                && (Grid.UpdateEnabled || (((CGrid<T>)Grid).FuncUpdateEnabled != null && ((CGrid<T>)Grid).FuncUpdateEnabled(Item))))
            {
                UpdateComponent.ShowCrudButtons();
            }
            else if (Grid.Mode == GridMode.Delete && Grid.DeleteComponent == null && DeleteComponent != null
                && (Grid.DeleteEnabled || (((CGrid<T>)Grid).FuncDeleteEnabled != null && ((CGrid<T>)Grid).FuncDeleteEnabled(Item))))
            {
                DeleteComponent.ShowCrudButtons();
            }
        }

        public void HideCrudButtons()
        {
            if (Grid.Mode == GridMode.Read && Grid.ReadComponent == null && ReadComponent != null
                && (Grid.ReadEnabled || (((CGrid<T>)Grid).FuncReadEnabled != null && ((CGrid<T>)Grid).FuncReadEnabled(Item))))
            {
                ReadComponent.HideCrudButtons();
            }
            else if (Grid.Mode == GridMode.Update && Grid.UpdateComponent == null && UpdateComponent != null
                && (Grid.UpdateEnabled || (((CGrid<T>)Grid).FuncUpdateEnabled != null && ((CGrid<T>)Grid).FuncUpdateEnabled(Item))))
            {
                UpdateComponent.HideCrudButtons();
            }
            else if (Grid.Mode == GridMode.Delete && Grid.DeleteComponent == null && DeleteComponent != null
                && (Grid.DeleteEnabled || (((CGrid<T>)Grid).FuncDeleteEnabled != null && ((CGrid<T>)Grid).FuncDeleteEnabled(Item))))
            {
                DeleteComponent.HideCrudButtons();
            }
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

        public async Task UpdateGrid(bool reloadData = true, bool shouldRender = true)
        {
            await OnBeforeRefreshGrid();
            
            if (reloadData) await Grid.UpdateGrid();
            SelectedRow = -1;
            SelectedRows.Clear();
            InitCheckboxAndSubGridVars();

#if !NETSTANDARD2_1
            if (Grid.PagingType == PagingType.Virtualization)
            {
                await ((Virtualize<T>)VirtualizedComponent.Variable).RefreshDataAsync();
            }
#endif

            _shouldRender = shouldRender;
            if(_shouldRender)
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
