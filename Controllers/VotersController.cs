using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VotingSystem.Models;

namespace VotingSystem.Controllers
{
    public class VotersController : Controller
    {
        private readonly ApplicationDbContext _context;

        public VotersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Voters
        public async Task<IActionResult> Index()
        {
            return _context.Voters != null ?
                        View(await _context.Voters.Include(m => m.Area).ToListAsync()) :
                        Problem("Entity set 'ApplicationDbContext.Voters'  is null.");
        }
        // GET: Voters/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Voters == null)
            {
                return NotFound();
            }

            var voter = await _context.Voters
                .Include(m => m.Area)
                .FirstOrDefaultAsync(m => m.VoterID == id);
            if (voter == null)
            {
                return NotFound();
            }

            return View(voter);
        }

        // GET: Voters/Create
        [Authorize(Roles = "Voter")]
        public IActionResult Create()
        {
            ViewData["AreaID"] = new SelectList(_context.Areas, "AreaID", "Name");

            var voter = new Voter();

            return View();
        }

        // POST: Voters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VoterID,Name,VoterImagePath,Age,Gender,Username,AreaID")] Voter voter, IFormFile? voterImage)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Handle flag image upload
                    if (voterImage != null && voterImage.Length > 0)
                    {

                        Console.WriteLine("Entering if loop for image verification");
                        var uploadsFolder = Path.Combine("wwwroot/images/voters");
                        var fileName = Path.GetFileName(voterImage.FileName);

                        // Ensure the folder exists
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var filePath = Path.Combine(uploadsFolder, fileName);

                        // Save the file to the server
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await voterImage.CopyToAsync(stream);
                        }

                        // Set the FlagImagePath
                        voter.VoterImagePath = $"/images/voters/{fileName}";
                        Console.WriteLine($"Voter Image Path: {voter.VoterImagePath}");
                    }

                    _context.Add(voter);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { id = voter.VoterID });

                }
                catch (Exception ex)
                {
                    // Log the error (you can use a logging framework here)
                    Console.WriteLine($"Error while creating Voter: {ex.Message}");

                    // Optionally, add a model state error to display on the view
                    ModelState.AddModelError("", "An error occurred while creating the voter.");
                }
            }

            ViewData["AreaID"] = new SelectList(_context.Areas, "AreaID", "Name", voter.AreaID);

            return View(voter);
        }

        // GET: Voters/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Voters == null)
            {
                return NotFound();
            }

            var voter = await _context.Voters.FindAsync(id);
            if (voter == null)
            {
                return NotFound();
            }

            ViewData["AreaID"] = new SelectList(_context.Areas, "AreaID", "Name", voter.AreaID);

            return View(voter);
        }

        // POST: Voters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("VoterID,Name,Age,Gender,Username,AreaID")] Voter voter, IFormFile? voterImage)
        {
            if (id != voter.VoterID)
            {
                return BadRequest("Mismatched Voter ID.");
            }

            if (!ModelState.IsValid)
            {
                // Reload dropdowns in case of an error
                
                ViewData["AreaID"] = new SelectList(_context.Areas, "AreaID", "Name", voter.AreaID);
                return View(voter);
            }

            var existingVoter = await _context.Voters.FindAsync(id);
            if (existingVoter == null)
            {
                return NotFound();
            }

            try
            {
                // Update properties
                existingVoter.Name = voter.Name;
                existingVoter.Age = voter.Age;
                existingVoter.Gender = voter.Gender;
                existingVoter.AreaID = voter.AreaID;

                existingVoter.Eligible = false;

                // Handle candidate image upload
                if (voterImage != null && voterImage.Length > 0)
                {
                    var uploadsFolder = Path.Combine("wwwroot/images/voters");
                    var fileName = Path.GetFileName(voterImage.FileName);

                    // Ensure the folder exists
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var filePath = Path.Combine(uploadsFolder, fileName);

                    // Save the file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await voterImage.CopyToAsync(stream);
                    }

                    // Update the image path
                    existingVoter.VoterImagePath = $"/images/voters/{fileName}";
                }

                else
                {
                    // If no new image is uploaded, retain the old image path if it exists
                    if (existingVoter.VoterImagePath == null)
                    {
                        existingVoter.VoterImagePath = string.Empty;
                    }
                }

                // Save changes
                _context.Update(existingVoter);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", new { id = voter.VoterID });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!VoterExists(voter.VoterID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }

        // GET: Voters/Delete/5
        [Authorize(Roles = "Admin, Voter")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Voters == null)
            {
                return NotFound();
            }

            var voter = await _context.Voters
                .Include(m => m.Area)
                .FirstOrDefaultAsync(m => m.VoterID == id);
            if (voter == null)
            {
                return NotFound();
            }

            return View(voter);
        }

        // POST: Voters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Voters == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Voters'  is null.");
            }
            var voter = await _context.Voters.FindAsync(id);
            if (voter != null)
            {
                _context.Voters.Remove(voter);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "Home");
        }

        private bool VoterExists(int id)
        {
            return (_context.Voters?.Any(e => e.VoterID == id)).GetValueOrDefault();
        }

        // POST: Voter/VerifyEligibility
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> VerifyEligibility(int id, bool isEligible)
        {

            var voter = await _context.Voters.FirstOrDefaultAsync(c => c.VoterID == id);

            if (voter == null)
            {
                return Json(new { success = false, message = "Voter not found" });
            }

            voter.Eligible = isEligible;
            _context.Update(voter);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }
    }
}
