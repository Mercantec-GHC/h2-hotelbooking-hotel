using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelAdmin.WebView.Models
{
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

    public class MessageResult
    {
        public string Id { get; set; }
        public string UserID { get; set; }
        public string UserName { get; set; }
        public string Message { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}