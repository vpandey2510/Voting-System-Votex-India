using System.ComponentModel.DataAnnotations;

namespace VotingSystem.Models
{
    public class Area
    {
        [Key]
        public int AreaID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [DataType(DataType.Upload)]
        [StringLength(255)] // Limit the file path length
        public string? AreaImagePath { get; set; }

        public ICollection<Candidate>? Candidates { get; set; }
    }
}
