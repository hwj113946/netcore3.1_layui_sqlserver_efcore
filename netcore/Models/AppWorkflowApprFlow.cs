using System;
using System.Collections.Generic;

#nullable disable

namespace netcore.Models
{
    public partial class AppWorkflowApprFlow
    {
        public int ApprFlowId { get; set; }
        public int? ApprTypeId { get; set; }
        public string ApprFlowName { get; set; }
        public string Note { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedUser { get; set; }
        public DateTime? CreationDate { get; set; }
        public int? CreationUser { get; set; }
    }
}
