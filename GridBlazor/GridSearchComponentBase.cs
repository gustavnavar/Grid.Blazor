using GridBlazor.Filtering;
using GridBlazor.Pagination;
using GridBlazor.Searching;
using GridShared.Utility;
using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GridBlazor
{
    public class GridSearchComponentBase<T> : ComponentBase
    {
        protected string _seachUrl;
        protected string _searchValue;

        [CascadingParameter(Name = "GridComponent")]
        protected GridComponent<T> GridComponent { get; set; }
        [Parameter]
        protected ICGrid Grid { get; set; }

        protected override void OnParametersSet()
        {
            _searchValue = Grid.Settings.SearchSettings.SearchValue;

            //determine current url:
            var queryBuilder = new CustomQueryStringBuilder(((QueryStringSearchSettings)Grid.Settings.SearchSettings).Query);

            var exceptQueryParameters = new List<string>
            {
                QueryStringFilterSettings.DefaultTypeQueryParameter,
                QueryStringFilterSettings.DefaultClearInitFilterQueryParameter
            };
            string pagerParameterName = GetPagerQueryParameterName(Grid.Pager);
            if (!string.IsNullOrEmpty(pagerParameterName))
                exceptQueryParameters.Add(pagerParameterName);

            _seachUrl = queryBuilder.GetQueryStringExcept(exceptQueryParameters);
        }

        /// <summary>
        ///     Extract query string parameter name from default grid pager (if using)
        /// </summary>
        private string GetPagerQueryParameterName(IGridPager pager)
        {
            var defaultPager = pager as GridPager;
            if (defaultPager == null)
                return string.Empty;
            return defaultPager.ParameterName;
        }

        public async Task ApplyButtonClicked()
        {
            await GridComponent.AddSearch(_searchValue);
        }

        public async Task ClearButtonClicked()
        {
            await GridComponent.RemoveSearch();
        }
    }
}