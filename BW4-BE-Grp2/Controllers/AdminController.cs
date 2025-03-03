using Microsoft.AspNetCore.Mvc;

namespace BW4_BE_Grp2.Controllers
{
    public class AdminController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
