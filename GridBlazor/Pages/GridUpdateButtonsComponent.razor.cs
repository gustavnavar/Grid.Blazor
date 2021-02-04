using Microsoft.AspNetCore.Components;

namespace GridBlazor.Pages
{
    public partial class GridUpdateButtonsComponent<T>
    {
        [CascadingParameter(Name = "GridUpdateComponent")]
        protected GridUpdateComponent<T> GridUpdateComponent { get; set; }

        public void Render()
        {
            StateHasChanged();
        }
    }
}
