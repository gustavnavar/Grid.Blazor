using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace GridBlazor.Pages
{
    public partial class SubGridComponent<T>
    {
        private int _sequence = 0;
        private ICGrid _grid;
        private bool _init;
        protected bool _visible;
        protected RenderFragment _subGridRender;

        [CascadingParameter(Name = "GridComponent")]
        private GridComponent<T> GridComponent { get; set; }

        [Parameter]
        public int GridPosition { get; set; }

        [Parameter]
        public int Cols { get; set; }

        [Parameter]
        public object[] Values { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            if (GridPosition < GridComponent.IsSubGridVisible.Length 
                && GridPosition < GridComponent.InitSubGrid.Length)
            {
                _visible = GridComponent.IsSubGridVisible[GridPosition];
                _init = GridComponent.InitSubGrid[GridPosition];

                if (_visible && (_grid == null || _init))
                {
                    GridComponent.InitSubGrid[GridPosition] = false;
                    _grid = await GridComponent.Grid.SubGrids(Values);
                    _subGridRender = CreateSubGridComponent();
                }
            }
        }

        private RenderFragment CreateSubGridComponent() => builder =>
        {
            Type gridComponentType = typeof(GridComponent<>).MakeGenericType(_grid.Type);
            builder.OpenComponent(++_sequence, gridComponentType);
            builder.AddAttribute(++_sequence, "Grid", _grid);
            builder.CloseComponent();
        };
    }
}
