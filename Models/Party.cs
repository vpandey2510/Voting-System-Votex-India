using System.ComponentModel.DataAnnotations;

namespace VotingSystem.Models
{
    public class Party
    {
        [Key]
        public int PartyID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [DataType(DataType.Upload)]
        [StringLength(255)] // Limit the file path length
        public string? FlagImagePath { get; set; }

        public ICollection<Candidate>? Candidates { get; set; }
    }
}
