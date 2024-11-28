using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using VotingSystem.Models;
using static System.Collections.Specialized.BitVector32;


namespace VotingSystem.Controllers
{
    public class CandidatesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CandidatesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Candidates
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Candidates.Include(c => c.Election).Include(c => c.Party).Include(c => c.Area);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Candidates/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Candidates == null)
            {
                return NotFound();
            }

            var candidate = await _context.Candidates
                .Include(c => c.Election)
                .Include(c => c.Party)
                .Include(c => c.Area)
                .FirstOrDefaultAsync(m => m.CandidateID == id);
            if (candidate == null)
            {
                return NotFound();
            }

            return View(candidate);
        }

        [Authorize(Roles = "Candidate")]
        // GET: Candidates/Create
        public IActionResult Create()
        {
            var openElections = _context.Elections
                .Where(e => e.Status == "Open") // Filter elections with Status == "Open"
                .ToList();

            ViewData["ElectionID"] = new SelectList(openElections, "ElectionID", "Name");
            ViewData["PartyID"] = new SelectList(_context.Parties.ToList(), "PartyID", "Name");
            ViewData["AreaID"] = new SelectList(_context.Areas.ToList(), "AreaID", "Name");

            var candidate = new Candidate();

            return View(candidate);
        }

        // POST: Candidates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CandidateID,Name,CandidateImagePath,Username,AreaID,ElectionID,PartyID,Position")] Candidate candidate, IFormFile? candidateImage)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Handle flag image upload
                    if (candidateImage != null && candidateImage.Length > 0)
                    {

                        Console.WriteLine("Entering if loop for image verification");
                        var uploadsFolder = Path.Combine("wwwroot/images/candidates");
                        var fileName = Path.GetFileName(candidateImage.FileName);

                        // Ensure the folder exists
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var filePath = Path.Combine(uploadsFolder, fileName);

                        // Save the file to the server
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await candidateImage.CopyToAsync(stream);
                        }

                        // Set the FlagImagePath
                        candidate.CandidateImagePath = $"/images/candidates/{fileName}";
                        Console.WriteLine($"Candidate Image Path: {candidate.CandidateImagePath}");
                    }

                    _context.Add(candidate);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Details", new { id = candidate.CandidateID });

                }
                catch (Exception ex)
                {
                    // Log the error (you can use a logging framework here)
                    Console.WriteLine($"Error while creating Candidate: {ex.Message}");

                    // Optionally, add a model state error to display on the view
                    ModelState.AddModelError("", "An error occurred while creating the canndidate.");
                }
            }

            var openElections = _context.Elections
                .Where(e => e.Status == "Open") // Filter elections with Status == "Open"
                .ToList();

            ViewData["ElectionID"] = new SelectList(openElections, "ElectionID", "Name", candidate.ElectionID);
            ViewData["PartyID"] = new SelectList(_context.Parties, "PartyID", "Name", candidate.PartyID);
            ViewData["AreaID"] = new SelectList(_context.Areas, "AreaID", "Name", candidate.AreaID);
            return View(candidate);
        }



        // GET: Candidates/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Candidates == null)
            {
                return NotFound();
            }

            var candidate = await _context.Candidates.FindAsync(id);
            if (candidate == null)
            {
                return NotFound();
            }

            var openElections = _context.Elections
                .Where(e => e.Status == "Open") // Filter elections with Status == "Open"
                .ToList();

            ViewData["ElectionID"] = new SelectList(openElections, "ElectionID", "Name", candidate.ElectionID);
            ViewData["PartyID"] = new SelectList(_context.Parties, "PartyID", "Name", candidate.PartyID);
            ViewData["AreaID"] = new SelectList(_context.Areas, "AreaID", "Name", candidate.AreaID);

            return View(candidate);
        }

        // POST: Candidates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CandidateID,Name,Username,AreaID,ElectionID,PartyID,Position,Verified")] Candidate candidate, IFormFile? candidateImage)
        {
            if (id != candidate.CandidateID)
            {
                return BadRequest("Mismatched Candidate ID.");
            }

            if (!ModelState.IsValid)
            {
                var openElections = _context.Elections
                 .Where(e => e.Status == "Open") // Filter elections with Status == "Open"
                 .ToList();

                ViewData["ElectionID"] = new SelectList(openElections, "ElectionID", "Name", candidate.ElectionID);
                ViewData["PartyID"] = new SelectList(_context.Parties, "PartyID", "Name", candidate.PartyID);
                ViewData["AreaID"] = new SelectList(_context.Areas, "AreaID", "Name", candidate.AreaID);
                return View(candidate);
            }

            var existingCandidate = await _context.Candidates.FindAsync(id);
            if (existingCandidate == null)
            {
                return NotFound();
            }

            try
            {
                // Update properties
                existingCandidate.Name = candidate.Name;
                existingCandidate.AreaID = candidate.AreaID;
                existingCandidate.ElectionID = candidate.ElectionID;
                existingCandidate.PartyID = candidate.PartyID;
                existingCandidate.Position = candidate.Position;

                existingCandidate.Verified = false;

                // Handle candidate image upload
                if (candidateImage != null && candidateImage.Length > 0)
                {
                    var uploadsFolder = Path.Combine("wwwroot/images/candidates");
                    var fileName = Path.GetFileName(candidateImage.FileName);

                    // Ensure the folder exists
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var filePath = Path.Combine(uploadsFolder, fileName);

                    // Save the file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await candidateImage.CopyToAsync(stream);
                    }

                    // Update the image path
                    existingCandidate.CandidateImagePath = $"/images/candidates/{fileName}";
                }

                else
                {
                    // If no new image is uploaded, retain the old image path if it exists
                    if (existingCandidate.CandidateImagePath == null)
                    {
                        existingCandidate.CandidateImagePath = string.Empty;
                    }
                }

                // Save changes
                _context.Update(existingCandidate);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", new { id = candidate.CandidateID });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!CandidateExists(candidate.CandidateID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
        }


        [Authorize(Roles = "Admin, Candidate")]
        // GET: Candidates/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Candidates == null)
            {
                return NotFound();
            }

            var candidate = await _context.Candidates
                .Include(c => c.Election)
                .Include(c => c.Party)
                .Include(c => c.Area)
                .FirstOrDefaultAsync(m => m.CandidateID == id);
            if (candidate == null)
            {
                return NotFound();
            }

            return View(candidate);
        }

        // POST: Candidates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Candidates == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Candidates'  is null.");
            }
            var candidate = await _context.Candidates.FindAsync(id);
            if (candidate != null)
            {
                _context.Candidates.Remove(candidate);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CandidateExists(int id)
        {
            return (_context.Candidates?.Any(e => e.CandidateID == id)).GetValueOrDefault();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Verify(int id, bool isVerified)
        {
            var candidate = await _context.Candidates.FirstOrDefaultAsync(c => c.CandidateID == id);

            if (candidate == null)
            {
                return Json(new { success = false, message = "Candidate not found" });
            }

            candidate.Verified = isVerified;
            _context.Update(candidate);
            await _context.SaveChangesAsync();

            return Json(new { success = true });
        }

        public async Task<bool> CandidateExistsByEmailAsync(string uname)
        {
            // Check if the candidate exists by matching the email of the user
            var candidateExists = await _context.Candidates
                                                .AnyAsync(c => c.Username == uname); // Assuming Candidate model has an Email field

            return candidateExists;
        }


    }
}
