using GridShared.Columns;
using GridShared.Utility;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;

namespace GridBlazor.Pages
{
    public partial class FormTooltipComponent<T>
    {
        protected int _offset = 0;
        protected bool _isTooltipVisible = false;
        protected string _customStyle1 = "";
        protected string _customStyle2 = "";

        protected ElementReference tooltip;

        [Inject]
        private IJSRuntime jSRuntime { get; set; }

        [CascadingParameter(Name = "GridComponent")]
        protected internal GridComponent<T> GridComponent { get; set; }

        [Parameter]
        public IGridColumn Column { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (tooltip.Id != null)
            {
                await jSRuntime.InvokeVoidAsync("gridJsFunctions.focusElement", tooltip);
                ScreenPosition sp = await jSRuntime.InvokeAsync<ScreenPosition>("gridJsFunctions.getPosition", tooltip);
                ScreenPosition gridComponentSP = GridComponent.gridComponentSP;
                if (GridComponent.Grid.Direction == GridShared.GridDirection.RTL)
                {
                    if (sp != null && gridComponentSP != null && sp.X < Math.Max(35, gridComponentSP.X))
                    {
                        _offset = -sp.X - Math.Max(35, gridComponentSP.X);
                        StateHasChanged();
                    }
                }
                else
                {
                    if (sp != null && gridComponentSP != null
                        && sp.X + sp.Width > Math.Min(sp.InnerWidth, gridComponentSP.X
                        + gridComponentSP.Width + 35))
                    {
                        _offset = sp.X + sp.Width - Math.Min(sp.InnerWidth, gridComponentSP.X
                            + gridComponentSP.Width + 35) - 35;
                        StateHasChanged();
                    }
                }
            }

            _customStyle1 = GridComponent.Grid.Direction == GridShared.GridDirection.RTL ? _offset > 0 ? "margin-right:-" + _offset.ToString() + "px;" : "margin-right:90px;" : _offset > 0 ? "margin-left:-" + _offset.ToString() + "px;" : "margin-left:90px;";
            _customStyle2 = GridComponent.Grid.Direction == GridShared.GridDirection.RTL ? _offset > 0 ? "margin-right:" + _offset.ToString() + "px" : "margin-right:-90px;" : _offset > 0 ? "margin-left:" + _offset.ToString() + "px" : "margin-left:-90px;";
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

