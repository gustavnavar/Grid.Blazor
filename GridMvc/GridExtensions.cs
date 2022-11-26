using GridShared.Columns;
using GridMvc.Html;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Http;
using GridCore;
using Microsoft.Extensions.Primitives;
using GridShared.Utility;
using GridShared.Sorting;
using System.Linq;

namespace GridMvc
{
    public static class GridExtensions
    {
        public static HtmlGrid<T> Grid<T>(this IHtmlHelper helper, IEnumerable<T> items, IViewEngine viewEngine = null)
        {
            return Grid(helper, items, GridRenderOptions.DefaultPartialViewName);
        }

        public static HtmlGrid<T> Grid<T>(this IHtmlHelper helper, IEnumerable<T> items, IQueryCollection query, IViewEngine viewEngine = null)
        {
            return Grid(helper, items, query, GridRenderOptions.DefaultPartialViewName);
        }

        public static HtmlGrid<T> Grid<T>(this IHtmlHelper helper, IEnumerable<T> items, string viewName, IViewEngine viewEngine = null)
        {
            var newGrid = new SGrid<T>(items, helper.ViewContext.HttpContext.Request.Query);
            var htmlGrid = new HtmlGrid<T>(helper, newGrid, viewName);
            return htmlGrid;
        }

        public static HtmlGrid<T> Grid<T>(this IHtmlHelper helper, IEnumerable<T> items, IQueryCollection query, string viewName, IViewEngine viewEngine = null)
        {
            var newGrid = new SGrid<T>(items, query);
            var htmlGrid = new HtmlGrid<T>(helper, newGrid, viewName);
            return htmlGrid;
        }

        public static HtmlGrid<T> Grid<T>(this IHtmlHelper helper, SGrid<T> sourceGrid, IViewEngine viewEngine = null)
        {
            //wrap source grid:
            var htmlGrid = new HtmlGrid<T>(helper, sourceGrid, GridRenderOptions.DefaultPartialViewName);
            return htmlGrid;
        }

        public static HtmlGrid<T> Grid<T>(this IHtmlHelper helper, SGrid<T> sourceGrid, string viewName, IViewEngine viewEngine)
        {
            //wrap source grid:
            var htmlGrid = new HtmlGrid<T>(helper, sourceGrid, viewName);
            return htmlGrid;
        }

        //support IHtmlString in RenderValueAs method
        public static IGridColumn<T> RenderValueAs<T>(this IGridColumn<T> column, 
            Func<T, IHtmlContent> constraint)
        {
            Func<T, string> valueContraint = a =>
                {
                    using (var sw = new StringWriter())
                    {
                        constraint(a).WriteTo(sw, HtmlEncoder.Default);
                        return sw.ToString();
                    }
                };
            return column.RenderValueAs(valueContraint);
        }

        //support WebPages inline helpers
        public static IGridColumn<T> RenderValueAs<T>(this IGridColumn<T> column,
            Func<T, Func<object, HelperResult>> constraint)
        {
            Func<T, string> valueContraint = a =>
            {
                using (var sw = new StringWriter())
                {
                    constraint(a)(null).WriteTo(sw, HtmlEncoder.Default);
                    return sw.ToString();
                }
            };
            return column.RenderValueAs(valueContraint);
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
                    ColumnOrderValue column = QueryDictionary<StringValues>.CreateColumnData(sorting);
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
                dictionary.Add(ColumnOrderValue.DefaultSortingQueryParameter, QueryDictionary<StringValues>.CreateStringValues(sortValues));
            }

            return dictionary;
        }
    }
}