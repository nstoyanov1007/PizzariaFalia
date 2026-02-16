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
    }

}
