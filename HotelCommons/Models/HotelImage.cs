using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelsCommons.Models
{
    public class HotelImage
    {
        public int Id { get; set; }
        public string HotelID { get; set; }  
        public string ImagePath { get; set; } 
        public Hotel Hotel { get; set; }
    }

}
