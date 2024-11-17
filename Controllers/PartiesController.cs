using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VotingSystem.Models;


namespace VotingSystem.Controllers
{
    [Authorize(Roles = "Admin")]
    public class PartiesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PartiesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Parties
        public async Task<IActionResult> Index()
        {
            return _context.Parties != null ?
                        View(await _context.Parties.ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.Parties'  is null.");
        }

        // GET: Parties/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Parties == null)
            {
                return NotFound();
            }

            var party = await _context.Parties
                .FirstOrDefaultAsync(m => m.PartyID == id);
            if (party == null)
            {
                return NotFound();
            }

            return View(party);
        }

        // GET: Parties/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Parties/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PartyID,Name,FlagImagePath")] Party party, IFormFile? flagImage)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Handle flag image upload
                    if (flagImage != null && flagImage.Length > 0)
                    {

                        Console.WriteLine("Entering if loop for image verification");
                        var uploadsFolder = Path.Combine("wwwroot/images/flags");
                        var fileName = Path.GetFileName(flagImage.FileName);

                        // Ensure the folder exists
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var filePath = Path.Combine(uploadsFolder, fileName);

                        // Save the file to the server
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await flagImage.CopyToAsync(stream);
                        }

                        // Set the FlagImagePath
                        party.FlagImagePath = $"/images/flags/{fileName}";
                        Console.WriteLine($"Flag Path: {party.FlagImagePath}");
                    }

                    // Add the new party to the context
                    _context.Add(party);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));  // Redirect to the index page after creating
                }
                catch (Exception ex)
                {
                    // Log the error (you can use a logging framework here)
                    Console.WriteLine($"Error while creating party: {ex.Message}");

                    // Optionally, add a model state error to display on the view
                    ModelState.AddModelError("", "An error occurred while creating the party.");
                }
            }

            return View(party); // Return to the view if the model state is invalid or if there was an error
        }


        // GET: Parties/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Parties == null)
            {
                return NotFound();
            }

            var party = await _context.Parties.FindAsync(id);
            if (party == null)
            {
                return NotFound();
            }
            return View(party);
        }

        // POST: Parties/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        // POST: Parties/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PartyID,Name")] Party party, IFormFile? flagImage)
        {
            if (id != party.PartyID)
            {
                return BadRequest("Mismatched Party ID.");
            }

            if (!ModelState.IsValid)
            {
                return View(party);
            }

            var existingParty = await _context.Parties.FindAsync(id);
            if (existingParty == null)
            {
                return NotFound();
            }

            try
            {
                // Update basic properties
                existingParty.Name = party.Name;

                // Handle flag image upload
                if (flagImage != null && flagImage.Length > 0)
                {
                    var uploadsFolder = Path.Combine("wwwroot/images/flags");
                    var fileName = Path.GetFileName(flagImage.FileName);

                    // Ensure the folder exists
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var filePath = Path.Combine(uploadsFolder, fileName);

                    // Save the file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await flagImage.CopyToAsync(stream);
                    }

                    // Update FlagImagePath
                    existingParty.FlagImagePath = $"/images/flags/{fileName}";
                }

                // Save changes
                _context.Update(existingParty);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PartyExists(party.PartyID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToAction(nameof(Index));
        }



        // GET: Parties/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Parties == null)
            {
                return NotFound();
            }

            var party = await _context.Parties
                .FirstOrDefaultAsync(m => m.PartyID == id);
            if (party == null)
            {
                return NotFound();
            }

            return View(party);
        }

        // POST: Parties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Parties == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Parties'  is null.");
            }
            var party = await _context.Parties.FindAsync(id);
            if (party != null)
            {
                _context.Parties.Remove(party);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PartyExists(int id)
        {
            return (_context.Parties?.Any(e => e.PartyID == id)).GetValueOrDefault();
        }
    }
}
