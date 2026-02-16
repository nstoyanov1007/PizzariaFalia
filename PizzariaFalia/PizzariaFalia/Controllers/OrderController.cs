using Microsoft.AspNetCore.Mvc;
using PizzariaFalia.Services.Core.Contracts;

namespace PizzariaFalia.Web.Controllers
{
    public class OrderController : Controller
    {
        private readonly IOrderService orderService;

        public OrderController(IOrderService orderService)
        {
            this.orderService = orderService;
        }

        public async Task<IActionResult> Index()
        {
            return View(await orderService.GetAllOrdersAsync());
        }
        public async Task<IActionResult> Details(int id)
        {
            return View(await orderService.GetOrderDetailsAsync(id));
        }
    }
}
