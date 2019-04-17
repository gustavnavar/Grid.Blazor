using GridShared.Sorting;
using GridShared.Utility;
using Microsoft.Extensions.Primitives;
using System;

namespace GridBlazor.Sorting
{
    /// <summary>
    ///     Grid sort settings takes from query string
    /// </summary>
    public class QueryStringSortSettings : IGridSortSettings
    {
        public const string DefaultDirectionQueryParameter = "grid-dir";
        public const string DefaultColumnQueryParameter = "grid-column";
        public readonly IQueryDictionary<StringValues> Query;
        private string _columnQueryParameterName;
        private string _directionQueryParameterName;

        public QueryStringSortSettings(IQueryDictionary<StringValues> query)
        {
            if (query == null)
                throw new ArgumentException("No http context here!");
            Query = query;
            ColumnQueryParameterName = DefaultColumnQueryParameter;
            DirectionQueryParameterName = DefaultDirectionQueryParameter;
        }

        public string ColumnQueryParameterName
        {
            get { return _columnQueryParameterName; }
            set
            {
                _columnQueryParameterName = value;
                RefreshColumn();
            }
        }

        public string DirectionQueryParameterName
        {
            get { return _directionQueryParameterName; }
            set
            {
                _directionQueryParameterName = value;
                RefreshDirection();
            }
        }

        #region IGridSortSettings Members

        public string ColumnName { get; set; }
        public GridSortDirection Direction { get; set; }

        #endregion

        private void RefreshColumn()
        {
            //Columns
            string currentSortColumn = Query.Get(ColumnQueryParameterName).ToString() ?? string.Empty;
            ColumnName = currentSortColumn;
            if (string.IsNullOrEmpty(currentSortColumn))
            {
                Direction = GridSortDirection.Ascending;
            }
        }

        private void RefreshDirection()
        {
            //Direction
            string currentDirection = Query.Get(DirectionQueryParameterName).ToString() ??
                                      string.Empty;
            if (string.IsNullOrEmpty(currentDirection))
            {
                Direction = GridSortDirection.Ascending;
                return;
            }
            GridSortDirection dir;
            Enum.TryParse(currentDirection, true, out dir);
            Direction = dir;
        }
    }
}