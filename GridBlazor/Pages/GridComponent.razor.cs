using GridBlazor.Columns;
using GridBlazor.Pagination;
using GridBlazor.Searching;
using GridShared;
using GridShared.Columns;
using GridShared.Filtering;
using GridShared.Sorting;
using GridShared.Utility;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GridBlazor.Pages
{
    public partial class GridComponent<T>
    {
        private int _sequence = 0;
        private bool _fromCrud = false;
        protected bool _hasSubGrid = false;
        protected bool _hasTotals = false;
        protected bool _requiredTotalsColumn = false;
        protected string _changePageSizeUrl;
        protected int _pageSize;
        internal bool[] IsSubGridVisible;
        internal bool[] InitSubGrid;
        protected IQueryDictionary<Type> _filterComponents;
        protected T _item;

        protected ElementReference gridmvc;

        [Inject]
        private IJSRuntime jSRuntime { get; set; }

        internal int SelectedRow { get; set; } = -1;

        internal List<int> SelectedRows { get; set; } = new List<int>();

        internal ICGridColumn FirstColumn { get; set; }

        internal ColumnOrderValue Payload { get; set; }

        protected RenderFragment CrudRender { get; set; }

        [Parameter]
        public ICGrid Grid { get; set; }

        [Parameter]
        public Action<object> OnRowClicked { get; set; }

        [Parameter]
        public IQueryDictionary<Type> CustomFilters { get; set; }

        [Parameter]
        public string GridMvcCssClass { get; set; } = "grid-mvc";

        [Parameter]
        public string GridWrapCssClass { get; set; } = "grid-wrap";

        [Parameter]
        public string GridFooterCssClass { get; set; } = "grid-footer";

        [Parameter]
        public string TableCssClass { get; set; } = "table grid-table";

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

        protected override void OnParametersSet()
        {
            _filterComponents = new QueryDictionary<Type>();
            _filterComponents.Add("System.String", typeof(TextFilterComponent<T>));
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
            _filterComponents.Add("System.Boolean", typeof(BooleanFilterComponent<T>));

            if (CustomFilters != null)
            {
                foreach(var widget in CustomFilters)
                {
                    if (_filterComponents.ContainsKey(widget.Key))
                        _filterComponents[widget.Key] = widget.Value;
                    else
                        _filterComponents.Add(widget);
                }
            }

            FirstColumn = (ICGridColumn)Grid.Columns.FirstOrDefault();

            _hasSubGrid = Grid.SubGridKeys != null && Grid.SubGridKeys.Length > 0;
            _hasTotals = Grid.IsSumEnabled || Grid.IsAverageEnabled || Grid.IsMaxEnabled || Grid.IsMinEnabled;
            _requiredTotalsColumn = _hasTotals
                && FirstColumn != null
                && (FirstColumn.IsSumEnabled || FirstColumn.IsAverageEnabled
                    || FirstColumn.IsMaxEnabled || FirstColumn.IsMinEnabled);

            InitSubGridVars();

            var queryBuilder = new CustomQueryStringBuilder(Grid.Settings.SearchSettings.Query);
            var exceptQueryParameters = new List<string> { GridPager.DefaultPageSizeQueryParameter };
            _changePageSizeUrl = queryBuilder.GetQueryStringExcept(exceptQueryParameters);
            _pageSize = Grid.Pager.ChangePageSize && Grid.Pager.QueryPageSize > 0 ? Grid.Pager.QueryPageSize : Grid.Pager.PageSize;
        }

        private void InitSubGridVars()
        {
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

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if ((firstRender || _fromCrud) && gridmvc.Id != null)
            {
                _fromCrud = false;
                await jSRuntime.InvokeVoidAsync("gridJsFunctions.focusElement", gridmvc);
            }

            if (Grid.ComponentOptions.Selectable && Grid.ComponentOptions.InitSelection
                && Grid.ItemsToDisplay.Count() > 0 && SelectedRow == -1 && SelectedRows.Count == 0)
            {
                RowClicked(0, Grid.ItemsToDisplay.First(), new MouseEventArgs { CtrlKey = false }) ;
            }
        }

        internal void RowClicked(int i, object item, MouseEventArgs args)
        {
            //If user clicked on a row withouth Control key, unselect all rows
            if (!args.CtrlKey)
                SelectedRows.Clear();
            
            //If Grid is MultiSelectable, add selected row to list of rows
            if (Grid.ComponentOptions.MultiSelectable)
            {
                SelectedRow = -1;
                //If selected row is already part of collection, remove it
                if (SelectedRows.Contains(i))
                    SelectedRows.Remove(i);
                else
                    SelectedRows.Add(i);
                
            }
            else
            {
                SelectedRow = i;
            }
            if (OnRowClicked != null)
                OnRowClicked.Invoke(item);
        }

        internal void SubGridClicked(int i)
        {
            IsSubGridVisible[i] = !IsSubGridVisible[i];
            StateHasChanged();
        }

        public async Task GoTo(int page)
        {
            Grid.AddQueryParameter(GridPager.DefaultPageQueryParameter, page.ToString());
            await UpdateGrid();

            if (Grid.ComponentOptions.Selectable && Grid.ComponentOptions.InitSelection
                && Grid.ItemsToDisplay.Count() > 0 && SelectedRow == -1 && SelectedRows.Count == 0)
            {
                RowClicked(0, Grid.ItemsToDisplay.First(), new MouseEventArgs { CtrlKey = false });
            }
        }

        public async Task GetSortUrl(string columnQueryParameterName, string columnName,
            string directionQueryParameterName, string direction)
        {
            Grid.AddQueryParameter(columnQueryParameterName, columnName);
            Grid.AddQueryParameter(directionQueryParameterName, direction);
            await UpdateGrid();
        }

        public async Task AddFilter(IGridColumn column, FilterCollection filters)
        {
            Grid.AddFilterParameter(column, filters);
            await UpdateGrid();
        }

        public async Task RemoveFilter(IGridColumn column)
        {
            Grid.RemoveFilterParameter(column);
            await UpdateGrid();
        }

        public async Task AddSearch(string searchValue)
        {
            Grid.AddQueryParameter(QueryStringSearchSettings.DefaultSearchQueryParameter, searchValue);
            await UpdateGrid();
        }

        public async Task RemoveSearch()
        {
            Grid.RemoveQueryParameter(QueryStringSearchSettings.DefaultSearchQueryParameter);
            await UpdateGrid();
        }

        public async Task AddExtSorting()
        {
            Grid.AddQueryString(ColumnOrderValue.DefaultSortingQueryParameter, Payload.ToString());
            await UpdateGrid();
        }

        public async Task ChangeExtSorting(ColumnOrderValue column)
        {
            var newColumnOrderValue = new ColumnOrderValue {
                ColumnName = column.ColumnName,
                Direction = column.Direction == GridSortDirection.Ascending ? GridSortDirection.Descending
                    : GridSortDirection.Ascending,
                Id = column.Id
            };
            Grid.ChangeQueryString(ColumnOrderValue.DefaultSortingQueryParameter, column.ToString(),
                newColumnOrderValue.ToString());
            await UpdateGrid();
        }

        public async Task RemoveExtSorting(ColumnOrderValue column)
        {
            Grid.RemoveQueryString(ColumnOrderValue.DefaultSortingQueryParameter, column.ToString());
            await UpdateGrid();
        }

        public async Task CreateHandler()
        {
            await SetSelectFields();
            _item = (T)Activator.CreateInstance(typeof(T));
            ((CGrid<T>)Grid).Mode = GridMode.Create;
            if (Grid.CreateComponent != null)
                CrudRender = CreateCrudComponent();
            else
                CrudRender = null;
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
            StateHasChanged();
        }

        public async Task UpdateHandler(object item)
        {
            await SetSelectFields();
            var keys = Grid.GetPrimaryKeyValues(item);
            try
            {
                _item = await ((CGrid<T>)Grid).CrudDataService.Get(keys);
                ((CGrid<T>)Grid).Mode = GridMode.Update;
                if (Grid.UpdateComponent != null)
                    CrudRender = UpdateCrudComponent();
                else
                    CrudRender = null;
                StateHasChanged();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                ((CGrid<T>)Grid).Mode = GridMode.Grid;
            }
        }

        public void DeleteHandler(object item)
        {
            _item = (T)item;
            ((CGrid<T>)Grid).Mode = GridMode.Delete;
            if (Grid.DeleteComponent != null)
                CrudRender = DeleteCrudComponent();
            else
                CrudRender = null;
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
                        if (isSelectField.SelectItemExpr == null)
                        {
                            var selectItems = await Grid.HttpClient.GetJsonAsync<SelectItem[]>(isSelectField.Url);
                            ((GridColumnBase<T>)column).SelectItems = selectItems.ToList();
                        }
                        else
                        {
                            ((GridColumnBase<T>)column).SelectItems = isSelectField.SelectItemExpr.Invoke();
                        }
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine(e.Message);
                        ((GridColumnBase<T>)column).SelectItems = new List<SelectItem>();
                    }
                }
            }
        }

        protected RenderFragment CreateCrudComponent() => builder =>
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

        private RenderFragment ReadCrudComponent() => builder =>
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

        private RenderFragment UpdateCrudComponent() => builder =>
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

        private RenderFragment DeleteCrudComponent() => builder =>
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

        public void BackButton()
        {
            ((CGrid<T>)Grid).Mode = GridMode.Grid;
            CrudRender = null;
            _fromCrud = true;
            StateHasChanged();
        }

        public async Task CreateItem()
        {
            try
            {
                await ((CGrid<T>)Grid).CrudDataService.Insert(_item);
                ((CGrid<T>)Grid).Mode = GridMode.Grid;
                CrudRender = null;
                _fromCrud = true;
                await UpdateGrid();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public async Task UpdateItem()
        {
            try
            {
                await ((CGrid<T>)Grid).CrudDataService.Update(_item);
                ((CGrid<T>)Grid).Mode = GridMode.Grid;
                CrudRender = null;
                _fromCrud = true;
                await UpdateGrid();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
        }

        public async Task DeleteItem()
        {
            try
            {
                var keys = Grid.GetPrimaryKeyValues(_item);
                await ((CGrid<T>)Grid).CrudDataService.Delete(keys);
                ((CGrid<T>)Grid).Mode = GridMode.Grid;
                CrudRender = null;
                _fromCrud = true;
                await UpdateGrid();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
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
            Grid.Pager.PageSize = _pageSize;
            Grid.AddQueryParameter(GridPager.DefaultPageSizeQueryParameter, _pageSize.ToString());
            await UpdateGrid();
        }

        public async Task GridComponentClick()
        {
            if (gridmvc.Id != null)
            {
                await jSRuntime.InvokeVoidAsync("gridJsFunctions.focusElement", gridmvc);
            }
        }

        public async Task GridComponentKeyup(KeyboardEventArgs e)
        {
            if (e.CtrlKey && e.Key == "ArrowLeft" && Grid.Pager.CurrentPage > 1)
            {
                await GoTo(Grid.Pager.CurrentPage - 1);
            }
            else if (e.CtrlKey && e.Key == "ArrowRight" && Grid.Pager.CurrentPage < ((GridPager)Grid.Pager).PageCount)
            {
                await GoTo(Grid.Pager.CurrentPage + 1);
            }
            else if (e.CtrlKey && e.Key == "Home")
            {
                await GoTo(1);
            }
            else if (e.CtrlKey && e.Key == "End")
            {
                await GoTo(((GridPager)Grid.Pager).PageCount);
            }
            else if (e.Key == "ArrowUp" && Grid.ComponentOptions.Selectable && SelectedRow > 0)
            {
                int selectedRow = SelectedRow - 1;
                RowClicked(selectedRow, Grid.ItemsToDisplay.ElementAt(selectedRow), new MouseEventArgs { CtrlKey = e.CtrlKey });
            }
            else if (e.Key == "ArrowDown" && Grid.ComponentOptions.Selectable && SelectedRow != -1 && SelectedRow < Grid.DisplayingItemsCount - 1)
            {
                int selectedRow = SelectedRow + 1;
                RowClicked(selectedRow, Grid.ItemsToDisplay.ElementAt(selectedRow), new MouseEventArgs { CtrlKey = e.CtrlKey });
            }
        }

        public async Task UpdateGrid()
        {
            await Grid.UpdateGrid();
            SelectedRow = -1;
            SelectedRows.Clear();
            InitSubGridVars();
            StateHasChanged();
            await GridComponentClick();
        }
    }
}
