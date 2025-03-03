using Microsoft.AspNetCore.Mvc;

namespace BW4_BE_Grp2.Controllers
{
    public class CarrelloController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
