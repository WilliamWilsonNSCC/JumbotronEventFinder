using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using JumbotronEventFinder.Models;

namespace JumbotronEventFinder.Controllers
{
    public class EventsController : Controller
    {
        private readonly ILogger<EventsController> _logger;
        public EventsController(ILogger<EventsController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            List<Events> events = new List<Events>
            {
                new() {
                   EventId = 1,
                   Title = "Super Mario Band",
                   Description = "Mario meets Metal",
                   Location = "Halifax, NS - Scotiabank Centre",
                   Date = new DateTime(2026,02,28,20,00,00),
                   CategoryId = 1,
                },
                new() {
                   EventId = 2,
                   Title = "WEB:APP:DEV",
                   Description = "Web Dev Showcase",
                   Location = "Toronto, ON - Metro Toronto Convention Centre",
                   Date = new DateTime(2026,03,20,9,00,00),
                   CategoryId = 2,
                },
                new() {
                   EventId = 1,
                   Title = "Super Mario Band",
                   Description = "Mario meets metal",
                   Location = "Montreal, QC - Centre Bell",
                   Date = new DateTime(2026,03,25,20,00,00),
                   CategoryId = 1,
                }
            };
            return View(events);
        }

        public IActionResult Sponsors()
        {
            return View();
        }

        public IActionResult Events()
        {
            return View();
        }
    }
}
