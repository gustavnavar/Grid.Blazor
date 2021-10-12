using GridCore;
using GridCore.Filtering;
using GridCore.Searching;
using GridCore.Sorting;
using GridShared.Filtering;
using GridShared.Searching;
using GridShared.Sorting;
using GridShared.Utility;
using Microsoft.Extensions.Primitives;

namespace GridCore
{
    /// <summary>
    ///     Provider of grid settings, based on query string parameters
    /// </summary>
    public class QueryStringGridSettingsProvider : IGridSettingsProvider
    {
        private readonly QueryStringFilterSettings _filterSettings;
        private readonly QueryStringSortSettings _sortSettings;
        private readonly QueryStringSearchSettings _searchSettings;

        public QueryStringGridSettingsProvider(IQueryDictionary<StringValues> query)
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

        #endregion
    }
}