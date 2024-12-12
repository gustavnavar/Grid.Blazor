using Microsoft.AspNetCore.Components;

namespace GridBlazor.Pages
{
    public partial class GridBodyComponent<T>
    {
        [CascadingParameter(Name = "GridComponent")]
        protected internal GridComponent<T> GridComponent { get; private set; }
    }
}
