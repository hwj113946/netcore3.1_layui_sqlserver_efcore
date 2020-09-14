using System;
using System.Collections.Generic;

#nullable disable

namespace netcore.Models
{
    public partial class AppWorkflowAppr
    {
        public int ApprId { get; set; }
        public int? ApprFlowId { get; set; }
        public int? DocId { get; set; }
        public string PageViewUrl { get; set; }
        public string DocTitle { get; set; }
        public string DocNote { get; set; }
        public int? ApplUserId { get; set; }
        public int? ApplCorpId { get; set; }
        public int? ApplDeptId { get; set; }
        public int? ApplPostId { get; set; }
        public DateTime? ApplDate { get; set; }
        public int? CurrApprUserId { get; set; }
        public string CurrApprNote { get; set; }
        public string Status { get; set; }
        public int? CurrFlowId { get; set; }
        public string ConfirmNodeCode { get; set; }
        public string ConfirmNodeType { get; set; }
        public string ApprNodeCode { get; set; }
        public string ApprNodeType { get; set; }
        public string UpperNodeCode { get; set; }
        public int? UpperFlowId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedUser { get; set; }
        public DateTime? CreationDate { get; set; }
        public int? CreationUser { get; set; }
    }
}
