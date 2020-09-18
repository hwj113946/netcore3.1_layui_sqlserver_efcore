using System;
using System.Collections.Generic;

#nullable disable

namespace netcore.Models
{
    public partial class FlowLine
    {
        public int LineId { get; set; }
        public int? ApprFlowId { get; set; }
        public string Type { get; set; }
        public string LineCode { get; set; }
        public string LineName { get; set; }
        public int? Num { get; set; }
        public string From { get; set; }
        public string To { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedUser { get; set; }
        public DateTime? CreationDate { get; set; }
        public int? CreationUser { get; set; }
    }
}
