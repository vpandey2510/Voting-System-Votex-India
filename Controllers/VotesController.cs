using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VotingSystem.Models;

namespace VotingSystem.Controllers
{
    public class VotesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<VotesController> _logger;

        public VotesController(ApplicationDbContext context, ILogger<VotesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        // Display form to cast vote for candidates in voter's area
        public async Task<IActionResult> CastVoteForm(int? electionId)
        {
            if (electionId == null)
            {
                return NotFound("Election ID is required.");
            }

            var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var voter = await _context.Voters.FirstOrDefaultAsync(v => v.Username == userEmail);

            if (voter == null || !voter.Eligible)
            {
                return Unauthorized("You are not eligible to vote.");
            }

            var candidates = await _context.Candidates
                .Include(c => c.Party) // Include related Party data
                .Where(c => c.ElectionID == electionId && c.AreaID == voter.AreaID)
                .ToListAsync();

            if (candidates == null || !candidates.Any())
            {
                _logger.LogWarning("No candidates found for ElectionID: {ElectionID}, AreaID: {AreaID}", electionId, voter.AreaID);
            }
            else
            {
                foreach (var candidate in candidates)
                {
                    _logger.LogInformation("Candidate: {Name}, Position: {Position}, Party: {Party}, Image: {Image}",
                        candidate.Name, candidate.Position, candidate.Party?.Name, candidate.CandidateImagePath);
                }
            }

            // Pass the ElectionID and list of candidates to the view
            ViewData["ElectionId"] = electionId;
            return View(candidates); // Pass the candidates directly to the view
        }


        public static int GenerateUniqueVoterHash(Voter voter)
        {
            string uniqueString = $"{voter.Username}";
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] hashBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(uniqueString));
                return Math.Abs(BitConverter.ToInt32(hashBytes, 0));
            }
        }

        public IActionResult CastVoteSuccess()
        {
            return View();
        }

        // Cast Vote Action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CastVote(int candidateId, int electionId)
        {
            var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var voter = await _context.Voters.FirstOrDefaultAsync(v => v.Username == userEmail);

            if (voter == null || !voter.Eligible)
            {
                _logger.LogWarning("Non-eligible voter attempted to vote: {UserEmail}", userEmail);
                return Unauthorized("You are not eligible to vote.");
            }

            int voterHash = GenerateUniqueVoterHash(voter);

            bool hasVoted = await _context.Votes.AnyAsync(v => v.VoterHashID == voterHash && v.ElectionID == electionId);
            if (hasVoted)
            {
                ViewBag.Message = "You have already voted in this election.";
                ViewBag.ElectionId = electionId;
                return View("CastVoteSuccess");
            }

            var vote = new Vote
            {
                CandidateID = candidateId,
                ElectionID = electionId,
                VoterHashID = voterHash
            };

            _context.Votes.Add(vote);
            var result = await _context.SaveChangesAsync();

            if (result > 0)
            {
                _logger.LogInformation("Voter {VoterHash} successfully cast a vote for candidate {CandidateId} in election {ElectionId}.", voterHash, candidateId, electionId);
                ViewBag.Message = "Your vote has been successfully cast!";
                return RedirectToAction("CastVoteSuccess");
            }
            else
            {
                _logger.LogError("Error casting vote for Voter {VoterHash} in election {ElectionId}.", voterHash, electionId);
                return BadRequest("There was an issue casting your vote. Please try again.");
            }
        }

        

        public async Task<IActionResult> GetTotalVotes(int? electionId = 1, int? areaId = 1)
        {

            var elections = await _context.Elections.ToListAsync();
            var areas = await _context.Areas.ToListAsync(); // Assuming areas are stored in Areas table

            var candidatesQuery = _context.Candidates
                .Include(c => c.Area)
                .Include(c => c.Party)
                .Include(c => c.Election)
                .Select(candidate => new
                {
                    CandidateID = candidate.CandidateID,
                    CandidateName = candidate.Name,
                    AreaID = candidate.AreaID,
                    AreaName = candidate.Area.Name,
                    PartyID = candidate.PartyID,
                    PartyName = candidate.Party.Name,
                    ElectionID = candidate.ElectionID,
                    ElectionName = candidate.Election.Name,
                    TotalVotes = _context.Votes.Count(v => v.CandidateID == candidate.CandidateID)
                });

            // Apply filters if provided
            if (electionId.HasValue)
                candidatesQuery = candidatesQuery.Where(c => c.ElectionID == electionId.Value);

            if (areaId.HasValue)
                candidatesQuery = candidatesQuery.Where(c => c.AreaID == areaId.Value);

            var candidates = await candidatesQuery
                .OrderByDescending(c => c.TotalVotes) // Sort by descending votes
                .ToListAsync();

            ViewData["Elections"] = elections;
            ViewData["Areas"] = areas;
            ViewData["SelectedElection"] = electionId;
            ViewData["SelectedArea"] = areaId;

            return View("Results", candidates);
        }


    }
}
