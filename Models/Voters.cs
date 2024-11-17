using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace VotingSystem.Models
{
    public class Voter
    {
        [Key]
        public int VoterID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [DataType(DataType.Upload)]
        [StringLength(255)] // Limit the file path length
        public string? VoterImagePath { get; set; }

        [Required]
        public int Age { get; set; }

        [Required]
        [StringLength(10)]
        public string Gender { get; set; }

        [Required]
        [StringLength(100)]
        public string Username { get; set; }

        [ForeignKey("Area")]
        public int AreaID { get; set; }
        public virtual Area? Area { get; set; }

        public bool Eligible { get; set; }
    }
}
