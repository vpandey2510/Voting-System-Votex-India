﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System;
using VotingSystem.Models;


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
            ViewData["ElectionID"] = new SelectList(_context.Elections, "ElectionID", "Name");
            ViewData["PartyID"] = new SelectList(_context.Parties, "PartyID", "Name");
            ViewData["AreaID"] = new SelectList(_context.Areas, "AreaID", "Name");

            return View();
        }

        // POST: Candidates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CandidateID,Name,Username,AreaID,ElectionID,PartyID,Position")] Candidate candidate)
        {
           if (ModelState.IsValid)
            {
                _context.Add(candidate);
                await _context.SaveChangesAsync();

                return RedirectToAction("Details", new { id = candidate.CandidateID });
            }

            ViewData["ElectionID"] = new SelectList(_context.Elections, "ElectionID", "Name", candidate.ElectionID);
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
            ViewData["ElectionID"] = new SelectList(_context.Elections, "ElectionID", "Name", candidate.ElectionID);
            ViewData["PartyID"] = new SelectList(_context.Parties, "PartyID", "Name", candidate.PartyID);
            ViewData["AreaID"] = new SelectList(_context.Areas, "AreaID", "Name", candidate.AreaID);

            return View(candidate);
        }

        // POST: Candidates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CandidateID,Name,Username,AreaID,ElectionID,PartyID,Position")] Candidate candidate)
        {
            if (id != candidate.CandidateID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(candidate);
                    await _context.SaveChangesAsync();
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
                return RedirectToAction("Details", new { id = candidate.CandidateID });
            }
            ViewData["ElectionID"] = new SelectList(_context.Elections, "ElectionID", "Name", candidate.ElectionID);
            ViewData["PartyID"] = new SelectList(_context.Parties, "PartyID", "Name", candidate.PartyID);
            ViewData["AreaID"] = new SelectList(_context.Areas, "AreaID", "Name", candidate.AreaID);

            return View(candidate);
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