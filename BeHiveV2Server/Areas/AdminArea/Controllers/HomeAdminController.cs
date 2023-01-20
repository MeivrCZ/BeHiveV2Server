using Microsoft.AspNetCore.Mvc;

namespace BeHiveV2Server.Areas.AdminArea.Controllers
{
    public class HomeAdminController : Controller
    {
        public IActionResult Admin()
        {
            return View();
        }
    }
}
