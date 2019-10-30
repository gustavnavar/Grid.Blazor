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
        private string _columnQueryParameterName;
        private string _directionQueryParameterName;
        private readonly DefaultOrderColumnCollection _sortValues = new DefaultOrderColumnCollection();

        public QueryStringSortSettings(IQueryDictionary<StringValues> query)
        {
            if (query == null)
                throw new ArgumentException("No http context here!");
            Query = query;
            ColumnQueryParameterName = DefaultColumnQueryParameter;
            DirectionQueryParameterName = DefaultDirectionQueryParameter;

            string[] sortings = Query.Get(ColumnOrderValue.DefaultSortingQueryParameter).Count > 0 ?
                Query.Get(ColumnOrderValue.DefaultSortingQueryParameter).ToArray() : null;
            if (sortings != null)
            {
                foreach (string sorting in sortings)
                {
                    ColumnOrderValue column = CreateColumnData(sorting);
                    if (column != ColumnOrderValue.Null)
                        _sortValues.Add(column);
                }
            }
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

        public IQueryDictionary<StringValues> Query { get; }
        public string ColumnName { get; set; }
        public GridSortDirection Direction { get; set; }
        public DefaultOrderColumnCollection SortValues
        {
            get
            {
                return _sortValues;
            }
        }

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

        private ColumnOrderValue CreateColumnData(string queryParameterValue)
        {
            if (string.IsNullOrEmpty(queryParameterValue))
                return ColumnOrderValue.Null;

            string[] data = queryParameterValue.Split(new[] { ColumnOrderValue.SortingDataDelimeter }, StringSplitOptions.RemoveEmptyEntries);
            if (data.Length != 3)
                return ColumnOrderValue.Null;

            return new ColumnOrderValue
            {
                ColumnName = data[0],
                Direction = (GridSortDirection)Enum.Parse(typeof(GridSortDirection), data[1]),
                Id = int.Parse(data[2])
            };
        }
    }
}