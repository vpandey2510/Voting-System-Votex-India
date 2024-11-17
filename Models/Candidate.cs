using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VotingSystem.Models
{
    public class Candidate
    {
        [Key]
        public int CandidateID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [DataType(DataType.Upload)]
        [StringLength(255)] // Limit the file path length
        public string? CandidateImagePath { get; set; }

        public string Username { get; set; } 

        [ForeignKey("Area")]
        public int AreaID { get; set; }
        public virtual Area? Area { get; set; }

        [ForeignKey("Election")]
        public int ElectionID { get; set; }
        public virtual Election? Election { get; set; }

        [ForeignKey("Party")]
        public int PartyID { get; set; }
        public virtual Party? Party { get; set; }

        public bool Verified { get; set; }

        [StringLength(100)]
        public string Position { get; set; }


    }
}
