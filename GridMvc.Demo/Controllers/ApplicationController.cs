using GridMvc.Demo.Filters;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;
using System.IO;
using System.Threading.Tasks;

namespace GridMvc.Demo.Controllers
{
    [ServiceFilter(typeof(LanguageFilter))]
    public abstract class ApplicationController : Controller
    {
        public readonly ICompositeViewEngine _compositeViewEngine;

        public ApplicationController(ICompositeViewEngine compositeViewEngine)
        {
            _compositeViewEngine = compositeViewEngine;
        }

        protected async Task<string> RenderAsync(string viewName, object model)
        {
            if (string.IsNullOrEmpty(viewName))
                viewName = ControllerContext.RouteData.Values["action"].ToString();

            ViewEngineResult viewResult = _compositeViewEngine.FindView(ControllerContext, viewName, false);

            var viewDictionary = new ViewDataDictionary(new EmptyModelMetadataProvider(), new ModelStateDictionary())
            {
                Model = model
            };

            using (var sw = new StringWriter())
            {
                var newViewContext = new ViewContext(
                    ControllerContext,
                    viewResult.View,
                    viewDictionary,
                    TempData,
                    sw,
                    new HtmlHelperOptions());
                await viewResult.View.RenderAsync(newViewContext);
                return sw.ToString();
            }
        }
    }
}