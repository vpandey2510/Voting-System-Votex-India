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

        // GET: Election
        // Admin view to manage elections (create, edit, and delete)
        
        public async Task<IActionResult> Index()
        {
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
        public async Task<IActionResult> Create([Bind("Name,StartDate,EndDate,Status,Description")] Election election)
        {
            if (ModelState.IsValid)
            {
                _context.Add(election);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
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
        public async Task<IActionResult> Edit(int id, [Bind("ElectionID,Name,StartDate,EndDate,Status,Description")] Election election)
        {
            if (id != election.ElectionID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(election);
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
            return View(election);
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
