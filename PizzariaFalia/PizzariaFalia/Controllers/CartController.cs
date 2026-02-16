using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using PizzariaFalia.Data.Models;
using PizzariaFalia.Services.Core;
using PizzariaFalia.Services.Core.Contracts;
using PizzariaFalia.ViewModels;

namespace PizzariaFalia.Web.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService cartService;
        private readonly UserManager<ApplicationUser> userManager;

        public CartController(ICartService cartService,
                              UserManager<ApplicationUser> userManager)
        {
            this.cartService = cartService;
            this.userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var userId = userManager.GetUserId(User);

            var items = await cartService.GetCartItemsAsync(userId);

            return View(items);
        }

        [HttpPost]
        public async Task<IActionResult> AddFromDetails(int dishId, bool isBig)
        {
            var userId = userManager.GetUserId(User);
            var dishDetails = await cartService.GetDishDetailsAsync(dishId);

            dishDetails.IsBig = isBig;

            await cartService.AddItemToCartAsync(dishDetails, userId);

            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> Remove(OrderItemViewModel item)
        {
            var userId = userManager.GetUserId(User);

            await cartService.RemoveItemFromCartAsync(item, userId);

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> PlaceOrder()
        {
            var userId = userManager.GetUserId(User);

            await cartService.PlaceCartOrderAsync(userId);

            return RedirectToAction("Index", "Menu");
        }
    }

}
