using System;
using System.Collections.Generic;

#nullable disable

namespace netcore.Models
{
    public partial class AppButton
    {
        public int ButtonId { get; set; }
        public string ButtonName { get; set; }
        public string ButtonElementId { get; set; }
        public string ButtonEvent { get; set; }
        public string ButtonIcon { get; set; }
        public string ButtonColor { get; set; }
        public double? ButtonSort { get; set; }
    }
}
