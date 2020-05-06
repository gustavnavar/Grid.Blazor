﻿using GridShared.Columns;
using GridShared.Filtering;
using GridShared.Utility;
using Microsoft.Extensions.Primitives;
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
        private readonly DefaultFilterColumnCollection _filterValues = new DefaultFilterColumnCollection();

        #region Ctor's

        public QueryStringFilterSettings(IQueryDictionary<StringValues> query)
        {
            if (query == null)
                throw new ArgumentException("No http context here!");
            Query = query;

            var filters = Query.Get(DefaultTypeQueryParameter);
            if (filters.Count > 0)
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
            if (data.Length != 2 && data.Length != 3)
                return ColumnFilterValue.Null;
            GridFilterType type;
            if (!Enum.TryParse(data[1], true, out type))
                type = GridFilterType.Equals;

            if (data.Length == 2)
                return new ColumnFilterValue { ColumnName = data[0], FilterType = type, FilterValue = String.Empty };
            else
                return new ColumnFilterValue { ColumnName = data[0], FilterType = type, FilterValue = data[2] };
        }

        #region IGridFilterSettings Members

        public IQueryDictionary<StringValues> Query { get; }

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
                return !Query.Get(DefaultClearInitFilterQueryParameter).Any(r => r.Equals(column.Name));
            }
        }

        #endregion
    }
}