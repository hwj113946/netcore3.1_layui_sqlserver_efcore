using System;
using System.Collections.Generic;

#nullable disable

namespace netcore.Models
{
    public partial class AppWorkflowApprLine
    {
        public int Id { get; set; }
        public int? ApprFlowId { get; set; }
        public string LineCode { get; set; }
        public string LineName { get; set; }
        public string Type { get; set; }
        public int? Num { get; set; }
        public string From { get; set; }
        public string To { get; set; }
    }
}
