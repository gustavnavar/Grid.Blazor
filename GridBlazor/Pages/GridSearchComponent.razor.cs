using GridBlazor.Filtering;
using GridBlazor.Pagination;
using GridShared.Utility;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace GridBlazor.Pages
{
    public partial class GridSearchComponent<T>
    {
        protected string _seachUrl;
        protected string _searchValue;
        public ElementReference SearchInput;

        [CascadingParameter(Name = "GridComponent")]
        protected internal GridComponent<T> GridComponent { get; set; }

        [Parameter]
        public ICGrid Grid { get; set; }

        protected override void OnParametersSet()
        {
            _searchValue = Grid.Settings.SearchSettings.SearchValue;

            //determine current url:
            var queryBuilder = new CustomQueryStringBuilder(Grid.Settings.SearchSettings.Query);

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

        public async Task InputSearchKeyup(KeyboardEventArgs e)
        {
            if(e.Key == "Enter")
                await GridComponent.AddSearch(_searchValue);
        }

        public async Task ClearButtonClicked()
        {
            await GridComponent.RemoveSearch();
        }
    }
}