using Microsoft.AspNetCore.Components;

namespace GridBlazor
{
    public class GridTotalsComponentBase : ComponentBase
    {
        [Parameter]
        public ICGrid Grid { get; set; }
    }
}
