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
            return View();
        }

        // POST: Voters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VoterID,Name,Age,Gender,Username,AreaID")] Voter voter)
        {
            if (ModelState.IsValid)
            {
                _context.Add(voter);
                await _context.SaveChangesAsync();
                return RedirectToAction("Details", new { id = voter.VoterID });
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
        public async Task<IActionResult> Edit(int id, [Bind("VoterID,Name,Age,Gender,Username,AreaID")] Voter voter)
        {
            if (id != voter.VoterID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(voter);
                    await _context.SaveChangesAsync();
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
                return RedirectToAction("Details", new { id = voter.VoterID });
            }

            ViewData["AreaID"] = new SelectList(_context.Areas, "AreaID", "Name", voter.AreaID);

            return View(voter);
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
            return RedirectToAction(nameof(Index));
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
