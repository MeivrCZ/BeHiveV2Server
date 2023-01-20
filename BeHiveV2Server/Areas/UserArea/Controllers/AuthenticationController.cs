using BeHiveV2Server.Areas.UserArea.Models;
using BeHiveV2Server.Services.Database.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace BeHiveV2Server.Areas.UserArea.Controllers
{
    public class AuthenticationController : Controller
    {
        private readonly ILogger<AuthenticationController> _logger;
        private readonly UserManager<UserIdentity> _userManager;
        private readonly SignInManager<UserIdentity> _signInManager;

        public AuthenticationController(ILogger<AuthenticationController> logger, UserManager<UserIdentity> userManager, SignInManager<UserIdentity> signInManager)
        {
            _logger = logger;
            _userManager = userManager;
            _signInManager = signInManager;
        }
        
        [AllowAnonymous]
        [HttpGet]
        public IActionResult login()
        {
            if (User.Identity.IsAuthenticated)
            {
                return View();
            }

            return View(new LogInModel());
        }

        [AllowAnonymous]
        [HttpPost]
        public IActionResult login(LogInModel logInData)
        {
            if (User.Identity.IsAuthenticated)
            {
                return Redirect("/index");
            }

            if (ModelState.IsValid)
            {
                var result = _signInManager.PasswordSignInAsync(logInData.UserIdentity, logInData.Password, false, false);

                if (result.Result.Succeeded)
                {
                    return Redirect("/");
                }
                ModelState.AddModelError("loginFaliure", "Invalid Login Attempt");
            }

            return View(logInData);

        }


        [Authorize]
        public IActionResult logout()
        {
            _signInManager.SignOutAsync();
            return Redirect("/");
        }
    }
}
