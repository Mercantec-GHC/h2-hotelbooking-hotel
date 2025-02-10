using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelsCommons.Models
{
    public class TicketMessage
    {
        public string Id { get; set; }

        public string Message { get; set; }

        public string TicketId { get; set; }
        public Ticket Ticket { get; set; }
    }
}
