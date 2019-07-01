using Microsoft.AspNetCore.Components;

namespace GridBlazor
{
    public class GridTotalsComponentBase : ComponentBase
    {
        [Parameter]
        protected ICGrid Grid { get; set; }
    }
}
