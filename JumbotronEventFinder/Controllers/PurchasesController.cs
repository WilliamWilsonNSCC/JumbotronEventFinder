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

        //POST: Purchases/Index (Collect data, save non-sensitive, store sensitive in TempData)
        [HttpPost]
        [ValidateAntiForgeryToken]
        // You MUST use Task<IActionResult> because SaveChangesAsync is async
        public async Task<IActionResult> Index(Purchase purchase)
        {
            // 1. Store the sensitive data immediately using TempData (Session)
            //    We check for the input values and store them for the next step.
            TempData["CardNumber"] = purchase.CardNumber;
            TempData["CVV"] = purchase.CVV.HasValue ? purchase.CVV.ToString() : string.Empty;
            TempData["ExpirationDate"] = purchase.ExpirationDate;

            // 2. Clear sensitive data on the model *before* saving to the database
            //    This is crucial for security and preventing FOREIGN KEY conflicts 
            //    if CVV is an int and not int? (as 0 might be invalid).
            purchase.CardNumber = string.Empty;
            purchase.CVV = null;

            // **Important Note:** You must have made 'CVV' in your Purchase model 'int?' 
            // to allow the ModelState.IsValid check to pass here.

            if (ModelState.IsValid)
            {
                // 3. Save the initial, non-sensitive purchase record.
                _context.Add(purchase);
                //await _context.SaveChangesAsync();

                // 4. Redirect to Details using the newly generated PurchaseId
                return RedirectToAction("Details", new { id = purchase.PurchaseId });
            }

            // If ModelState fails, return to the form
            return View(purchase);
        }


        //GET: Purchase/details (check that details are correct before continuing)
        // FIX: Must be async Task<IActionResult> and use await
        public async Task<IActionResult> Details(int id)
        {
            // FIX: Use await to get the actual Purchase object, not the Task object.
            var purchase = await _context.Purchase.FirstOrDefaultAsync(p => p.PurchaseId == id);

            if (purchase == null)
            {
                return NotFound();
            }

            // 5. Retrieve sensitive data from TempData and populate the model for the view
            if (TempData.Peek("CardNumber") is string cardNumber)
            {
                purchase.CardNumber = cardNumber;
            }
            // Use TryParse since TempData is string and CVV in the model is int?
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
