using GridShared.Sorting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public static QueryDictionary<StringValues> Convert(IQueryCollection collection)
        {
            QueryDictionary<StringValues> dictionary = new QueryDictionary<StringValues>();
            foreach (var element in collection)
            {
                if (dictionary.ContainsKey(element.Key))
                    dictionary[element.Key] = StringValues.Concat(dictionary[element.Key], element.Value);
                else
                    dictionary.Add(element.Key, element.Value);
            }

            // creare a consecutive sortValues  
            var sortings = dictionary.Get(ColumnOrderValue.DefaultSortingQueryParameter);
            if (sortings.Count > 0)
            {
                // get sortValues from Query
                var sortValues = new DefaultOrderColumnCollection();
                foreach (string sorting in sortings)
                {
                    ColumnOrderValue column = CreateColumnData(sorting);
                    if (column != ColumnOrderValue.Null)
                        sortValues.Add(column);
                }

                // creare a consecutive sortValues  
                var sortList = sortValues.OrderBy(r => r.Id).ToList();
                int i = 0;
                sortValues = new DefaultOrderColumnCollection();
                foreach (var sortItem in sortList)
                {
                    i++;
                    var column = new ColumnOrderValue
                    {
                        ColumnName = sortItem.ColumnName,
                        Direction = sortItem.Direction,
                        Id = i
                    };
                    sortValues.Add(column);
                }

                // update query with new sortValues
                dictionary.Remove(ColumnOrderValue.DefaultSortingQueryParameter);
                dictionary.Add(ColumnOrderValue.DefaultSortingQueryParameter, CreateStringValues(sortValues));
            }

            return dictionary;
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

        private static StringValues CreateStringValues(IEnumerable<ColumnOrderValue> columns)
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
