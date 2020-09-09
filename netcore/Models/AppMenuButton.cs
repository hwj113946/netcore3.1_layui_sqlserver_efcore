using System;
using System.Collections.Generic;

#nullable disable

namespace netcore.Models
{
    public partial class AppMenuButton
    {
        public int MenuButtonId { get; set; }
        public int MenuId { get; set; }
        public int ButtonId { get; set; }
    }
}
