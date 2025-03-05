using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HotelsCommons.Models
{
    public class TicketMessage : Common
    {
        public string Message { get; set; }
        public string TicketId { get; set; }
        public Ticket Ticket { get; set; }
        public string UserID { get; set; }
        public User User { get; set; }
    }

    public class MessageCreateDTO
    {
        [JsonPropertyName("ticketId")]
        public string TicketId { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }

    public class MessageResult
    {
        public string Id { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
