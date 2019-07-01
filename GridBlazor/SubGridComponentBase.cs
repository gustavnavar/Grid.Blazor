using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace GridBlazor
{
    public class SubGridComponentBase<T> : ComponentBase
    {
        private int _sequence = 0;
        private ICGrid _grid;
        private bool _init;
        protected bool _visible;
        protected RenderFragment _subGridRender;

        [CascadingParameter(Name = "GridComponent")]
        protected GridComponent<T> GridComponent { get; set; }

        [Parameter]
        protected int GridPosition { get; set; }

        [Parameter]
        protected int Cols { get; set; }

        [Parameter]
        protected object[] Values { get; set; }

        protected override async Task OnParametersSetAsync()
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

        private RenderFragment CreateSubGridComponent() => builder =>
        {
            Type gridComponentType = typeof(GridComponent<>).MakeGenericType(_grid.Type);
            builder.OpenComponent(++_sequence, gridComponentType);
            builder.AddAttribute(++_sequence, "Grid", _grid);
            builder.CloseComponent();
        };
    }
}
