using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace GridBlazor
{
    public class DateTimeFilterComponentBase<T> : ComponentBase
    {
        protected bool _clearVisible = false;

        [CascadingParameter(Name = "GridHeaderComponent")]
        protected GridHeaderComponent<T> GridHeaderComponent { get; set; }

        [Parameter]
        protected bool visible { get; set; }

        [Parameter]
        protected string filterType { get; set; }

        [Parameter]
        protected string filterValue { get; set; }

        protected override void OnParametersSet()
        {
            _clearVisible = !string.IsNullOrWhiteSpace(filterValue);
        }

        protected async Task ApplyButtonClicked()
        {
            await GridHeaderComponent.AddFilter(filterType, filterValue);
        }

        protected async Task ClearButtonClicked()
        {
            await GridHeaderComponent.RemoveFilter();
        }
    }
}

