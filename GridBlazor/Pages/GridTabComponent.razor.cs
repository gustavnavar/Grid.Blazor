using GridShared;
using GridShared.Utility;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;

namespace GridBlazor.Pages
{
    public partial class GridTabComponent
    {
        private int selectedTab = 0;

        [Parameter]
        public IEnumerable<SelectItem> TabLabels { get; set; }

        [Parameter]
        public QueryDictionary<RenderFragment> TabContent { get; set; }

        protected void TabClicked(MouseEventArgs e, int i)
        {
            selectedTab = i;
        }
    }
}
