using Microsoft.AspNetCore.Identity;

namespace Application_Tracker.Models
{
    public class User : IdentityUser<Guid>
    {
        public List<Application> Applications { get; set; } = new List<Application>();
    }
}
