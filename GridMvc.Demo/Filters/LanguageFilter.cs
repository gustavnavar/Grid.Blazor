using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using System.Globalization;

namespace GridMvc.Demo.Filters
{
    public class LanguageFilter : ActionFilterAttribute
    {
        private readonly ILogger _logger;

        public LanguageFilter(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger("LanguageActionFilter");
        }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            string culture;
            if (context.RouteData.Values["culture"] == null)
                culture = "en-US";
           else
                culture = context.RouteData.Values["culture"].ToString();
            _logger.LogInformation($"Setting the culture from the URL: {culture}");

            CultureInfo.CurrentCulture = new CultureInfo(culture);
            CultureInfo.CurrentUICulture = new CultureInfo(culture);

            base.OnActionExecuting(context);
        }
    }
}
