using GridShared.Columns;
using GridShared.Filtering;
using Microsoft.AspNetCore.Http;
using System;
using System.Linq;


namespace GridMvc.Filtering
{
    /// <summary>
    ///     Object gets filter settings from query string
    /// </summary>
    public class QueryStringFilterSettings : IGridFilterSettings
    {
        public const string DefaultTypeQueryParameter = "grid-filter";
        private const string FilterDataDelimeter = "__";
        public const string DefaultClearInitFilterQueryParameter = "grid-clearinitfilter";
        public readonly IQueryCollection Query;
        private readonly DefaultFilterColumnCollection _filterValues = new DefaultFilterColumnCollection();

        #region Ctor's

        public QueryStringFilterSettings(IQueryCollection query)
        {
            if (query == null)
                throw new ArgumentException("No http context here!");
            Query = query;

            string[] filters = Query[DefaultTypeQueryParameter].Count > 0 ? 
                Query[DefaultTypeQueryParameter].ToArray() : null;
            if (filters != null)
            {
                foreach (string filter in filters)
                {
                    ColumnFilterValue column = CreateColumnData(filter);
                    if (column != ColumnFilterValue.Null)
                        _filterValues.Add(column);
                }
            }
        }

        #endregion

        private ColumnFilterValue CreateColumnData(string queryParameterValue)
        {
            if (string.IsNullOrEmpty(queryParameterValue))
                return ColumnFilterValue.Null;

            string[] data = queryParameterValue.Split(new[] {FilterDataDelimeter}, StringSplitOptions.RemoveEmptyEntries);
            if (data.Length != 3)
                return ColumnFilterValue.Null;
            GridFilterType type;
            if (!Enum.TryParse(data[1], true, out type))
                type = GridFilterType.Equals;

            return new ColumnFilterValue {ColumnName = data[0], FilterType = type, FilterValue = data[2]};
        }

        #region IGridFilterSettings Members

        public IFilterColumnCollection FilteredColumns
        {
            get { return _filterValues; }
        }

        public bool IsInitState(IGridColumn column)
        {
            if(column.InitialFilterSettings == ColumnFilterValue.Null)
            {
                return false;
            }
            else
            {
                return !Query[DefaultClearInitFilterQueryParameter].Any(r => r.Equals(column.Name));
            }
        }

        #endregion
    }
}