using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelsCommons.Models
{
    public class Hotel : Common
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Land { get; set; }
        public string By { get; set; }
        public List<Room> Rooms { get; set; }
      //  public List<User> Workers { get; set; }
        public List<Booking> Bookings { get; set; }
    }
    public class CreateHotelDTO : Common
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public string Land { get; set; }
        public string By { get; set; }
    }
}
