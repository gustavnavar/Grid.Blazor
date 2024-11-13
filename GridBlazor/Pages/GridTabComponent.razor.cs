using GridShared;
using GridShared.Utility;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GridBlazor.Pages
{
    public partial class GridTabComponent<T>
    {
        private bool _shouldRender = false;

        private ElementReference labels;
        private ElementReference content;

        [Inject]
        private IJSRuntime jSRuntime { get; set; }

        [CascadingParameter(Name = "GridComponent")]
        protected internal GridComponent<T> GridComponent { get; set; }

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
            await jSRuntime.InvokeVoidAsync("gridJsFunctions.setItemActive", labels, i, GridComponent.Grid.HtmlClass.TabItemActive);
            await jSRuntime.InvokeVoidAsync("gridJsFunctions.setLinkActive", labels, i, GridComponent.Grid.HtmlClass.TabLinkActive);
            await jSRuntime.InvokeVoidAsync("gridJsFunctions.setPaneActive", content, i, 
                GridComponent.Grid.HtmlClass.TabPaneActive, GridComponent.Grid.HtmlClass.TabPaneHidden);
        }
    }
}
