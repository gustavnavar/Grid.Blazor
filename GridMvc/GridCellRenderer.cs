using GridShared;
using GridShared.Columns;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.IO;
using System.Text.Encodings.Web;

namespace GridMvc
{
    public class GridCellRenderer : GridStyledRenderer, IGridCellRenderer
    {
        private const string TdClass = "grid-cell";

        public GridCellRenderer()
        {
            AddCssClass(TdClass);
        }

        public IHtmlContent Render(IGridColumn column, object item)
        {
            IGridCell cell = column.GetCell(item);
            string cssStyles = GetCssStylesString();
            string cssClass = GetCssClassesString();
            string columnCssClasses = column.GetCellCssClasses(item);

            var builder = new TagBuilder("td");
            if (!string.IsNullOrWhiteSpace(cssClass))
                builder.AddCssClass(cssClass);
            if (!string.IsNullOrWhiteSpace(columnCssClasses))
                builder.AddCssClass(columnCssClasses);
            if (!string.IsNullOrWhiteSpace(cssStyles))
                builder.MergeAttribute("style", cssStyles);
            builder.MergeAttribute("data-name", column.Name);

            builder.InnerHtml.SetHtmlContent(cell.ToString());

            using (var sw = new StringWriter())
            {
                builder.WriteTo(sw, HtmlEncoder.Default);
                return new HtmlString(sw.ToString());
            }
        }
    }
}