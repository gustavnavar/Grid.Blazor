using GridBlazor.Columns;
using GridShared.Columns;
using GridShared.Utility;
using Microsoft.AspNetCore.Components;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace GridBlazor.Pages
{
    public partial class GridReadComponent<T> : ICustomGridComponent<T>
    {
        private int _sequence = 0;
        private QueryDictionary<RenderFragment> _grids;

        [CascadingParameter(Name = "GridComponent")]
        protected GridComponent<T> GridComponent { get; set; }

        [Parameter]
        public T Item { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            _grids = new QueryDictionary<RenderFragment>();
            foreach (var column in GridComponent.Grid.Columns)
            {
                if (((ICGridColumn)column).SubGrids != null)
                {
                    var values = ((ICGridColumn)column).GetSubGridKeyValues(Item).Values.ToArray();
                    var grid = await ((ICGridColumn)column).SubGrids(values, false, true, false, false) as ICGrid;
                    _grids.Add(column.Name, CreateSubGridComponent(grid));
                }
            }
        }
        
        private RenderFragment CreateSubGridComponent(ICGrid grid) => builder =>
        {
            Type gridComponentType = typeof(GridComponent<>).MakeGenericType(grid.Type);
            builder.OpenComponent(++_sequence, gridComponentType);
            builder.AddAttribute(++_sequence, "Grid", grid);
            builder.CloseComponent();
        };

        protected void BackButtonClicked()
        {
            GridComponent.BackButton();
        }
    }
}