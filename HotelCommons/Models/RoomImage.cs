using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelsCommons.Models
{
    public class RoomImage
    {
        public int Id { get; set; }
        public string RoomID { get; set; }  // Foreign Key
        public string ImagePath { get; set; } = string.Empty; // Path or URL to the image
        public Room Room { get; set; }
    }
}
