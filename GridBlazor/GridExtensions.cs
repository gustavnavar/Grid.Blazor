using GridShared.Columns;
using Microsoft.AspNetCore.Html;
using System;
using System.IO;
using System.Text.Encodings.Web;

namespace GridBlazor
{
    public static class GridExtensions
    {
        //support IHtmlString in RenderValueAs method
        public static IGridColumn<T> RenderValueAs<T>(this IGridColumn<T> column, Func<T, IHtmlContent> constraint)
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
                                                      Func<T, Func<object, IHtmlContent>> constraint)
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