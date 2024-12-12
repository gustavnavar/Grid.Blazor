using Microsoft.AspNetCore.Components;

namespace GridBlazor.Pages
{
    public partial class GridVirtualRowComponent<T>
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
    }
}
