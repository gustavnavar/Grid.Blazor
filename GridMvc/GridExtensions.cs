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
    }
}