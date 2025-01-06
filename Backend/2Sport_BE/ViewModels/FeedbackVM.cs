using _2Sport_BE.Repository.Models;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace _2Sport_BE.ViewModels
{
    public class FeedbackDTO
    {
        [Required]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "FullName must be between 1 and 50 characters.")]
        public string FullName { get; set; }

        [Required]
        [EmailAddress]
        [StringLength(50, MinimumLength = 1, ErrorMessage = "Email must be between 1 and 50 characters.")]
        public string Email { get; set; }

        [Required]
        [StringLength(500, MinimumLength = 1, ErrorMessage = "Content must be between 1 and 500 characters.")]
        public string Content { get; set; }
    }
    public class FeedbackVM : FeedbackDTO
    {
        public int Id { get; set; }
        public int? UserId { get; set; }
        public string? UserName { get; set; }
        public string? PhoneNumber { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class FeedbackCM : FeedbackDTO
    {

    }

    public class FeedbackUM : FeedbackDTO
    {

    }

}
