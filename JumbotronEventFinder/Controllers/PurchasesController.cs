using JumbotronEventFinder.Data;
using JumbotronEventFinder.Models;
using Microsoft.AspNetCore.Mvc;

namespace JumbotronEventFinder.Controllers
{
    public class PurchasesController : Controller
    {
        private readonly JumbotronEventFinderContext _context;
        public PurchasesController(JumbotronEventFinderContext context)
        {
            _context = context;
        }

        //GET: Purchases/Index
        public IActionResult Index(int showId)
        {
            var purchase = new Purchase { ShowId = showId };
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
            if (purchase.CardNumber != 16 || string.IsNullOrWhiteSpace(purchase.CardNumber.ToString()))
            {
                ModelState.AddModelError("CardNumber", "Card number must be exactly 16 digits.");
            }

            if ()
            {
                ModelState.AddModelError("CVV", )
            }

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
