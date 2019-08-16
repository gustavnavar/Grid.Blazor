using GridBlazor.Pagination;
using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace GridBlazor
{
    public class GridPagerComponentBase<T> : ComponentBase
    {
        [CascadingParameter (Name = "GridComponent")]
        private GridComponent<T> GridComponent { get; set; }

        [Parameter]
        public GridPager GridPager { get; set; }

        protected async Task PagerButtonClicked(int page)
        {
            await GridComponent.GoTo(page);
        }
    }
}
