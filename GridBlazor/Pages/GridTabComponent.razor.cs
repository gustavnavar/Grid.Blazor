using GridShared;
using GridShared.Utility;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GridBlazor.Pages
{
    public partial class GridTabComponent
    {
        private bool _shouldRender = false;

        private ElementReference labels;
        private ElementReference content;

        [Inject]
        private IJSRuntime jSRuntime { get; set; }

        [Parameter]
        public IEnumerable<SelectItem> TabLabels { get; set; }

        [Parameter]
        public QueryDictionary<RenderFragment> TabContent { get; set; }

        protected override void OnParametersSet()
        {
            _shouldRender = true;
        }

        protected override bool ShouldRender()
        {
            return _shouldRender;
        }

        protected override void OnAfterRender(bool firstRender)
        {
            _shouldRender = false;
        }

        protected async Task TabClicked(MouseEventArgs e, int i)
        {
            await jSRuntime.InvokeVoidAsync("gridJsFunctions.setActive", labels, i);
            await jSRuntime.InvokeVoidAsync("gridJsFunctions.setActive", content, i);
        }
    }
}
