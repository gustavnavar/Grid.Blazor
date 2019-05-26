using GridMvc.Filtering;
using GridMvc.Searching;
using GridMvc.Sorting;
using GridShared.Filtering;
using GridShared.Searching;
using GridShared.Sorting;
using Microsoft.AspNetCore.Http;

namespace GridMvc
{
    /// <summary>
    ///     Provider of grid settings, based on query string parameters
    /// </summary>
    public class QueryStringGridSettingsProvider : IGridSettingsProvider
    {
        private readonly QueryStringFilterSettings _filterSettings;
        private readonly QueryStringSortSettings _sortSettings;
        private readonly QueryStringSearchSettings _searchSettings;

        public QueryStringGridSettingsProvider(IQueryCollection query)
        {
            _sortSettings = new QueryStringSortSettings(query);
            //add additional header renderer for filterable columns:
            _filterSettings = new QueryStringFilterSettings(query);
            _searchSettings = new QueryStringSearchSettings(query);
        }

        #region IGridSettingsProvider Members

        public IGridSortSettings SortSettings
        {
            get { return _sortSettings; }
        }

        public IGridFilterSettings FilterSettings
        {
            get { return _filterSettings; }
        }

        public IGridSearchSettings SearchSettings
        {
            get { return _searchSettings; }
        }

        public IGridColumnHeaderRenderer GetHeaderRenderer()
        {
            var headerRenderer = new GridHeaderRenderer();
            headerRenderer.AddAdditionalRenderer(new QueryStringFilterColumnHeaderRenderer(_filterSettings));
            headerRenderer.AddAdditionalRenderer(new QueryStringSortColumnHeaderRenderer(_sortSettings));
            headerRenderer.SearchRender = new QueryStringSearchHeaderRenderer(_searchSettings);
            return headerRenderer;
        }

        #endregion
    }
}