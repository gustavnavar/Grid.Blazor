using GridShared.Columns;
using GridShared.Utility;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Threading.Tasks;

namespace GridBlazor.Pages
{
    public partial class FormTooltipComponent
    {
        protected int _offset = 0;
        protected bool _isTooltipVisible = false;

        protected ElementReference tooltip;

        [Inject]
        private IJSRuntime jSRuntime { get; set; }

        [Parameter]
        public IGridColumn Column { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (tooltip.Id != null)
            {
                await jSRuntime.InvokeVoidAsync("gridJsFunctions.focusElement", tooltip);
                ScreenPosition sp = await jSRuntime.InvokeAsync<ScreenPosition>("gridJsFunctions.getPosition", tooltip);
                if (sp != null && sp.X + sp.Width > sp.InnerWidth)
                {
                    _offset = sp.X + sp.Width - sp.InnerWidth - 35;
                    StateHasChanged();
                }
            }
        }

        public void DisplayTooltip(string columnName)
        {
            _isTooltipVisible = true;
            StateHasChanged();
        }

        public void HideTooltip(string columnName)
        {
            _isTooltipVisible = false;
            StateHasChanged();
        }
    }
}

