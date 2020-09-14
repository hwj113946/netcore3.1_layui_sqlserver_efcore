using System;
using System.Collections.Generic;

#nullable disable

namespace netcore.Models
{
    public partial class AppWorkflowApprNode
    {
        public int Id { get; set; }
        public int? ApprFlowId { get; set; }
        public string NodeCode { get; set; }
        public string NodeName { get; set; }
        public string Type { get; set; }
        public int? Num { get; set; }
        public double? Left { get; set; }
        public double? Top { get; set; }
        public double? Width { get; set; }
        public double? Height { get; set; }
    }
}
