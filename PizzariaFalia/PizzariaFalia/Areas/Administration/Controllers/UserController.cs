using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PizzariaFalia.Services.Core.Contracts;
using PizzariaFalia.ViewModels;

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

            public async Task<IActionResult> Index(int page = 1)
            {
                int pageSize = 10;

                var model = await userService.GetUsersPagedAsync(page, pageSize);

                return View(model);
            }

            public async Task<IActionResult> Details(Guid id)
            {
                var user = await userService.GetUserDetailsAsync(id);
                return View(user);
            }

            public async Task<IActionResult> Edit(Guid id)
            {
                var model = await userService.GetUserForEditAsync(id);
                return View(model);
            }
            [HttpPost]
            public async Task<IActionResult> Edit(UserEditViewModel model)
            {
                if (!ModelState.IsValid)
                {
                    return View(model);
                }

                await userService.EditUserAsync(model);

                return RedirectToAction(nameof(Index));
            }
        }
    }
}
