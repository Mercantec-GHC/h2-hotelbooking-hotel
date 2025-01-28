using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelsCommons.Models
{
    public class Room : Common
    {
        public List<Booking> Bookings { get; set; }
        public string HotelID { get; set; }
        //public string available {  get; set; }
        //public int Price { get; set; }
    }
    public class RoomDTO : Common
    {
        public string HotelID { get; set; }
        //public string available {  get; set; }
        //public int Price { get; set; }
    }
}
