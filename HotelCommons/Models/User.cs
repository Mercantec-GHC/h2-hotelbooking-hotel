using Microsoft.AspNetCore.Identity;

namespace HotelsCommons.Models
{
    public class User : IdentityUser
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public ICollection<Ticket> Tickets { get; set; }
    }
}
