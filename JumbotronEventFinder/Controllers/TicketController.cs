using Microsoft.AspNetCore.Mvc;

namespace JumbotronEventFinder.Controllers
{
    public class TicketController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
