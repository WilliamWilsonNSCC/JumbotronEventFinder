using JumbotronEventFinder.Data;
using JumbotronEventFinder.Models;
using Microsoft.AspNetCore.Mvc;

namespace JumbotronEventFinder.Controllers
{
    public class PurchaseController : Controller
    {
        private readonly JumbotronEventFinderContext _context;
        public PurchaseController(JumbotronEventFinderContext context)
        {
            _context = context;
        }

        //GET: Purchase/Index
        public IActionResult Index(int showId, string title)
        {
            var purchase = new Purchase { ShowId = showId };
            ViewBag.ShowTitle = title;
            return View(purchase); //show form
        }

        //POST: Purchases/Index (collect data then check for confirmation)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Index(Purchase purchase)
        {
            if (ModelState.IsValid)
            {
                return RedirectToAction("Details", purchase);
            }
            return View(purchase);
        }

        //POST: Purchases/Confirm (final sale)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm(Purchase purchase)
        {
            if (ModelState.IsValid)
            {
                _context.Add(purchase);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index", "Home");
            }
            return RedirectToAction("Index", purchase.ShowId);
        }
    }
}
