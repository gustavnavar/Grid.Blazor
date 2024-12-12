using Microsoft.AspNetCore.Components;

namespace GridBlazor.Pages
{
    public partial class GridRowComponent<T>
    {
        [CascadingParameter(Name = "GridComponent")]
        protected internal GridComponent<T> GridComponent { get; set; }

        [Parameter]
        public ICGrid Grid { get; set; }

        [Parameter]
        public bool HasSubGrid { get; set; }

        [Parameter]
        public bool RequiredTotalsColumn { get; set; }

        [Parameter]
        public object Item { get; set; }

        [Parameter]
        public int GridPosition { get; set; }

        internal void SubGridClicked()
        {
            GridComponent.IsSubGridVisible[GridPosition] = !GridComponent.IsSubGridVisible[GridPosition];
            StateHasChanged();
        }
    }
}
