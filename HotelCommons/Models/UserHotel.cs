using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelsCommons.Models
{
     public class UserHotel
    {
        public string UserId { get; set; }
        public User User { get; set; }
        public string HotelId { get; set; }
        public Hotel Hotel { get; set; }
    }

    public class UserHotelsDTO
    {
        public string UserId { get; set; }
        public List<String> HotelIds { get; set; }
    }
}
