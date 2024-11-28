using System.ComponentModel.DataAnnotations;

namespace VotingSystem.Models
{
    public class Election
    {
        [Key]
        public int ElectionID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [DataType(DataType.Upload)]
        [StringLength(255)] // Limit the file path length
        public string? ElectionImagePath { get; set; }

        [Required]
        public DateOnly StartDate { get; set; }

        [Required]
        public DateOnly EndDate { get; set; }

        [Required]
        [StringLength(10)]
        public string Status { get; set; } = "Open";// Possible values: "open" or "closed"

        [StringLength(500)]
        public string Description { get; set; }
    }
}
