using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace GridBlazorClientSide.Server.Controllers
{
    [Route("api/[controller]")]
    public class CultureController : Controller
    {
        [HttpGet("[action]")]
        public ActionResult GetCulture()
        {
            var cultureCookie = HttpContext.Request.Cookies[CookieRequestCultureProvider.DefaultCookieName];
            var culture = CookieRequestCultureProvider.ParseCookieValue(cultureCookie);
            if(culture == null || culture.Cultures.Count == 0)
                return Ok();
            return Ok(culture.Cultures.First().Value);
        }

        [HttpPost("[action]")]
        public ActionResult SetCulture(string culture)
        {
            if (culture != null)
            {
                HttpContext.Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName,
                    CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)));
                return NoContent();
            }
            return BadRequest(new
            {
                message = "ModelState is not valid"
            });
        }
    }
}
