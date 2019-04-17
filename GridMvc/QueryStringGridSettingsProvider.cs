using GridShared.Sorting;
using GridMvc.Filtering;
using GridMvc.Sorting;
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

        public QueryStringGridSettingsProvider(IQueryCollection query)
        {
            _sortSettings = new QueryStringSortSettings(query);
            //add additional header renderer for filterable columns:
            _filterSettings = new QueryStringFilterSettings(query);
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

        public IGridColumnHeaderRenderer GetHeaderRenderer()
        {
            var headerRenderer = new GridHeaderRenderer();
            headerRenderer.AddAdditionalRenderer(new QueryStringFilterColumnHeaderRenderer(_filterSettings));
            headerRenderer.AddAdditionalRenderer(new QueryStringSortColumnHeaderRenderer(_sortSettings));
            return headerRenderer;
        }

        #endregion
    }
}