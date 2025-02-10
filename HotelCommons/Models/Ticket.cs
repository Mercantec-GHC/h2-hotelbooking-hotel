using System.Reflection.Metadata;

namespace HotelsCommons.Models
{
    public class Ticket
    {
        public string Id { get; set; }

        public ICollection<TicketMessage> Messages { get; set; }

        public string UserEmail { get; set; }
        public User User { get; set; }
    }
}
