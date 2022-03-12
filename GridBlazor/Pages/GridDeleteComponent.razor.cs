using GridBlazor.Columns;
using GridBlazor.Resources;
using GridShared;
using GridShared.Columns;
using GridShared.Utility;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GridBlazor.Pages
{
    public partial class GridDeleteComponent<T> : ICustomGridComponent<T>
    {
        private bool _shouldRender = false;
        private QueryDictionary<RenderFragment> _renderFragments;
        private IEnumerable<string> _tabGroups;
        internal int _buttonsVisibility = 0;
        private QueryDictionary<bool> _buttonCrudComponentVisibility = new QueryDictionary<bool>();
        private string _code = StringExtensions.RandomString(8);
        private string _confirmationCode = "";

        public string Error { get; set; } = "";

        public GridDeleteButtonsComponent<T> GridDeleteButtonsComponent { get; private set; }

        public QueryDictionary<VariableReference> Children { get; private set; } = new QueryDictionary<VariableReference>();

        [CascadingParameter(Name = "GridComponent")]
        protected GridComponent<T> GridComponent { get; set; }

        [Parameter]
        public T Item { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            _renderFragments = new QueryDictionary<RenderFragment>();
            Children = new QueryDictionary<VariableReference>();

            foreach (var column in GridComponent.Grid.Columns)
            {
                // Name must have a non empty value
                if (string.IsNullOrWhiteSpace(column.Name))
                    column.Name = Guid.NewGuid().ToString();

                if (((ICGridColumn)column).SubGrids != null)
                {
                    var values = ((ICGridColumn)column).GetSubGridKeyValues(Item);
                    var grid = await ((ICGridColumn)column).SubGrids(values.Values.ToArray(), false, true, false, true) as ICGrid;
                    grid.Direction = GridComponent.Grid.Direction;
                    grid.FixedValues = values;
                    VariableReference reference = new VariableReference();
                    Children.AddParameter(column.Name, reference);
                    _renderFragments.AddParameter(column.Name, CreateSubGridComponent(grid, reference));
                }
                else if (column.DeleteComponentType != null)
                {
                    VariableReference reference = new VariableReference();
                    Children.AddParameter(column.Name, reference);
                    _renderFragments.AddParameter(column.Name, GridCellComponent<T>.CreateComponent(GridComponent, 
                        column.DeleteComponentType, column, Item, null, true, reference));
                }
            }
            _tabGroups = GridComponent.Grid.Columns
                .Where(r => !string.IsNullOrWhiteSpace(r.TabGroup) && _renderFragments.Keys.Any(s => s.Equals(r.Name)))
                .Select(r => r.TabGroup).Distinct();

            if (((CGrid<T>)GridComponent.Grid).ButtonCrudComponents != null && ((CGrid<T>)GridComponent.Grid).ButtonCrudComponents.Count() > 0)
            {
                foreach(var key in ((CGrid<T>)GridComponent.Grid).ButtonCrudComponents.Keys)
                {
                    var buttonCrudComponent = ((CGrid<T>)GridComponent.Grid).ButtonCrudComponents.Get(key);
                    if ((buttonCrudComponent.DeleteMode != null && buttonCrudComponent.DeleteMode(Item)) ||
                        (buttonCrudComponent.DeleteModeAsync != null && await buttonCrudComponent.DeleteModeAsync(Item)) ||
                        (buttonCrudComponent.GridMode.HasFlag(GridMode.Delete)))
                    {
                        _buttonCrudComponentVisibility.AddParameter(key, true);
                    }
                    else
                    {
                        _buttonCrudComponentVisibility.AddParameter(key, false);
                    }
                }
            }

            _shouldRender = true;
        }

        private RenderFragment CreateSubGridComponent(ICGrid grid, VariableReference reference) => builder =>
        {
            Type gridComponentType = typeof(GridComponent<>).MakeGenericType(grid.Type);
            builder.OpenComponent(0, gridComponentType);
            builder.AddAttribute(1, "Grid", grid);
            builder.AddComponentReferenceCapture(2, r => reference.Variable = r);
            builder.CloseComponent();
        };

        protected override bool ShouldRender()
        {
            return _shouldRender;
        }

        protected override void OnAfterRender(bool firstRender)
        {
            _shouldRender = false;
        }

        public void ShowCrudButtons()
        {
            _buttonsVisibility ++;
            GridDeleteButtonsComponent.Render();
        }

        public void HideCrudButtons()
        {
            _buttonsVisibility --;
            GridDeleteButtonsComponent.Render();
        }

        public async Task DeleteItem()
        {
            if (GridComponent.Grid.DeleteConfirmation && _code != _confirmationCode)
            {
                _shouldRender = true;
                Error = Strings.DeleteConfirmCodeError;
                StateHasChanged();
                return;
            }

            try
            {
                bool isValid = await GridComponent.DeleteItem(this);
                if(isValid)
                    _tabGroups = null;
            }
            catch (GridException e)
            {
                await OnParametersSetAsync();
                _shouldRender = true;
                Error = string.IsNullOrWhiteSpace(e.Code) ? e.Message : e.Code + " - " + e.Message;
                StateHasChanged();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                await OnParametersSetAsync();
                _shouldRender = true;
                Error = Strings.DeleteError;
                StateHasChanged();
            }
        }

        public async Task BackButtonClicked()
        {
            await GridComponent.Back();
        }
    }
}