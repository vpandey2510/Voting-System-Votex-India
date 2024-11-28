using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using VotingSystem.Models;

namespace VotingSystem.Controllers
{

    public class ElectionsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ElectionsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Helper method to close elections with passed end dates

        private async Task AutoOpenElectionsAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            // Find elections where the start date has passed and the status is not "Open"
            var electionsToOpen = await _context.Elections
                .Where(e => e.StartDate <= today && e.Status != "Open")
                .ToListAsync();

            foreach (var election in electionsToOpen)
            {
                election.Status = "Open";
            }

            // Save changes if there are updates
            if (electionsToOpen.Count > 0)
            {
                _context.UpdateRange(electionsToOpen);
                await _context.SaveChangesAsync();
            }
        }

        private async Task AutoCloseElectionsAsync()
        {
            var today = DateOnly.FromDateTime(DateTime.Now);

            // Find elections where the end date has passed and the status is not "Closed"
            var electionsToClose = await _context.Elections
                .Where(e => e.EndDate < today && e.Status != "Closed")
                .ToListAsync();

            foreach (var election in electionsToClose)
            {
                election.Status = "Closed";
            }

            // Save changes if there are updates
            if (electionsToClose.Count > 0)
            {
                _context.UpdateRange(electionsToClose);
                await _context.SaveChangesAsync();
            }
        }

        // GET: Election
        // Admin view to manage elections (create, edit, and delete)

        public async Task<IActionResult> Index()
        {
            await AutoOpenElectionsAsync();
            await AutoCloseElectionsAsync();

            return View(await _context.Elections.ToListAsync());
        }

        // GET: Election/Details/5
        // Candidate and Admin can view details of an election
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var election = await _context.Elections
                .FirstOrDefaultAsync(m => m.ElectionID == id);
            if (election == null)
            {
                return NotFound();
            }

            return View(election);
        }

        // GET: Election/Create
        // Only Admin can create elections
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            var election = new Election
            {
                Status = "Open" // Set default status to "open"
            };
            return View();
        }

        // POST: Election/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Name,ElectionImagePath,StartDate,EndDate,Status,Description")] Election election, IFormFile? electionImage)
        {
            // Validate dates
            if (election.EndDate < election.StartDate)
            {
                // Add a model state error for end date before start date
                ModelState.AddModelError("EndDate", "End date cannot be earlier than the start date.");
                return View(election); // Return the view with the error message
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Handle flag image upload
                    if (electionImage != null && electionImage.Length > 0)
                    {

                        Console.WriteLine("Entering if loop for image verification");
                        var uploadsFolder = Path.Combine("wwwroot/images/elections");
                        var fileName = Path.GetFileName(electionImage.FileName);

                        // Ensure the folder exists
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var filePath = Path.Combine(uploadsFolder, fileName);

                        // Save the file to the server
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await electionImage.CopyToAsync(stream);
                        }

                        // Set the FlagImagePath
                        election.ElectionImagePath = $"/images/elections/{fileName}";
                        Console.WriteLine($"Election Path: {election.ElectionImagePath}");
                    }

                    _context.Add(election);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));

                }
                catch (Exception ex)
                {
                    // Log the error (you can use a logging framework here)
                    Console.WriteLine($"Error while creating party: {ex.Message}");

                    // Optionally, add a model state error to display on the view
                    ModelState.AddModelError("", "An error occurred while creating the party.");
                }
            }
            return View(election);
        }

        // GET: Election/Edit/5
        // Only Admin can edit elections
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var election = await _context.Elections.FindAsync(id);
            if (election == null)
            {
                return NotFound();
            }
            return View(election);
        }

        // POST: Election/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("ElectionID,Name,StartDate,EndDate,Status,Description")] Election election, IFormFile? electionImage)
        {
            if (id != election.ElectionID)
            {
                return BadRequest("Mismatched Election ID.");
            }

            // Validate dates
            if (election.EndDate < election.StartDate)
            {
                ModelState.AddModelError("EndDate", "End date cannot be earlier than the start date.");
                return View(election); // Return the view with the error message
            }

            if (!ModelState.IsValid)
            {
                return View(election);
            }

            var existingElection = await _context.Elections.FindAsync(id);
            if (existingElection == null)
            {
                return NotFound();
            }

            try
            {
                // Update basic properties
                existingElection.Name = election.Name;
                existingElection.Status = election.Status;
                existingElection.StartDate = election.StartDate;
                existingElection.EndDate = election.EndDate;
                existingElection.Description = election.Description;

                // Handle flag image upload
                if (electionImage != null && electionImage.Length > 0)
                {
                    var uploadsFolder = Path.Combine("wwwroot/images/elections");
                    var fileName = Path.GetFileName(electionImage.FileName);

                    // Ensure the folder exists
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var filePath = Path.Combine(uploadsFolder, fileName);

                    // Save the file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await electionImage.CopyToAsync(stream);
                    }

                    // Update FlagImagePath
                    existingElection.ElectionImagePath = $"/images/elections/{fileName}";
                }

                else
                {
                    // If no new image is uploaded, retain the old image path if it exists
                    if (existingElection.ElectionImagePath == null)
                    {
                        existingElection.ElectionImagePath = string.Empty;
                    }
                }

                // Save changes
                _context.Update(existingElection);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!ElectionExists(election.ElectionID))
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

        // GET: Election/Delete/5
        // Only Admin can delete elections
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var election = await _context.Elections
                .FirstOrDefaultAsync(m => m.ElectionID == id);
            if (election == null)
            {
                return NotFound();
            }

            return View(election);
        }

        // POST: Election/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var election = await _context.Elections.FindAsync(id);
            _context.Elections.Remove(election);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Election/ChangeStatus/5
        // Admin can change the status of the election
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangeStatus(int id, string status)
        {
            var election = await _context.Elections.FindAsync(id);
            if (election == null)
            {
                return NotFound();
            }

            // Check if the status is valid ("open" or "closed or upcoming")
            if (status != "Open" && status != "Closed" && status != "Upcoming")
            {
                ModelState.AddModelError(string.Empty, "Invalid status.");
                return View(nameof(Index));
            }

            election.Status = status;
            _context.Update(election);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }


        // Helper method to check if an election exists
        private bool ElectionExists(int id)
        {
            return _context.Elections.Any(e => e.ElectionID == id);
        }
    }
}
