using GridMvc.Resources;
using GridShared.Utility;
using Microsoft.AspNetCore.Html;
using System.Collections.Generic;

namespace GridMvc.Searching
{
    /// <summary>
    ///     Renderer for search widget.
    /// </summary>
    internal class QueryStringSearchHeaderRenderer : IGridSearchHeaderRenderer
    {
        private readonly QueryStringSearchSettings _settings;

        public QueryStringSearchHeaderRenderer(QueryStringSearchSettings settings)
        {
            _settings = settings;
        }

        public IHtmlContent Render()
        {
            return new HtmlString(GetSearchHeaderContent());
        }

        protected string GetSearchHeaderContent()
        {

            return
                "<div class=\"row grid-search\" data-search-url=\"" + GetSeachUrl() + "\" >" +
                    "<div class=\"col-md-offset-6 offset-md-6 col-md-6\">" +
                        "<div class=\"input-group\">" +
                            "<span class=\"input-group-btn input-group-prepend\">" +
                                "<button class=\"btn btn-default btn-outline-secondary grid-search-apply\" type=\"button\"></button>" +
                            "</span>" +
                            "<input type=\"text\" class=\"form-control grid-search-input\" value=\"" + _settings.SearchValue + "\"  placeholder=\"" + Strings.SearchFor+ "\" />" +
                            "<span class=\"input-group-btn input-group-append\">" +
                                "<button class=\"btn btn-default btn-outline-secondary grid-search-clear\" type=\"button\"></button>" + 
                            "</span>" + 
                        "</div>" + 
                    "</div>" + 
                "</div>";
        }

        private string GetSeachUrl()
        {
            //determine current url:
            var builder = new CustomQueryStringBuilder(_settings.Query);

            var exceptQueryParameters = new List<string>();
            return builder.GetQueryStringExcept(exceptQueryParameters);
        }
    }
}