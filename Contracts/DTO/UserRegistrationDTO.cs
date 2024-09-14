using System.ComponentModel.DataAnnotations;

namespace Application_Tracker.Contracts.DTO
{
    public class UserRegistrationDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [MinLength(6, ErrorMessage = "The password must be at least 6 characters")]
        public string Password { get; set; }
    }
}
