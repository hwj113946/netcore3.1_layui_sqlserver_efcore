using System;
using System.Collections.Generic;

#nullable disable

namespace netcore.Models
{
    public partial class FlowNodePro
    {
        public int NodeProId { get; set; }
        public int? NodeId { get; set; }
        public string PageViewUrl { get; set; }
        public int? ApprUserId { get; set; }
        public int? ApprCorpId { get; set; }
        public int? ApprDeptId { get; set; }
        public int? ApprPostId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedUser { get; set; }
        public DateTime? CreationDate { get; set; }
        public int? CreationUser { get; set; }
    }
}
