using GridShared.Columns;
using GridShared.Sorting;
using System;
using System.Linq;
using System.Net;

namespace GridBlazor.Sorting
{
    /// <summary>
    ///     Settings grid items, based on current sorting settings
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class SortGridODataProcessor<T> : IGridODataProcessor<T>
    {
        private readonly ICGrid _grid;
        private IGridSortSettings _settings;

        public SortGridODataProcessor(ICGrid grid, IGridSortSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");
            _grid = grid;
            _settings = settings;
        }

        public void UpdateSettings(IGridSortSettings settings)
        {
            if (settings == null)
                throw new ArgumentNullException("settings");
            _settings = settings;
        }

        #region IGridODataProcessor<T> Members

        public string Process()
        {
            string result = GetSorting();
            if (!string.IsNullOrWhiteSpace(result))
            {
                result = "$orderby=" + WebUtility.UrlEncode(result) + "";
            }
            return result;
        }

        private string GetSorting()
        {
            string result = "";
            if (_settings.SortValues?.Count() > 0)
            {
                var sortedColumns = _settings.SortValues.OrderBy(r => r.Id).ToList();

                var gridColumn = _grid.Columns.FirstOrDefault(c => c.Name == sortedColumns[0].ColumnName) as IGridColumn<T>;
                if (gridColumn == null)
                    return result;
                result = gridColumn.Orderers.FirstOrDefault().GetOrderBy(sortedColumns[0].Direction);
                for (int i = 1; i < gridColumn.Orderers.Count(); i++)
                {
                    string orderer = gridColumn.Orderers.ElementAt(i).GetThenBy(GridSortDirection.Ascending);
                    if (string.IsNullOrWhiteSpace(result))
                        result = orderer;
                    else
                        result += "," + orderer;
                }

                if (sortedColumns.Count() > 1)
                {
                    for (int i = 1; i < sortedColumns.Count(); i++)
                    {
                        gridColumn = _grid.Columns.FirstOrDefault(r => r.Name == sortedColumns[i].ColumnName) as IGridColumn<T>;
                        string orderer = gridColumn.Orderers.FirstOrDefault().GetThenBy(sortedColumns[i].Direction);
                        if (string.IsNullOrWhiteSpace(result))
                            result = orderer;
                        else
                            result += "," + orderer;

                        for (int j = 1; j < gridColumn.Orderers.Count(); j++)
                        {
                            orderer = gridColumn.Orderers.ElementAt(j).GetThenBy(GridSortDirection.Ascending);
                            if (string.IsNullOrWhiteSpace(result))
                                result = orderer;
                            else
                                result += "," + orderer;
                        }
                    }
                }

                if (string.IsNullOrEmpty(_settings.ColumnName))
                    return result;
                //determine gridColumn sortable:
                gridColumn = _grid.Columns.FirstOrDefault(c => c.Name == _settings.ColumnName) as IGridColumn<T>;
                if (gridColumn == null || !gridColumn.SortEnabled)
                    return result;
                foreach (var columnOrderer in gridColumn.Orderers)
                {
                    string orderer = columnOrderer.GetThenBy(_settings.Direction);
                    if (string.IsNullOrWhiteSpace(result))
                        result = orderer;
                    else
                        result += "," + orderer;
                }
            }
            else
            {
                if (string.IsNullOrEmpty(_settings.ColumnName))
                    return result;
                //determine gridColumn sortable:
                var gridColumn = _grid.Columns.FirstOrDefault(c => c.Name == _settings.ColumnName) as IGridColumn<T>;
                if (gridColumn == null || !gridColumn.SortEnabled)
                    return result;
                foreach (var columnOrderer in gridColumn.Orderers)
                {
                    string orderer = columnOrderer.GetOrderBy(_settings.Direction);
                    if (string.IsNullOrWhiteSpace(result))
                        result = orderer;
                    else
                        result += "," + orderer;
                }
            }
            return result;
        }
        #endregion
    }
}