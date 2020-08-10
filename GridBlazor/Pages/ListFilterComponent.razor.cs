using GridShared.Filtering;
using GridShared.Utility;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GridBlazor.Pages
{
    public partial class ListFilterComponent<T>
    {
        protected bool _clearVisible = false;
        protected List<Filter> _filters;
        protected int _offset = 0;

        protected ElementReference listFilter;

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
            var filterSettings = FilterSettings.Where(r => r != ColumnFilterValue.Null
               && r.FilterType != GridFilterType.Condition).Select(r =>
                   new Filter(r.FilterType.ToString("d"), r.FilterValue)).ToList();
            _clearVisible = filterSettings.Count() > 0;
            _filters = filterSettings.ToList();
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender && listFilter.Id != null)
            {
                await jSRuntime.InvokeVoidAsync("gridJsFunctions.focusElement", listFilter);
                ScreenPosition sp = await jSRuntime.InvokeAsync<ScreenPosition>("gridJsFunctions.getPosition", listFilter);
                if (GridHeaderComponent.GridComponent.Grid.Direction == GridShared.GridDirection.RTL)
                {
                    if (sp != null && GridHeaderComponent.GridComponent.ScreenPosition != null
                        && sp.X < Math.Max(25, GridHeaderComponent.GridComponent.ScreenPosition.X))
                    {
                        _offset = -sp.X + Math.Max(25, GridHeaderComponent.GridComponent.ScreenPosition.X);
                        StateHasChanged();
                    }
                }
                else
                {
                    if (sp != null && GridHeaderComponent.GridComponent.ScreenPosition != null
                        && sp.X + sp.Width > Math.Min(sp.InnerWidth, GridHeaderComponent.GridComponent.ScreenPosition.X
                        + GridHeaderComponent.GridComponent.ScreenPosition.Width + 25))
                    {
                        _offset = sp.X + sp.Width - Math.Min(sp.InnerWidth, GridHeaderComponent.GridComponent.ScreenPosition.X
                        + GridHeaderComponent.GridComponent.ScreenPosition.Width + 25) + 25;
                        StateHasChanged();
                    }
                }
            }
        }

        private void MyClickHandler(MouseEventArgs e, bool isChecked, string value)
        {
            if (isChecked)
            {
                RemoveColumnFilterValue(value);
            }
            else
            {
                AddColumnFilterValue(value);
            }
            StateHasChanged();
        }

        protected void AddColumnFilterValue(string value)
        {
            _filters.Add(new Filter(GridFilterType.Equals.ToString("d"), value));
        }

        protected void RemoveColumnFilterValue(string value)
        {
            var filters = _filters.Where(r => r.Value.Equals(value));
            for (int i = filters.Count() - 1; i >= 0; i--)
                _filters.Remove(filters.ElementAt(i));
        }

        protected async Task ApplyButtonClicked()
        {
            FilterCollection filters = new FilterCollection(_filters);
            if (filters.Count() > 1)
                filters.Add(GridFilterType.Condition.ToString("d"), "2");
            await GridHeaderComponent.AddFilter(filters);
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

