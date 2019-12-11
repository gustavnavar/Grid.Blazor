using GridShared.Columns;
using Microsoft.AspNetCore.Components;

namespace GridBlazor.Pages
{
    public partial class GridReadComponent<T> : ICustomGridComponent<T>
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