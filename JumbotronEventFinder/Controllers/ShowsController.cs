using Azure.Storage.Blobs;
using Azure.Storage.Blobs.Models;
using JumbotronEventFinder.Data;
using JumbotronEventFinder.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace JumbotronEventFinder.Controllers
{
    [Authorize]
    public class ShowsController : Controller
    {
        private readonly IConfiguration _configuration;
        private readonly JumbotronEventFinderContext _context;

        private readonly BlobContainerClient _containerClient;

        public ShowsController(IConfiguration configuration, JumbotronEventFinderContext context)
        {
            _context = context;
            _configuration = configuration;

            //Setup blob container client
            var connectionString = _configuration["AzureStorage"];
            var containerName = "jumbotron-event-uploads";
            _containerClient = new BlobContainerClient(connectionString, containerName);
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
                    //
                    // Upload file to Azure Blob Storage
                    //

                    // store the file to upload in fileUpload
                    IFormFile fileUpload = show.FormFile;

                    // create a unique filename for the blob
                    string blobName = Guid.NewGuid().ToString() + "_" + fileUpload.FileName;

                    var blobClient = _containerClient.GetBlobClient(blobName);

                    using (var stream = fileUpload.OpenReadStream())
                    {
                        await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = fileUpload.ContentType });
                    }

                    string blobURL = blobClient.Uri.ToString();

                    // assign the blob URL to the record to save in Db
                    show.Filename = blobURL;
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
                var existingShow = await _context.Show.AsNoTracking().FirstOrDefaultAsync(s => s.ShowId == id);

                if (existingShow == null)
                {
                    return NotFound();
                }

                show.Filename = existingShow.Filename;

                //
                // Step 1: save the file (optionally)
                //
                if (show.FormFile != null)
                {

                    //Blob code added Nov 3/2025

                    if (!string.IsNullOrEmpty(existingShow.Filename))
                    {
                        try
                        {
                            string oldBlobName = GetBlobNameFromUrl(existingShow.Filename);
                            if (!string.IsNullOrEmpty(oldBlobName))
                            {
                                var oldBlob = _containerClient.GetBlobClient(oldBlobName);
                                await oldBlob.DeleteIfExistsAsync();
                            }
                        }
                        catch (Exception ex)
                        {
                            Console.WriteLine($"Error deleting old blob: {ex.Message}");
                        }
                    }

                    // store the file to upload in fileUpload
                    IFormFile fileUpload = show.FormFile;

                    // create a unique filename for the blob
                    string blobName = Guid.NewGuid().ToString() + Path.GetExtension(fileUpload.FileName);

                    var blobClient = _containerClient.GetBlobClient(blobName);

                    using (var stream = fileUpload.OpenReadStream())
                    {
                        await blobClient.UploadAsync(stream, new BlobHttpHeaders { ContentType = fileUpload.ContentType });
                    }

                    string blobURL = blobClient.Uri.ToString();

                    // assign the blob URL to the record to save in Db
                    show.Filename = blobURL;
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
                    try
                    {
                        string blobName = GetBlobNameFromUrl(show.Filename);
                        if (!string.IsNullOrEmpty(blobName))
                        {
                            var blob = _containerClient.GetBlobClient(blobName);
                            await blob.DeleteIfExistsAsync();
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error deleting blob: {ex.Message}");
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

        //Extacts blob name from container
        private string GetBlobNameFromUrl(string blobUrl)
        {
            if (string.IsNullOrEmpty(blobUrl))
            {
                return string.Empty;
            }

            if (Uri.TryCreate(blobUrl, UriKind.Absolute, out Uri uri))
            {
                string path = uri.PathAndQuery;

                var containerName = "jumbotron-event-uploads";

                int containerIndex = path.IndexOf($"/jumborton-event-uploads/", StringComparison.OrdinalIgnoreCase);

                if (containerIndex != -1)
                {
                    return path.Substring(containerIndex + containerName.Length + 2);
                }
            }
            return string.Empty;
        }
    }
}
