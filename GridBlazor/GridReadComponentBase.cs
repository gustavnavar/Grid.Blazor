using Microsoft.AspNetCore.Components;

namespace GridBlazor
{
    public class GridReadComponentBase<T> : ComponentBase
    {
        [CascadingParameter(Name = "GridComponent")]
        protected GridComponent<T> GridComponent { get; set; }

        [Parameter]
        public T Item { get; set; }

        protected void BackButtonClicked()
        {
            GridComponent.BackButton();
        }
    }
}