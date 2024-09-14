using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Application_Tracker.Models
{
    public class Application
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string Company { get; set; }

        [Required]
        [MaxLength(100)]
        public string Position { get; set; }

        [Required]
        [MaxLength(100)]
        public string Location { get; set; }

        [Required]
        [Url]
        public string Link { get; set; }

        [Required]
        public DateTime Date { get; set; }

        [Required]
        public string Status { get; set; }

        [Required]
        public Guid UserId { get; set; }

        [ForeignKey("UserId")]
        public User User { get; set; }
    }
}
