using System;
using System.Collections.Generic;

#nullable disable

namespace netcore.Models
{
    public partial class AppRoleMenu
    {
        public int RoleMenuId { get; set; }
        public int RoleId { get; set; }
        public int MenuId { get; set; }
    }
}
