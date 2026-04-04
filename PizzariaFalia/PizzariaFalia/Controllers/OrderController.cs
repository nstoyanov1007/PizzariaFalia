using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzariaFalia.Data.Models.Enums;
using PizzariaFalia.Services.Core.Contracts;
using System.Security.Claims;

namespace PizzariaFalia.Web.Controllers
{
    [Authorize]
    public class OrderController : Controller
    {
        private readonly IOrderService orderService;

        public OrderController(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await orderService.GetAllOrdersAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)));
        }
        public async Task<IActionResult> Details(int id)
        {
            return View(await orderService.GetOrderDetailsAsync(id));
        }
        public async Task<IActionResult> Cancel(int id)
        {
            await orderService.ChangeOrderStatusAsync(id, Status.Cancelled);

            return RedirectToAction("Index");
        }
    }
}
