using GridBlazor.Pagination;
using Microsoft.AspNetCore.Components;

namespace GridBlazor.Pages
{
    public partial class GridCountComponent<T>
    {
        private bool _shouldRender = false;
        private int _itemsCount;

        [CascadingParameter (Name = "GridComponent")]
        private GridComponent<T> GridComponent { get; set; }

        protected override void OnParametersSet()
        {
            _itemsCount = GridComponent.Grid.Pager.ItemsCount;
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
            _itemsCount = GridComponent.Grid.Pager.ItemsCount;
            _shouldRender = true;
            StateHasChanged();
        }
    }
}
