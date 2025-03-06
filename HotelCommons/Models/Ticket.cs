using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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
        [Required(ErrorMessage = "Topic is required.", AllowEmptyStrings = false)]
        [JsonPropertyName("topic")]
        public string Topic { get; set; }

        [Required(ErrorMessage = "Message is required.", AllowEmptyStrings = false)]
        [JsonPropertyName("message")]
        public string Message { get; set; }
    }

    public class TicketStatusDTO
    {
        [JsonPropertyName("status")]
        public string Status { get; set; }
    }

    public static class Util
    {
        public static string CapitalizeFirstLetter(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return input;
            }

            return char.ToUpper(input[0]) + input.Substring(1);
        }
    }

    public class AllTicketsResult
    {
        public string Id { get; set; }
        public string Topic { get; set; }
        private string _status { get; set; }
        public string Status
        {
            get { return _status; }
            set { _status = Util.CapitalizeFirstLetter(value); }
        }
        public DateTime CreatedAt { get; set; }
    }

    public class TicketResult
    {
        public string Id { get; set; }
        public string Topic { get; set; }
        private string _status { get; set; }
        public string Status
        {
            get { return _status; }
            set { _status = Util.CapitalizeFirstLetter(value); }
        }
        public DateTime CreatedAt { get; set; }
        public List<MessageResult> Messages { get; set; }

        public Dictionary<string, bool> GetStatusOptions()
        {
            Dictionary<string, bool> dict = new()
            {
                { "Open", Status == "Open" },
                { "In Progress", Status == "In Progress" },
                { "Closed", Status == "Closed" }
            };

            return dict;
        }
    }
}
