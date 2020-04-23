using GridShared.Columns;
using GridShared.Filtering;
using GridShared.Searching;
using System;
using System.Collections.Generic;
using System.Net;

namespace GridBlazor.Filtering
{
    /// <summary>
    ///     Grid items filter proprocessor
    /// </summary>
    internal class FilterGridODataProcessor<T> : IGridODataProcessor<T>
    {
        private readonly ICGrid _grid;
        private IGridFilterSettings _settings;
        private IGridSearchSettings _searchSettings;

        public FilterGridODataProcessor(ICGrid grid, IGridFilterSettings settings, IGridSearchSettings searchSettings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");
            _grid = grid;
            _settings = settings;
            _searchSettings = searchSettings;
        }

        public void UpdateSettings(IGridFilterSettings settings, IGridSearchSettings searchSettings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");
            _settings = settings;
            _searchSettings = searchSettings;
        }

        #region IGridODataProcessor<T> Members

        public string Process()
        {
            var filters = new List<string>();
            foreach (IGridColumn column in _grid.Columns)
            {
                var gridColumn = column as IGridColumn<T>;
                if (gridColumn == null) continue;
                if (gridColumn.Filter == null) continue;

                IEnumerable<ColumnFilterValue> options = _settings.IsInitState(column)
                                                             ? new List<ColumnFilterValue>
                                                                 {
                                                                     column.InitialFilterSettings
                                                                 }
                                                             : _settings.FilteredColumns.GetByColumn(column);

                var filter = gridColumn.Filter.GetFilter(options);
                if (!string.IsNullOrWhiteSpace(filter))
                    filters.Add(filter);
            }

            // workaround for lack of $search OData support
            var search = new List<string>();
            if (_grid.SearchingEnabled && !string.IsNullOrWhiteSpace(_searchSettings.SearchValue))
            {
                foreach (IGridColumn column in _grid.Columns)
                {
                    var gridColumn = column as IGridColumn<T>;
                    if (gridColumn == null) continue;
                    if (!_grid.SearchingHiddenColumns && gridColumn.Hidden) continue;
                    if (gridColumn.Filter == null) continue;                   
                    if (!gridColumn.Filter.IsTextColumn()) continue;

                    List<ColumnFilterValue> options = new List<ColumnFilterValue>();
                    var columnFilterValue = new ColumnFilterValue(column.Name, GridFilterType.Contains, _searchSettings.SearchValue);
                    options.Add(columnFilterValue);

                    var filter = gridColumn.Filter.GetFilter(options);
                    if (!string.IsNullOrWhiteSpace(filter))
                        search.Add(filter);
                }
            }
            string searchResult = "";
            for (int i = 0; i < search.Count; i++)
            {
                searchResult += "(" + search[i] + ")";
                if (i != search.Count - 1)
                    searchResult += " or ";
            }

            string filterResult = "";
            for (int i = 0; i < filters.Count; i++)
            {
                filterResult += "(" + filters[i] + ")";
                if (i != filters.Count - 1)
                    filterResult += " and ";
            }

            if (string.IsNullOrWhiteSpace(searchResult))
            {
                if (string.IsNullOrWhiteSpace(filterResult))
                {
                    return "";
                }
                else
                {
                    return "$filter=" + WebUtility.UrlEncode(filterResult);
                }
            }
            else
            {
                if (string.IsNullOrWhiteSpace(filterResult))
                {
                    return "$filter=" + WebUtility.UrlEncode(searchResult);
                }
                else
                {
                    return "$filter=(" + WebUtility.UrlEncode(searchResult) + ") and (" + WebUtility.UrlEncode(filterResult) + ")";
                }
            }
        }

        #endregion
    }
}