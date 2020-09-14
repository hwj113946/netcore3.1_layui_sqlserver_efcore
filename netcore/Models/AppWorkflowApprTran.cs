using System;
using System.Collections.Generic;

#nullable disable

namespace netcore.Models
{
    public partial class AppWorkflowApprTran
    {
        public int ApprTranId { get; set; }
        public int? ApprId { get; set; }
        public int? ApprUserId { get; set; }
        public int? ApprCorpId { get; set; }
        public int? ApprDeptId { get; set; }
        public int? ApprPostId { get; set; }
        public string ApprNote { get; set; }
        public DateTime? ApprDate { get; set; }
        public string Status { get; set; }
        public int? CurrFlowId { get; set; }
        public string CurrNote { get; set; }
        public string CurrType { get; set; }
        public string NextCode { get; set; }
        public string NextType { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedUser { get; set; }
        public DateTime? CreationDate { get; set; }
        public int? CreationUser { get; set; }
    }
}
