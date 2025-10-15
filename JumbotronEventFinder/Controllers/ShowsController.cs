using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using JumbotronEventFinder.Data;
using JumbotronEventFinder.Models;
using System.Configuration;
using System.Reflection.Metadata;

namespace JumbotronEventFinder.Controllers
{
    public class ShowsController : Controller
    {
        private readonly JumbotronEventFinderContext _context;

        public ShowsController(JumbotronEventFinderContext context)
        {
            _context = context;
        }

        // GET: Shows/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Set<Category>(), "CategoryId", "Title");
            
            return View();
        }

        // POST: Shows/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ShowId,Title,Description,Location,Creator,Date,CategoryId,FormFile")] Show show)
        {
            show.CreateDate = DateTime.Now;
            
            if (ModelState.IsValid)
            {
                //
                // Step 1: save the file (optionally)
                //
                if (show.FormFile != null)
                {
                    // Create a unique filename using a Guid          
                    string filename = Guid.NewGuid().ToString() + Path.GetExtension(show.FormFile.FileName); // f81d4fae-7dec-11d0-a765-00a0c91e6bf6.jpg

                    // Initialize the filename in photo record
                    show.Filename = filename;

                    // Get the file path to save the file. Use Path.Combine to handle different OS
                    string saveFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "photos", filename);

                    // Save file
                    using (FileStream fileStream = new FileStream(saveFilePath, FileMode.Create))
                    {
                        await show.FormFile.CopyToAsync(fileStream);
                    }
                }

                //Step 2: Save record to database
                _context.Add(show);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index), "Home");
            }

            ViewData["CategoryId"] = new SelectList(_context.Set<Category>(), "CategoryId", "Title", show.CategoryId);

            return View(show);
        }

        // GET: Shows/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var show = await _context.Show.FindAsync(id);

            if (show == null)
            {
                return NotFound();
            }

            ViewData["CategoryId"] = new SelectList(_context.Set<Category>(), "CategoryId", "Title", show.CategoryId);

            return View(show);
        }

        // POST: Shows/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ShowId,Title,Description,Location,Creator,Filename, Date,CreateDate,CategoryId, FormFile")] Show show)
        {
            if (id != show.ShowId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                //
                // Step 1: save the file (optionally)
                //
                if (show.FormFile != null)
                {
                    // determine new filename         
                    string newFilename = Guid.NewGuid().ToString() + Path.GetExtension(show.FormFile.FileName); // f81d4fae-7dec-11d0-a765-00a0c91e6bf6.jpg

                    //Delete the old file
                    if (!string.IsNullOrEmpty(show.Filename) && show.Filename != newFilename)
                    {
                        string oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "photos", show.Filename);
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                        
                    }

                    // set the new filename in the db record
                    show.Filename = newFilename;
                    
                    // upload the new file
                    string saveFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "photos", newFilename);

                    // Save file
                    using (FileStream fileStream = new FileStream(saveFilePath, FileMode.Create))
                    {
                        await show.FormFile.CopyToAsync(fileStream);
                    }
                }
                
                //
                //Step 2: save in database
                //

                try
                {
                    _context.Update(show);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ShowExists(show.ShowId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                return RedirectToAction(nameof(Index), "Home");
            }

            ViewData["CategoryId"] = new SelectList(_context.Set<Category>(), "CategoryId", "Title", show.CategoryId);

            return View(show);
        }

        // GET: Shows/Delete/5
        public async Task<IActionResult> Delete(int? id)
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

        // POST: Shows/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var show = await _context.Show.FindAsync(id);

            if (show != null)
            {

                if (!string.IsNullOrEmpty(show.Filename))
                {
                    string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "photos", show.Filename);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                _context.Show.Remove(show);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index), "Home");
        }

        private bool ShowExists(int id)
        {
            return _context.Show.Any(e => e.ShowId == id);
        }
    }
}
