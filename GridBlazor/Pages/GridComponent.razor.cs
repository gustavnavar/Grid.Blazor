using GridBlazor.Columns;
using GridBlazor.Pagination;
using GridBlazor.Searching;
using GridShared;
using GridShared.Columns;
using GridShared.Events;
using GridShared.Filtering;
using GridShared.Pagination;
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
        internal bool _isDateTimeLocalSupported = false;

        protected ElementReference gridmvc;
        public ElementReference PageSizeInput;
        public GridSearchComponent<T> SearchComponent;

        public event Func<object, SortEventArgs, Task> SortChanged;
        public event Func<object, ExtSortEventArgs, Task> ExtSortChanged;
        public event Func<object, FilterEventArgs, Task> FilterChanged;
        public event Func<object, SearchEventArgs, Task> SearchChanged;
        public event Func<object, PagerEventArgs, Task> PagerChanged;

        public event Func<GridCreateComponent<T>, T, Task<bool>> BeforeInsert;
        public event Func<GridUpdateComponent<T>, T, Task<bool>> BeforeUpdate;
        public event Func<GridDeleteComponent<T>, T, Task<bool>> BeforeDelete;

        internal event Action FilterButtonClicked;

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
        public string GridItemsCountCssClass { get; set; } = "grid-itemscount";

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

        protected override bool ShouldRender()
        {
            return _shouldRender;
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                _isDateTimeLocalSupported = await jSRuntime.InvokeAsync<bool>("gridJsFunctions.isDateTimeLocalSupported");
            }
            
            if ((firstRender || _fromCrud) && gridmvc.Id != null && Grid.Keyboard)
            {
                _fromCrud = false;
                await SetFocus(gridmvc);
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

            _shouldRender = false;
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
                Grid.SelectedItems = Grid.SelectedItems.Concat(new[] {item });
            }
            if (OnRowClicked != null)
                OnRowClicked.Invoke(item);

            _shouldRender = true;
            StateHasChanged();
        }

        internal void SubGridClicked(int i)
        {
            IsSubGridVisible[i] = !IsSubGridVisible[i];

            _shouldRender = true;
            StateHasChanged();
        }

        public async Task GoTo(int page)
        {
            Grid.AddQueryParameter(GridPager.DefaultPageQueryParameter, page.ToString());
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
            Grid.AddFilterParameter(column, filters);
            await UpdateGrid();
            await OnFilterChanged();
        }

        protected virtual async Task OnFilterChanged()
        {
            FilterEventArgs args = new FilterEventArgs();
            args.FilteredColumns = Grid.Settings.FilterSettings.FilteredColumns;

            if (FilterChanged != null)
            {
                await FilterChanged.Invoke(this, args);
            }
        }

        public async Task RemoveFilter(IGridColumn column)
        {
            Grid.RemoveFilterParameter(column);
            await UpdateGrid();
            await OnFilterChanged();
        }

        public async Task RemoveAllFilters()
        {
            Grid.RemoveAllFilters();
            await UpdateGrid();
            await OnFilterChanged();
        }

        public async Task AddSearch(string searchValue)
        {
            Grid.AddQueryParameter(QueryStringSearchSettings.DefaultSearchQueryParameter, searchValue);
            await UpdateGrid();
            await OnSearchChanged();
        }

        protected virtual async Task OnSearchChanged()
        {
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
                    await ((CGrid<T>)Grid).CrudDataService.Insert(_item);
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

        public async Task UpdateItem(GridUpdateComponent<T> component)
        {
            try
            {
                bool isValid = await OnBeforeUpdate(component);
                if (isValid)
                {
                    await ((CGrid<T>)Grid).CrudDataService.Update(_item);
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

        public async Task DeleteItem(GridDeleteComponent<T> component)
        {
            try
            {
                bool isValid = await OnBeforeDelete(component);
                if (isValid)
                {
                    var keys = Grid.GetPrimaryKeyValues(_item);
                    await ((CGrid<T>)Grid).CrudDataService.Delete(keys);
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

        public async Task InputPageSizeKeyup(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                await ChangePageSize(_pageSize);
            }
        }

        public async Task InputPageSizeBlur()
        {
            await ChangePageSize(_pageSize);
        }

        public async Task ChangePageSize(int pageSize)
        {
            Grid.Pager.PageSize = pageSize;
            Grid.AddQueryParameter(GridPager.DefaultPageSizeQueryParameter, pageSize.ToString());
            await UpdateGrid();
            await OnPagerChanged();
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

        public async Task UpdateGrid(bool ReloadData = true)
        {
            if (ReloadData) await Grid.UpdateGrid();
            SelectedRow = -1;
            SelectedRows.Clear();
            InitSubGridVars();

            _shouldRender = true;
            StateHasChanged();

            await SetGridFocus();
        }
    }
}
