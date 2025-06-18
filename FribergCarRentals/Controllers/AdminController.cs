using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace FribergCarRentals.Controllers
{
    public class AdminController : Controller
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly UserManager<IdentityUser> _userManager;

        public AdminController(SignInManager<IdentityUser> signInManager, UserManager<IdentityUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        [HttpGet("/admin")]
        public IActionResult Index()
        {
            return View();
        }

        [HttpPost("/admin")]
        public async Task<IActionResult> Index(string email, string password)
        {
            var user = await _userManager.FindByEmailAsync(email);

            if (user != null && await _userManager.IsInRoleAsync(user, "Admin"))
            {
                var result = await _signInManager.PasswordSignInAsync(user, password, false, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Dashboard");
                }
            }

            ModelState.AddModelError("", "Ogiltig inloggning eller ej behörig.");
            return View();
        }

        [HttpGet]
        public IActionResult Dashboard()
        {
            return View();
        }
    }
}
