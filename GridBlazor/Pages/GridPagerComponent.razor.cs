using GridBlazor.Pagination;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace GridBlazor.Pages
{
    public partial class GridPagerComponent<T>
    {
        private int _currentPage;

        [CascadingParameter (Name = "GridComponent")]
        private GridComponent<T> GridComponent { get; set; }

        [Parameter]
        public GridPager GridPager { get; set; }

        protected override void OnParametersSet()
        {
            _currentPage = GridPager.CurrentPage;
        }

        protected async Task PagerButtonClicked(int page)
        {
            await GridComponent.GoTo(page);
        }

        public async Task GoToKeyup(KeyboardEventArgs e)
        {
            if (e.Key == "Enter")
            {
                await GoToBlur();
            }
        }

        public async Task GoToBlur()
        {
            await GridComponent.GoTo(_currentPage);
        }
    }
}
