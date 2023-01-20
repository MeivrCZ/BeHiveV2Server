using Microsoft.AspNetCore.Mvc;

namespace BeHiveV2Server.Controllers
{
    public class MainController : Controller
    {
        public IActionResult index()
        {
            return View();
        }
    }
}
