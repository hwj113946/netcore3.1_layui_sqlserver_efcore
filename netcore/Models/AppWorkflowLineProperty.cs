using System;
using System.Collections.Generic;

#nullable disable

namespace netcore.Models
{
    public partial class AppWorkflowLineProperty
    {
        public int ProId { get; set; }
        public int? ApprFlowId { get; set; }
        public string LineCode { get; set; }
        public string Sql { get; set; }
    }
}
