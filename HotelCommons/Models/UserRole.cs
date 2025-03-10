﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HotelsCommons.Models
{
    public class UserRole
    {
        public string UserId { get; set; }
        public User User { get; set; }
        public string RoleId { get; set; }
        public Role Role { get; set; }
    }

    public class UserRoleDTO
    {
        public string UserId { get; set; }
        public List<String> RoleId { get; set; }
    }
}
