using Microsoft.AspNetCore.Components;

namespace GridBlazor.Pages
{
    public partial class GridBodyComponent<T>
    {
        [CascadingParameter(Name = "GridComponent")]
        protected GridComponent<T> GridComponent { get; private set; }
    }
}
