using GridShared.Utility;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace GridBlazor.Pages
{
    public partial class HeaderTooltipComponent<T>
    {
        protected int _offset = 0;

        protected ElementReference tooltip;

        [Inject]
        private IJSRuntime jSRuntime { get; set; }

        [CascadingParameter(Name = "GridComponent")]
        protected GridComponent<T> GridComponent { get; set; }

        [Parameter]
        public bool Visible { get; set; }

        [Parameter]
        public string Value { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (tooltip.Id != null)
            {
                await jSRuntime.InvokeVoidAsync("gridJsFunctions.focusElement", tooltip);
                ScreenPosition sp = await jSRuntime.InvokeAsync<ScreenPosition>("gridJsFunctions.getPosition", tooltip);
                if (GridComponent.Grid.Direction == GridShared.GridDirection.RTL)
                {
                    if (sp != null && GridComponent.ScreenPosition != null
                        && sp.X < Math.Max(35, GridComponent.ScreenPosition.X))
                    {
                        _offset = -sp.X - Math.Max(35, GridComponent.ScreenPosition.X);
                        StateHasChanged();
                    }
                }
                else
                {
                    if (sp != null && GridComponent.ScreenPosition != null
                        && sp.X + sp.Width > Math.Min(sp.InnerWidth, GridComponent.ScreenPosition.X
                        + GridComponent.ScreenPosition.Width + 35))
                    {
                        _offset = sp.X + sp.Width - Math.Min(sp.InnerWidth, GridComponent.ScreenPosition.X
                        + GridComponent.ScreenPosition.Width + 35) - 35;
                        StateHasChanged();
                    }
                }
            }
        }
    }
}

