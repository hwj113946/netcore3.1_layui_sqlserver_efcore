using System;
using System.Collections.Generic;

#nullable disable

namespace netcore.Models
{
    public partial class AppUserRole
    {
        public int UserRoleId { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
    }
}
