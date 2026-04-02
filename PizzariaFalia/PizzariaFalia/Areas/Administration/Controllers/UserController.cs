using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzariaFalia.Services.Core.Contracts;

namespace PizzariaFalia.Web.Areas.Administration.Controllers
{
    namespace PizzariaFalia.Web.Areas.Administration.Controllers
    {
        [Authorize(Roles = "Admin")]
        [Area("Administration")]
        public class UserController : Controller
        {
            private readonly IUserManagerService userService;

            public UserController(IUserManagerService userService)
            {
                this.userService = userService;
            }

            public async Task<IActionResult> Index()
            {
                var users = await userService.GetAllUsersAsync();
                return View(users);
            }

            public async Task<IActionResult> Details(Guid id)
            {
                var user = await userService.GetUserDetailsAsync(id);
                return View(user);
            }
        }
    }
}
