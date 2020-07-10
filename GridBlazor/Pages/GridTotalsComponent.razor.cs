using Microsoft.AspNetCore.Components;

namespace GridBlazor.Pages
{
    public partial class GridTotalsComponent<T>
    {
        private const string TdStyle = "display:none;";

        [Parameter]
        public ICGrid Grid { get; set; }
    }
}
