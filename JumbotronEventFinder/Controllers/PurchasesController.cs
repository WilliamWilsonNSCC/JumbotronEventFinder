using JumbotronEventFinder.Data;
using JumbotronEventFinder.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JumbotronEventFinder.Controllers
{
    [Authorize]
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
        public async Task<IActionResult> Index(Purchase purchase)
        {
            TempData["CardNumber"] = purchase.CardNumber;
            TempData["CardNumber"] = purchase.CVV.HasValue ? purchase.CVV.ToString() : string.Empty;
            TempData["ExpirationDate"] = purchase.ExpirationDate;

            purchase.CardNumber = string.Empty;
            purchase.CVV = null;


            if (ModelState.IsValid)
            {
                _context.Add(purchase);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = purchase.PurchaseId });
            }

            return View(purchase);
        }


        //GET: Purchase/details (check that details are correct before continuing)
        public async Task<IActionResult> Details(int id)
        {
            var purchase = await _context.Purchase.FirstOrDefaultAsync(p => p.PurchaseId == id);
            if (purchase == null)
            {
                return NotFound();
            }

            if(TempData.Peek("CardNumber") is string cardNumber)
            {
                purchase.CardNumber = cardNumber;
            }
            if (TempData.Peek("CVV") is string cvvString && int.TryParse(cvvString, out int cvv))
            {
                purchase.CVV = cvv;
            }
            if (TempData.Peek("ExpirationDate") is string expDate)
            {
                purchase.ExpirationDate = expDate;
            }

            return View(purchase);
        }

        //POST: Purchases/Confirm (final sale)
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Confirm(Purchase purchase)
        //{
        //    if (string.IsNullOrWhiteSpace(purchase.CardNumber) || purchase.CardNumber.Length != 16)
        //    {
        //        ModelState.AddModelError("CardNumber", "Card number must be exactly 16 digits.");
        //    }

        //    if (purchase.CVV < 100 || purchase.CVV > 999)
        //    {
        //        ModelState.AddModelError("CVV", "CVV must be exactly 3 digits.");
        //    }

        //    if (string.IsNullOrWhiteSpace(purchase.ExpirationDate) || !System.Text.RegularExpressions.Regex.IsMatch(purchase.ExpirationDate, @"^(0[1-9]|1[0-2])\/\d{2}$"))
        //    {
        //        ModelState.AddModelError("Expiration", "Expiration must be in MM/YY format.");
        //    }

        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(purchase);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction("Index", new { purchase.ShowId });
        //    }
        //    return RedirectToAction("Index", purchase.ShowId);
        //}
    }
}
