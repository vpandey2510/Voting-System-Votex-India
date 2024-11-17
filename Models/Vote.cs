using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VotingSystem.Models
{
    public class Vote
    {
        [Key]
        public int VoteId { get; set; }

        public int VoterHashID { get; set; }


        [ForeignKey("Candidate")]
        public int CandidateID { get; set; }
        public virtual Candidate Candidate { get; set; }

        [ForeignKey("Election")]
        public int ElectionID { get; set; }
        public virtual Election Election { get; set; }

    }
}
