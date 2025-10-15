using Microsoft.AspNetCore.Mvc;

namespace JumbotronEventFinder.Controllers
{
    public class AccountController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
