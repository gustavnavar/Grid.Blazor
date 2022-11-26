using GridShared.Sorting;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;

namespace GridShared.Utility
{
    public class QueryDictionary<T> : Dictionary<string, T>, IQueryDictionary<T>
    {
        public T Get(string key)
        {
            return this.Get<string,T>(key);
        }

        public void AddParameter(string parameterName, T parameterValue)
        {      
            if (string.IsNullOrEmpty(parameterName))
                throw new ArgumentException("parameterName");

            this.AddOrSet(parameterName, parameterValue);
        }

        public static ColumnOrderValue CreateColumnData(string queryParameterValue)
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

        public static StringValues CreateStringValues(IEnumerable<ColumnOrderValue> columns)
        {
            List<string> values = new List<string>();
            foreach (var column in columns)
            {
                values.Add(column.ToString());
            }
            return new StringValues(values.ToArray());
        }
    }
}
