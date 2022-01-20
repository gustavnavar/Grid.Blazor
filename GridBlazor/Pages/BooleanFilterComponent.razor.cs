using GridShared.Filtering;
using GridShared.Utility;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GridBlazor.Pages
{
    public partial class BooleanFilterComponent<T>
    {
        protected bool _clearVisible = false;
        protected string _filterValue = "";
        protected int _offset = 0;
        protected bool _initPosition = false;

        protected ElementReference boolFilter;

        [Inject]
        private IJSRuntime jSRuntime { get; set; }

        [CascadingParameter(Name = "GridHeaderComponent")]
        private GridHeaderComponent<T> GridHeaderComponent { get; set; }

        [Parameter]
        public bool Visible { get; set; }

        [Parameter]
        public string ColumnName { get; set; }

        [Parameter]
        public IEnumerable<ColumnFilterValue> FilterSettings { get; set; }

        protected override void OnParametersSet()
        {
            _filterValue = FilterSettings.FirstOrDefault().FilterValue;
            _clearVisible = !string.IsNullOrWhiteSpace(_filterValue);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!_initPosition && boolFilter.Id != null)
            {
                await jSRuntime.InvokeVoidAsync("gridJsFunctions.focusElement", boolFilter);
                ScreenPosition sp = await jSRuntime.InvokeAsync<ScreenPosition>("gridJsFunctions.getPosition", boolFilter);
                ScreenPosition gridTableSP = await jSRuntime.InvokeAsync<ScreenPosition>("gridJsFunctions.getPosition", GridHeaderComponent.GridComponent.GridTable);
                if (sp != null && gridTableSP != null)
                {
                    if (gridTableSP.X + gridTableSP.Width < sp.X + sp.Width)
                    {
                        _offset = gridTableSP.X + gridTableSP.Width - sp.X - sp.Width;
                        StateHasChanged();
                    }
                    else if (sp.X < gridTableSP.X)
                    {
                        _offset = gridTableSP.X - sp.X;
                        StateHasChanged();
                    }
                }
                _initPosition = true;
            }
        }

        protected async Task ApplyTrueButtonClicked()
        {
            await GridHeaderComponent.AddFilter(new FilterCollection(GridFilterType.Equals.ToString("d"), "true"));
        }

        protected async Task ApplyFalseButtonClicked()
        {
            await GridHeaderComponent.AddFilter(new FilterCollection(GridFilterType.Equals.ToString("d"), "false"));
        }


        protected async Task ApplyButtonClicked(string filterValue)
        {
            await GridHeaderComponent.AddFilter(new FilterCollection(GridFilterType.Equals.ToString("d"), filterValue));
        }

        protected async Task ClearButtonClicked()
        {
            await GridHeaderComponent.RemoveFilter();
        }

        public async Task FilterKeyup(KeyboardEventArgs e)
        {
            if (e.Key == "Escape")
            {
                await GridHeaderComponent.FilterIconClicked();
            }
        }
    }
}

