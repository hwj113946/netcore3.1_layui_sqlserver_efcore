using System;
using System.Collections.Generic;

#nullable disable

namespace netcore.Models
{
    public partial class AppMenu
    {
        public int MenuId { get; set; }
        public int ParentMenuId { get; set; }
        public string MenuType { get; set; }
        public string MenuName { get; set; }
        public string MenuUrl { get; set; }
        public string MenuIcon { get; set; }
        public double? MenuSort { get; set; }
    }
}
