using GridShared.Utility;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GridBlazor.Pages
{
    public partial class SubGridComponent<T>
    {
        private ICGrid _grid;
        private bool _init;
        protected bool _visible;
        protected RenderFragment _subGridRender;

        [CascadingParameter(Name = "GridComponent")]
        protected internal GridComponent<T> GridComponent { get; set; }

        [Parameter]
        public int GridPosition { get; set; }

        [Parameter]
        public int Cols { get; set; }

        [Parameter]
        public QueryDictionary<object> Values { get; set; }

        [Parameter]
        public IEnumerable<Action<object>> OnRowClickedActions { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            if (GridPosition < GridComponent.IsSubGridVisible.Length 
                && GridPosition < GridComponent.InitSubGrid.Length)
            {
                _visible = GridComponent.IsSubGridVisible[GridPosition];
                _init = GridComponent.InitSubGrid[GridPosition];

                if (_visible && (_grid == null || _init || (_grid != null && _grid.FixedValues != Values)))
                {
                    GridComponent.InitSubGrid[GridPosition] = false;
                    _grid = await GridComponent.Grid.SubGrids(Values.Values.ToArray());
                    _grid.Direction = GridComponent.Grid.Direction;
                    _grid.FixedValues = Values;
                    _subGridRender = CreateSubGridComponent();
                }
            }
        }

        private RenderFragment CreateSubGridComponent() => builder =>
        {
            Type gridComponentType = typeof(GridComponent<>).MakeGenericType(_grid.Type);
            builder.OpenComponent(0, gridComponentType);
            builder.AddAttribute(1, "Grid", _grid);
            if (OnRowClickedActions != null && OnRowClickedActions.Count() > 0)
            {
                builder.AddAttribute(2, "OnRowClickedActions", OnRowClickedActions);
            }
            builder.CloseComponent();
        };
    }
}
