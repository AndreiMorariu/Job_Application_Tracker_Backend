using System.ComponentModel.DataAnnotations;

namespace Application_Tracker.Contracts.DTO
{
    public class UserLoginDTO
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }
    }
}
