using GridShared.Filtering;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GridBlazor
{
    public class BooleanFilterComponentBase<T> : ComponentBase
    {
        protected bool _clearVisible = false;
        protected string _filterValue = "";

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
    }
}

