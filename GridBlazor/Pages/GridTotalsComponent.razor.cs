using Microsoft.AspNetCore.Components;

namespace GridBlazor.Pages
{
    public partial class GridTotalsComponent
    {
        [Parameter]
        public ICGrid Grid { get; set; }
    }
}
