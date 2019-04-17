using GridShared.Sorting;
using GridShared.Utility;
using GridBlazor.Filtering;
using GridBlazor.Sorting;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;

namespace GridBlazor
{
    /// <summary>
    ///     Provider of grid settings, based on query string parameters
    /// </summary>
    public class QueryStringGridSettingsProvider : IGridSettingsProvider
    {
        private readonly QueryStringFilterSettings _filterSettings;
        private readonly QueryStringSortSettings _sortSettings;

        public QueryStringGridSettingsProvider(IQueryDictionary<StringValues> query)
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

        public IQueryDictionary<StringValues> ToQuery()
        {
            QueryDictionary<StringValues> query = new QueryDictionary<StringValues>();

            if (!string.IsNullOrWhiteSpace(_sortSettings.ColumnName))
            {
                query.Add(_sortSettings.ColumnQueryParameterName, _sortSettings.ColumnName);
                query.Add(_sortSettings.DirectionQueryParameterName, _sortSettings.Direction.ToString("d"));
            }

            List<string> stringValues = new List<string>();
            foreach (var column in _filterSettings.FilteredColumns)
            {
                stringValues.Add(column.ColumnName + QueryStringFilterSettings.FilterDataDelimeter +
                    column.FilterType + QueryStringFilterSettings.FilterDataDelimeter +
                    column.FilterValue); 
            }
            if(stringValues.Count > 0)
                query.Add(QueryStringFilterSettings.DefaultTypeQueryParameter, 
                    new StringValues(stringValues.ToArray()));

            return query;
        }

        #endregion
    }
}