using Microsoft.AspNetCore.Mvc;

namespace BeHiveV2Server.Areas.UserArea.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult profile()
        {
            return View();
        }
        public IActionResult settings()
        {
            return View();
        }
    }
}
