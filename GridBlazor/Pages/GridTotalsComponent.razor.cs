using Microsoft.AspNetCore.Components;

namespace GridBlazor.Pages
{
    public partial class GridTotalsComponent<T>
    {
        private const string TdStyle = "display:none;";

        [CascadingParameter(Name = "GridComponent")]
        protected GridComponent<T> GridComponent { get; set; }

        [Parameter]
        public ICGrid Grid { get; set; }
    }
}
