using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using VotingSystem.Models;

namespace VotingSystem.Controllers
{
    public class FeedbacksController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FeedbacksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // POST: Submit Feedback
        [Route("Feedback/SubmitFeedback")]
        [HttpPost]
        public async Task<IActionResult> SubmitFeedback(Feedback feedback)
        {
            if (ModelState.IsValid)
            {
                _context.Feedbacks.Add(feedback);
                await _context.SaveChangesAsync();
                TempData["FeedbackMessage"] = "Thank you for your feedback!";
                return RedirectToAction("Index", "Home");
            }
            return View("Error");
        }

        // GET: List Feedback for Home Page
        [Authorize(Roles = "Admin")]
        public IActionResult GetFeedbacks()
        {
            var feedbacks = _context.Feedbacks
                .OrderByDescending(f => f.FeedbackID)
                .Take(5) // Limit to recent 5 feedbacks
                .ToList();
            return View("_FeedbackList", feedbacks);
        }
    }
}
