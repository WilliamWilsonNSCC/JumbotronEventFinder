using JumbotronEventFinder.Data;
using JumbotronEventFinder.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

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
        public async Task<IActionResult> Index(int showId)
        {
            var show = await _context.Show.FindAsync(showId);
            if(show == null)
            {
                TempData["Error"] = "Show not found!";
                return RedirectToAction("Index", "Home");
            }
            var purchase = new Purchase { ShowId = showId };
            return View(purchase); //show form
        }

        //POST: Purchases/Index (Collect data, save non-sensitive, store sensitive in TempData)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Index(Purchase purchase)
        {
            var show = await _context.Show.FindAsync(purchase.ShowId);
            if (show == null)
            {
                ModelState.AddModelError("ShowId", "Invalid show selected.");
                return View(purchase);
            }

            // Validate credit card format before proceeding
            if (string.IsNullOrWhiteSpace(purchase.CardNumber) || purchase.CardNumber.Length != 16)
            {
                ModelState.AddModelError("CardNumber", "Card number must be exactly 16 digits.");
            }

            if (!purchase.CVV.HasValue || purchase.CVV < 100 || purchase.CVV > 999)
            {
                ModelState.AddModelError("CVV", "CVV must be exactly 3 digits.");
            }

            if (string.IsNullOrWhiteSpace(purchase.ExpirationDate) ||
                !System.Text.RegularExpressions.Regex.IsMatch(purchase.ExpirationDate, @"^(0[1-9]|1[0-2])\/\d{2}$"))
            {
                ModelState.AddModelError("ExpirationDate", "Expiration must be in MM/YY format.");
            }

            if (!ModelState.IsValid)
            {
                return View(purchase);
            }

            TempData["CardNumber"] = purchase.CardNumber;
            TempData["CVV"] = purchase.CVV.ToString(); 
            TempData["ExpirationDate"] = purchase.ExpirationDate;

            purchase.CardNumber = string.Empty;
            purchase.CVV = null;
            purchase.ExpirationDate = string.Empty;

            _context.Add(purchase);
            await _context.SaveChangesAsync();

            return RedirectToAction("Details", new { id = purchase.PurchaseId });
        }


        //GET: Purchase/details (check that details are correct before continuing)
        public async Task<IActionResult> Details(int id)
        {
            // FIX: Use await to get the actual Purchase object, not the Task object.
            var purchase = await _context.Purchase
                .Include(p => p.Show)
                .FirstOrDefaultAsync(p => p.PurchaseId == id);

            if (purchase == null)
            {
                return NotFound();
            }

            if (TempData.Peek("CardNumber") is string cardNumber)
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

        //POST: Purchases/Confirm (Final confirmation and payment processing)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Confirm(int id)
        {
            var purchase = await _context.Purchase.FindAsync(id);

            if (purchase == null)
            {
                return NotFound();
            }

            // Retrieve payment details from TempData
            var cardNumber = TempData["CardNumber"] as string;
            var cvvString = TempData["CVV"] as string;
            var expirationDate = TempData["ExpirationDate"] as string;

            if (string.IsNullOrEmpty(cardNumber) || string.IsNullOrEmpty(cvvString))
            {
                TempData["Error"] = "Payment information expired. Please try again.";
                return RedirectToAction("Index", new { showId = purchase.ShowId });
            }

            purchase.PaymentType = purchase.PaymentType; // Already saved
            await _context.SaveChangesAsync();

            TempData["Success"] = "Purchase completed successfully!";
            return RedirectToAction("Success", new { id = purchase.PurchaseId });
        }

        //GET: Purchases/Success
        public async Task<IActionResult> Success(int id)
        {
            var purchase = await _context.Purchase
                .Include(p => p.Show)
                .FirstOrDefaultAsync(p => p.PurchaseId == id);

            if (purchase == null)
            {
                return NotFound();
            }

            return View(purchase);
        }
    }
}
