using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using PizzariaFalia.Services.Core.Contracts;
using PizzariaFalia.ViewModels;

namespace PizzariaFalia.Web.Areas.Administration.Controllers
{
    [Authorize(Roles = "Admin")]
    [Area("Administration")]
    public class HomeController : Controller
    {
        private readonly IAdminMenuChangeService adminService;
        private readonly IMenuService menuService;
        public HomeController(IAdminMenuChangeService adminService, IMenuService menuService)
        {
            this.adminService = adminService;
            this.menuService = menuService;
        }
        public IActionResult Index()
        {
            return RedirectToAction(nameof(Dashboard), "Home", new { area = "Administration" });
        }

        public IActionResult Dashboard()
        {
            return View();
        }

        public async Task<IActionResult> CreateCategory()
        {
            var categories = await menuService.GetAllCategoriesAsync();

            ViewData["Categories"] = new SelectList(categories, "Id", "Name");

            return View(new CategoryFormViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> CreateCategory(CategoryFormViewModel vm)
        {
            if (ModelState.IsValid)
            {
                await adminService.CreateCategoryAsync(vm);

                return RedirectToAction(nameof(Dashboard));
            }
            else
            {
                return View(vm);
            }
        }


        public async Task<IActionResult> CreateDish()
        {
            var categories = await menuService.GetAllCategoriesAsync();

            var flatList = categories
                .SelectMany(parent =>
                    new[] { parent }
                    .Concat(parent.Children ?? Enumerable.Empty<CategoryTreeViewModel>())
                )
                .ToList();

            ViewData["Categories"] = new SelectList(flatList, "Id", "Name");
            return View(new DishFormViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> CreateDish(DishFormViewModel vm)
        {
            if (ModelState.IsValid)
            {
                await adminService.CreateDishAsync(vm);

                return RedirectToAction(nameof(Dashboard));
            }
            else
            {
                return View(vm);
            }
        }


        public async Task<IActionResult> DeleteDish(int dishId)
        {
            Console.WriteLine(dishId);
            await adminService.DeleteDishAsync(dishId);

            return RedirectToAction("Index", "Home", new { area = "" });
        }
        public async Task<IActionResult> DeleteCategory(int dishId)
        {
            return RedirectToAction("Index", "Home", new { area = "" });
        }

        public async Task<IActionResult> EditDish(int id)
        {
            var details = await menuService.GetDishDetailsAsync(id);

            var categories = await menuService.GetAllCategoriesAsync();

            var flatList = categories
                .SelectMany(parent =>
                    new[] { parent }
                    .Concat(parent.Children ?? Enumerable.Empty<CategoryTreeViewModel>())
                )
                .ToList();

            ViewData["Categories"] = new SelectList(flatList, "Id", "Name");

            return View("EditDish", new DishFormViewModel()
            {
                PriceSmall = details.PriceSmall,
                GramsSmall = details.GramsSmall,
                Description = details.Description,
                GramsBig = details.GramsBig,
                PriceBig = details.PriceBig,
                Id = id,
                Name = details.Name,
                CategoryId = details.CategoryId ?? 1
            });
        }
        [HttpPost]
        public async Task<IActionResult> EditDish(DishFormViewModel model)
        {
            await adminService.EditDishAsync(model);

            return RedirectToAction("Index", "Home", new { area = "" });
        }

    }
}
