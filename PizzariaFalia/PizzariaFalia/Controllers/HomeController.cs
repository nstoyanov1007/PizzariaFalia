using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PizzariaFalia.ViewModels;

namespace PizzariaFalia.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return RedirectToAction("Index", "Menu");
        }
        public IActionResult Privacy()
        {
            return View();
        }
        [Route("Home/Error")]
        public IActionResult Error(int? statusCode = null)
        {
            ViewBag.StatusCode = statusCode;

            switch (statusCode)
            {
                case 404:
                    ViewBag.ErrorMessage = "Page not found.";
                    return View("Error_404");
                case 403:
                    ViewBag.ErrorMessage = "Access denied.";
                    return View("Error_403");
                case 500:
                    ViewBag.ErrorMessage = "Internal Server Error.";
                    return View("Error_500");

            }
            //if no status , go to default handler
            var exceptionFeature = HttpContext.Features.Get<
                Microsoft.AspNetCore.Diagnostics.IExceptionHandlerPathFeature>();

            if (exceptionFeature != null)
            {
                ViewBag.ErrorMessage = exceptionFeature.Error.Message;
                ViewBag.Path = exceptionFeature.Path;
            }

            return View();
        }
    }

}
