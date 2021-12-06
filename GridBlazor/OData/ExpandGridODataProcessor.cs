using GridShared.Columns;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace GridBlazor.OData
{
    /// <summary>
    ///     Settings grid items, based on current searching settings
    /// </summary>
    /// <typeparam name="T"></typeparam>
    internal class ExpandGridODataProcessor<T> : IGridODataProcessor<T>
    {
        private readonly ICGrid _grid;

        public ExpandGridODataProcessor(ICGrid grid)
        {
            _grid = grid;
        }

        #region IGridODataProcessor<T> Members

        public string Process()
        {
            string result = GetExpand();
            if (!string.IsNullOrWhiteSpace(result))
            {
                result = "$expand=" + WebUtility.UrlEncode(result) + "";
            }
            return result;
        }

        private string GetExpand()
        {
            List<string> columnNames = new List<string>();

            if (!_grid.ODataOverrideExpandList)
            {
                foreach (IGridColumn column in _grid.Columns)
                {
                    var gridColumn = column as IExpandColumn<T>;
                    if (gridColumn == null) continue;
                    if (gridColumn.Expand == null) continue;

                    columnNames.Add(gridColumn.Expand.GetName());
                }
            }

            if (_grid.ODataExpandList != null)
                columnNames.AddRange(_grid.ODataExpandList);

            if (columnNames.Count == 0)
                return "";

            columnNames = columnNames.Where(r => !string.IsNullOrWhiteSpace(r)).Distinct().ToList();
            string result = "";
            for (int i = columnNames.Count - 1; i >= 0; i--)
            {
                result += columnNames[i];
                if (i != 0)
                    result += ",";
            }
            return result;
        }

        #endregion
    }
}
