using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzariaFalia.Services.Core.Contracts;
using PizzariaFalia.ViewModels;

namespace PizzariaFalia.Web.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminMenuController : Controller
    {
        private readonly IAdminMenuChangeService adminService;

        public AdminMenuController(IAdminMenuChangeService adminService)
        {
            this.adminService = adminService;
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public IActionResult CreateCategory()
            => View();

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CategoryFormViewModel vm)
        {
            await adminService.CreateCategoryAsync(vm);
            return RedirectToAction(nameof(Dashboard));
        }

        public IActionResult CreateDish()
            => View();

        [HttpPost]
        public async Task<IActionResult> CreateDish(DishFormViewModel vm)
        {
            await adminService.CreateDishAsync(vm);
            return RedirectToAction(nameof(Dashboard));
        }
    }

}
