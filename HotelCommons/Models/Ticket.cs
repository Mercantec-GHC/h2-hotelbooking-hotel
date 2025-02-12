using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace HotelsCommons.Models
{
    public class Ticket : Common
    {
        public ICollection<TicketMessage> Messages { get; set; }
        public string Topic { get; set; }
        public string Status { get; set; }
        public string UserID { get; set; }
        public User User { get; set; }
    }

    public class TicketCreateDTO
    {
        [JsonPropertyName("topic")]
        public string Topic { get; set; }

        [JsonPropertyName("message")]
        public string Message { get; set; }
    }

    public class TicketStatusDTO
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }
    }
}
