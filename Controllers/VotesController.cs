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
            var userEmail = User.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value;
            var voter = await _context.Voters.FirstOrDefaultAsync(v => v.Username == userEmail);

            if (voter == null || !voter.Eligible)
            {
                _logger.LogWarning("Non-eligible voter attempted to vote: {UserEmail}", userEmail);
                return Unauthorized("You are not eligible to vote.");
            }

            // Fetch candidates only from the voter's area
            var candidates = await _context.Candidates
                .Where(c => c.ElectionID == electionId && c.AreaID == voter.AreaID)
                .ToListAsync();

            // Pass the ElectionID and list of candidates to the view
            ViewData["ElectionID"] = electionId;
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
                return BadRequest("You have already voted in this election.");
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
                return RedirectToAction("CastVoteSuccess");
            }
            else
            {
                _logger.LogError("Error casting vote for Voter {VoterHash} in election {ElectionId}.", voterHash, electionId);
                return BadRequest("There was an issue casting your vote. Please try again.");
            }
        }

        // Get Total Votes per Candidate and Area for an Election
        public async Task<IActionResult> GetTotalVotes()
        {
            // Retrieve elections with their respective candidates and vote counts
            var elections = await _context.Elections
                .Select(election => new
                {
                    ElectionID = election.ElectionID,
                    ElectionName = election.Name,
                    Status = election.Status,
                    CandidatesByArea = _context.Candidates
                        .Where(candidate => candidate.ElectionID == election.ElectionID)
                        .GroupBy(candidate => new { candidate.AreaID, candidate.Area.Name })
                        .Select(areaGroup => new
                        {
                            AreaID = areaGroup.Key.AreaID,
                            AreaName = areaGroup.Key.Name,
                            Candidates = areaGroup
                                .Select(candidate => new
                                {
                                    CandidateID = candidate.CandidateID,
                                    CandidateName = candidate.Name,
                                    TotalVotes = _context.Votes.Count(vote => vote.CandidateID == candidate.CandidateID)
                                })
                                .OrderByDescending(c => c.TotalVotes)
                                .ToList()
                        })
                        .ToList()
                })
                .ToListAsync();

            var electionResults = elections.Select(e => new
            {
                ElectionID = e.ElectionID,
                ElectionName = e.ElectionName,
                Status = e.Status,
                CandidatesByArea = e.CandidatesByArea
            }).ToList();

            return View("Results", electionResults);

        }
    }
}
