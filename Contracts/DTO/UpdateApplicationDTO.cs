using System.ComponentModel.DataAnnotations;

namespace Application_Tracker.Contracts.DTO
{
    public class UpdateApplicationDTO
    {
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
    }
}
