using GridShared.Filtering;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace GridBlazor
{
    public class TextFilterComponentBase<T> : ComponentBase
    {
        protected bool _clearVisible = false;
        protected Filter[] _filters;
        protected string _condition;

        [CascadingParameter(Name = "GridHeaderComponent")]
        protected GridHeaderComponent<T> GridHeaderComponent { get; set; }

        [Parameter]
        protected bool visible { get; set; }

        [Parameter]
        protected string ColumnName { get; set; }

        [Parameter]
        protected IEnumerable<ColumnFilterValue> FilterSettings { get; set; }

        protected override void OnParametersSet()
        {
            _condition = FilterSettings.SingleOrDefault(r => r != ColumnFilterValue.Null
                && r.FilterType == GridFilterType.Condition).FilterValue;
            if (string.IsNullOrWhiteSpace(_condition))
                _condition = GridFilterCondition.And.ToString("d");

            var filterSettings = FilterSettings.Where(r => r != ColumnFilterValue.Null
               && r.FilterType != GridFilterType.Condition).Select(r =>
                   new Filter(r.FilterType.ToString("d"), r.FilterValue)).ToList();
            _clearVisible = filterSettings.Count() > 0;
            if (filterSettings.Count() == 0)
                filterSettings.Add(new Filter(GridFilterType.Equals.ToString("d"), ""));
            _filters = filterSettings.ToArray();
        }

        protected void AddColumnFilterValue()
        {
            Array.Resize(ref _filters, _filters.Length + 1);
            _filters[_filters.Length - 1] = new Filter(GridFilterType.Equals.ToString("d"), "");
            StateHasChanged();
        }

        protected void RemoveColumnFilterValue()
        {
            if (_filters.Length > 1)
            {
                Array.Resize(ref _filters, _filters.Length - 1);
                StateHasChanged();
            }
        }

        protected async Task ApplyButtonClicked()
        {
            FilterCollection filters = new FilterCollection(_filters);
            if (filters.Count() > 1)
                filters.Add(GridFilterType.Condition.ToString("d"), _condition);
            await GridHeaderComponent.AddFilter(filters);
        }

        protected async Task ClearButtonClicked()
        {
            await GridHeaderComponent.RemoveFilter();
        }
    }
}

