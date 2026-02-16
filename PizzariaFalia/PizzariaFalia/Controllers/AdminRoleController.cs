using Microsoft.AspNetCore.Mvc;

namespace PizzariaFalia.Web.Controllers
{
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using PizzariaFalia.Data.Models;

    public class AdminRoleController : Controller
    {
        private readonly UserManager<ApplicationUser> userManager;
        private readonly SignInManager<ApplicationUser> signInManager;

        public AdminRoleController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
        }

        [HttpPost]
        public async Task<IActionResult> MakeAdmin()
        {
            var user = await userManager.GetUserAsync(User);

            if (user != null && !await userManager.IsInRoleAsync(user, "Admin"))
            {
                await userManager.AddToRoleAsync(user, "Admin");

                await signInManager.RefreshSignInAsync(user);
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }

        [HttpPost]
        public async Task<IActionResult> RemoveAdmin()
        {
            var user = await userManager.GetUserAsync(User);

            if (user != null && await userManager.IsInRoleAsync(user, "Admin"))
            {
                await userManager.RemoveFromRoleAsync(user, "Admin");

                await signInManager.RefreshSignInAsync(user);
            }

            return Redirect(Request.Headers["Referer"].ToString());
        }
    }

}
