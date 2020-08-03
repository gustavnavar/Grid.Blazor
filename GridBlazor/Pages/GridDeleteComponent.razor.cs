using GridBlazor.Columns;
using GridBlazor.Resources;
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
        private int _sequence = 0;
        private bool _shouldRender = false;
        private QueryDictionary<RenderFragment> _renderFragments;
        private IEnumerable<string> _tabGroups;

        public string Error { get; set; } = "";

        public QueryDictionary<VariableReference> Children { get; private set; } = new QueryDictionary<VariableReference>();

        [CascadingParameter(Name = "GridComponent")]
        protected GridComponent<T> GridComponent { get; set; }

        [Parameter]
        public T Item { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            _renderFragments = new QueryDictionary<RenderFragment>();
            foreach (var column in GridComponent.Grid.Columns)
            {
                // Name must have a non empty value
                if (string.IsNullOrWhiteSpace(column.Name))
                    column.Name = Guid.NewGuid().ToString();

                if (((ICGridColumn)column).SubGrids != null)
                {
                    var values = ((ICGridColumn)column).GetSubGridKeyValues(Item).Values.ToArray();
                    var grid = await ((ICGridColumn)column).SubGrids(values, false, true, false, true) as ICGrid;
                    grid.Direction = GridComponent.Grid.Direction;
                    VariableReference reference = new VariableReference();
                    if (Children.ContainsKey(column.Name))
                        Children[column.Name] = reference;
                    else
                        Children.Add(column.Name, reference);
                    if (_renderFragments.ContainsKey(column.Name))
                        _renderFragments[column.Name] = CreateSubGridComponent(grid, reference);
                    else
                        _renderFragments.Add(column.Name, CreateSubGridComponent(grid, reference));
                }
                else if (column.DeleteComponentType != null)
                {
                    VariableReference reference = new VariableReference();
                    if (Children.ContainsKey(column.Name))
                        Children[column.Name] = reference;
                    else
                        Children.Add(column.Name, reference);
                    if (_renderFragments.ContainsKey(column.Name))
                        _renderFragments[column.Name] = GridCellComponent<T>.CreateComponent(_sequence,
                            GridComponent, column.DeleteComponentType, column, Item, null, true, reference);
                    else
                        _renderFragments.Add(column.Name, GridCellComponent<T>.CreateComponent(_sequence,
                            GridComponent, column.DeleteComponentType, column, Item, null, true, reference));
                }
            }
            _tabGroups = GridComponent.Grid.Columns
                .Where(r => !string.IsNullOrWhiteSpace(r.TabGroup) && _renderFragments.Keys.Any(s => s.Equals(r.Name)))
                .Select(r => r.TabGroup).Distinct();

            _shouldRender = true;
        }

        private RenderFragment CreateSubGridComponent(ICGrid grid, VariableReference reference) => builder =>
        {
            Type gridComponentType = typeof(GridComponent<>).MakeGenericType(grid.Type);
            builder.OpenComponent(++_sequence, gridComponentType);
            builder.AddAttribute(++_sequence, "Grid", grid);
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

        protected async Task DeleteItem()
        {
            try
            {
                _tabGroups = null;
                await GridComponent.DeleteItem(this);
            }
            catch (GridException e)
            {
                await OnParametersSetAsync();
                _shouldRender = true;
                Error = string.IsNullOrWhiteSpace(e.Code) ? e.Message : e.Code + " - " + e.Message;
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                await OnParametersSetAsync();
                _shouldRender = true;
                Error = Strings.DeleteError;
            }
        }

        protected void BackButtonClicked()
        {
            GridComponent.BackButton();
        }
    }
}