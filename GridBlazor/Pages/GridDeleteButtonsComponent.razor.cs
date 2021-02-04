using Microsoft.AspNetCore.Components;

namespace GridBlazor.Pages
{
    public partial class GridDeleteButtonsComponent<T>
    {
        [CascadingParameter(Name = "GridDeleteComponent")]
        protected GridDeleteComponent<T> GridDeleteComponent { get; set; }

        public void Render()
        {
            StateHasChanged();
        }
    }
}
