using System;
using System.Collections.Generic;

#nullable disable

namespace netcore.Models
{
    public partial class AppWorkflowNodePropety
    {
        public int ProId { get; set; }
        public int? ApprFlowId { get; set; }
        public string NodeCode { get; set; }
        public string Type { get; set; }
        public string Rect { get; set; }
        public string PageViewUrl { get; set; }
        public string IsConfirmUser { get; set; }
        public int? ApprUserId { get; set; }
        public int? ApprCorpId { get; set; }
        public int? ApprDeptId { get; set; }
        public int? ApprPostId { get; set; }
    }
}
