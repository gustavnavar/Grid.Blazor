using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace GridBlazor
{
    public class GridDeleteComponentBase<T> : ComponentBase
    {
        [CascadingParameter(Name = "GridComponent")]
        protected GridComponent<T> GridComponent { get; set; }

        [Parameter]
        public T Item { get; set; }

        protected async Task DeleteItem()
        {
            await GridComponent.DeleteItem();
        }

        protected void BackButtonClicked()
        {
            GridComponent.BackButton();
        }
    }
}