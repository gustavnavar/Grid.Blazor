using Microsoft.AspNetCore.Components;

namespace GridBlazor.Pages
{
    public partial class GridTotalsComponent<T>
    {
        private const string TdStyle = "display:none;";
        private bool _shouldRender = false;

        [CascadingParameter(Name = "GridComponent")]
        protected internal GridComponent<T> GridComponent { get; set; }

        [Parameter]
        public ICGrid Grid { get; set; }

        protected override void OnParametersSet()
        {
            _shouldRender = true;
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            _shouldRender = false;
        }

        protected override bool ShouldRender()
        {
            return _shouldRender;
        }

        public void Refresh()
        {
            _shouldRender = true;
            StateHasChanged();
        }
    }
}
