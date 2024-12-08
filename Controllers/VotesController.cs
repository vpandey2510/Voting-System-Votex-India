using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using VotingSystem.Models;
using static System.Collections.Specialized.BitVector32;

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

            if (voter == null)
            {
                return Challenge();
            }
            else if (!voter.Eligible)
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

        public async Task<IActionResult> GetTotalVotes(int? electionId = 1, int? areaId = null)
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

            var electionsStatus = await _context.Elections.ToListAsync();
            var selectedElection = elections.FirstOrDefault(e => e.ElectionID == electionId);

            ViewData["Elections"] = elections;
            ViewData["Areas"] = areas;
            ViewData["SelectedElection"] = electionId;
            ViewData["SelectedElectionStatus"] = selectedElection?.Status;
            ViewData["SelectedArea"] = areaId;

            return View("Results", candidates);
        }

        public async Task<IActionResult> ElectionResult(int? electionId = null, int? areaId = null)
        {
            var electionResultsQuery = _context.Votes
                .Join(
                    _context.Candidates,
                    vote => vote.CandidateID,
                    candidate => candidate.CandidateID,
                    (vote, candidate) => new { vote, candidate }
                )
                .Join(
                    _context.Parties,
                    vc => vc.candidate.PartyID,
                    party => party.PartyID,
                    (vc, party) => new { vc.vote, vc.candidate, party }
                )
                .Join(
                    _context.Areas,
                    vcp => vcp.candidate.AreaID,
                    area => area.AreaID,
                    (vcp, area) => new
                    {
                        AreaName = area.Name, // Area name
                        PartyName = vcp.party.Name, // Party name
                        CandidateName = vcp.candidate.Name, // Candidate name
                        VoteCount = vcp.vote.CandidateID, // This will be grouped later
                        ElectionID = vcp.vote.ElectionID, // Add ElectionID here to filter by election
                        AreaID = vcp.candidate.AreaID,
                        PartyImage = vcp.party.FlagImagePath
                    }
                );

            // Apply filters if electionId or areaId is provided
            if (electionId.HasValue)
            {
                electionResultsQuery = electionResultsQuery.Where(v => v.ElectionID == electionId);
            }

            if (areaId.HasValue)
            {
                electionResultsQuery = electionResultsQuery.Where(v => v.AreaID == areaId);
            }

            // Group to calculate vote counts for each party in an area
            var groupedResults = await electionResultsQuery
                .GroupBy(r => new { r.AreaName, r.AreaID, r.PartyName, r.PartyImage, r.ElectionID })
                .Select(g => new
                {
                    g.Key.AreaID,
                    g.Key.AreaName,
                    g.Key.PartyName,
                    g.Key.PartyImage,
                    g.Key.ElectionID,
                    TotalVotes = g.Count(), // Count votes for each party in the area
                    Candidates = g.Select(c => c.CandidateName).Distinct().ToList() // List of candidates in the party
                })
                .ToListAsync();

            // Get the top party in each area based on vote count
            var electionResults = groupedResults
                .GroupBy(gr => new { gr.AreaID, gr.AreaName, gr.ElectionID })
                .Select(g => g.OrderByDescending(gr => gr.TotalVotes).FirstOrDefault()) // Select top party by votes
                .ToList();

            // Retrieve all areas and elections for dropdowns or filters
            var areas = await _context.Areas.ToListAsync();
            var elections = await _context.Elections.ToListAsync();
            var selectedElection = elections.FirstOrDefault(e => e.ElectionID == electionId);

            // Pass data to the view
            ViewData["Elections"] = elections;
            ViewData["Areas"] = areas;
            ViewData["SelectedElection"] = electionId;
            ViewData["SelectedElectionStatus"] = selectedElection?.Status;
            ViewData["SelectedArea"] = areaId;

            // Return the grouped results to a view named "ElectionResults"
            return View("ElectionResult", electionResults);
        }
    }
}
