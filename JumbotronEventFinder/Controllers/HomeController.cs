using JumbotronEventFinder.Data;
using JumbotronEventFinder.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace JumbotronEventFinder.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly JumbotronEventFinderContext _context;
        public HomeController(ILogger<HomeController> logger, JumbotronEventFinderContext context)
        {
            _logger = logger;
            _context = context;
        }

        // GET: Shows
        public async Task<IActionResult> Index()
        {
            var shows = await _context.Show
                .OrderByDescending(m => m.Date)
                .Include(s => s.Category)
                .ToListAsync();

            return View(shows);
        }

        // GET: Shows/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var show = await _context.Show
                .Include(s => s.Category)
                .FirstOrDefaultAsync(m => m.ShowId == id);

            if (show == null)
            {
                return NotFound();
            }

            return View(show);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
