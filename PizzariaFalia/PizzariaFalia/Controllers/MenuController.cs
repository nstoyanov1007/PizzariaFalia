using Microsoft.AspNetCore.Mvc;
using PizzariaFalia.Services.Core.Contracts;
using PizzariaFalia.ViewModels;

namespace PizzariaFalia.Web.Controllers
{
    public class MenuController : Controller
    {
        private readonly IMenuService menuService;

        public MenuController(IMenuService menuService)
        {
            this.menuService = menuService;
        }

        public async Task<IActionResult> Index(int? categoryId)
        {
            var categories = await menuService.GetAllCategoriesAsync();

            IEnumerable<DishIndexViewModel> dishes;

            if (categoryId.HasValue)
            {
                dishes = await menuService.GetDishesIndexByCategoryAsync(categoryId.Value);
            }
            else
            {
                dishes = await menuService.GetAllDishesIndexAsync();
            }

            var model = new MenuPageViewModel
            {
                Categories = categories,
                Dishes = dishes,
                SelectedCategoryId = categoryId
            };

            return View(model);
        }

        public async Task<IActionResult> Details(int id)
        {
            var dish = await menuService.GetDishDetailsAsync(id);
            return View(dish);
        }
    }
}
