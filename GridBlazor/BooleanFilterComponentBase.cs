using Microsoft.AspNetCore.Components;
using System.Threading.Tasks;

namespace GridBlazor
{
    public class BooleanFilterComponentBase<T> : ComponentBase
    {
        protected bool _clearVisible = false;

        [CascadingParameter(Name = "GridHeaderComponent")]
        private GridHeaderComponent<T> GridHeaderComponent { get; set; }

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

        protected async Task ApplyTrueButtonClicked()
        {
            await GridHeaderComponent.AddFilter("1", "true");
        }

        protected async Task ApplyFalseButtonClicked()
        {
            await GridHeaderComponent.AddFilter("1", "false");
        }


        protected async Task ApplyButtonClicked(string filterValue)
        {
            await GridHeaderComponent.AddFilter("1", filterValue);
        }

        protected async Task ClearButtonClicked()
        {
            await GridHeaderComponent.RemoveFilter();
        }
    }
}

