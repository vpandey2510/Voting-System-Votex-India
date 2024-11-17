using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VotingSystem.Models;

namespace VotingSystem.Controllers
{
    public class AreasController : Controller
    {
        private readonly ApplicationDbContext _context;

        public AreasController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Areas
        public async Task<IActionResult> Index()
        {
              return _context.Areas != null ? 
                          View(await _context.Areas.ToListAsync()) :
                          Problem("Entity set 'ApplicationDbContext.Areas'  is null.");
        }

        // GET: Areas/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Areas == null)
            {
                return NotFound();
            }

            var area = await _context.Areas
                .FirstOrDefaultAsync(m => m.AreaID == id);
            if (area == null)
            {
                return NotFound();
            }

            return View(area);
        }

        // GET: Areas/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Areas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("AreaID,Name,AreaImagePath")] Area area, IFormFile? areaImage)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // Handle flag image upload
                    if (areaImage != null && areaImage.Length > 0)
                    {

                        Console.WriteLine("Entering if loop for image verification");
                        var uploadsFolder = Path.Combine("wwwroot/images/areas");
                        var fileName = Path.GetFileName(areaImage.FileName);

                        // Ensure the folder exists
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        var filePath = Path.Combine(uploadsFolder, fileName);

                        // Save the file to the server
                        using (var stream = new FileStream(filePath, FileMode.Create))
                        {
                            await areaImage.CopyToAsync(stream);
                        }

                        // Set the FlagImagePath
                        area.AreaImagePath = $"/images/areas/{fileName}";
                        Console.WriteLine($"Flag Path: {area.AreaImagePath}");
                    }

                    _context.Add(area);
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
            return View(area);
        }

        // GET: Areas/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Areas == null)
            {
                return NotFound();
            }

            var area = await _context.Areas.FindAsync(id);
            if (area == null)
            {
                return NotFound();
            }
            return View(area);
        }

        // POST: Areas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("AreaID,Name")] Area area, IFormFile? areaImage)
        {
            if (id != area.AreaID)
            {
                return BadRequest("Mismatched Area ID.");
            }

            if (!ModelState.IsValid)
            {
                return View(area);
            }

            var existingArea = await _context.Areas.FindAsync(id);
            if (existingArea == null)
            {
                return NotFound();
            }

            try
            {
                // Update basic properties
                existingArea.Name = area.Name;

                // Handle flag image upload
                if (areaImage != null && areaImage.Length > 0)
                {
                    var uploadsFolder = Path.Combine("wwwroot/images/areas");
                    var fileName = Path.GetFileName(areaImage.FileName);

                    // Ensure the folder exists
                    if (!Directory.Exists(uploadsFolder))
                    {
                        Directory.CreateDirectory(uploadsFolder);
                    }

                    var filePath = Path.Combine(uploadsFolder, fileName);

                    // Save the file
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await areaImage.CopyToAsync(stream);
                    }

                    // Update FlagImagePath
                    existingArea.AreaImagePath = $"/images/areas/{fileName}";
                }

                // Save changes
                _context.Update(existingArea);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!AreaExists(area.AreaID))
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

        // GET: Areas/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Areas == null)
            {
                return NotFound();
            }

            var area = await _context.Areas
                .FirstOrDefaultAsync(m => m.AreaID == id);
            if (area == null)
            {
                return NotFound();
            }

            return View(area);
        }

        // POST: Areas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Areas == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Areas'  is null.");
            }
            var area = await _context.Areas.FindAsync(id);
            if (area != null)
            {
                _context.Areas.Remove(area);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool AreaExists(int id)
        {
          return (_context.Areas?.Any(e => e.AreaID == id)).GetValueOrDefault();
        }
    }
}
