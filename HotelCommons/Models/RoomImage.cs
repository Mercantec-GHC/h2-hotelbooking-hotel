using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelsCommons.Models
{
    public class RoomImage
    {
        public int Id { get; set; } // Primary Key
        public string FileName { get; set; } // Stores the file path (e.g., "/RoomImages/xyz.jpg")

        public string RoomID { get; set; } // Foreign Key
        public Room Room { get; set; } // Navigation property
    }
}
