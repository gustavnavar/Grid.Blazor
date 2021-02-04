using Microsoft.AspNetCore.Components;

namespace GridBlazor.Pages
{
    public partial class GridReadButtonsComponent<T>
    {
        [CascadingParameter(Name = "GridReadComponent")]
        protected GridReadComponent<T> GridReadComponent { get; set; }

        public void Render()
        {
            StateHasChanged();
        }
    }
}
