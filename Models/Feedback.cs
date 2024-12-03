using System.ComponentModel.DataAnnotations;

namespace VotingSystem.Models
{
    public class Feedback
    {
        [Key]
        public int FeedbackID { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [StringLength(100)]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Text)]
        [StringLength(255)]
        public string Message { get; set; }
    }
}
