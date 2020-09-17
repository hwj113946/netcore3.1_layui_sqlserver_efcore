using System;
using System.Collections.Generic;

#nullable disable

namespace netcore.Models
{
    public partial class ApprTran
    {
        public int TranId { get; set; }
        public int? ApprId { get; set; }
        public int? TranNumber { get; set; }
        public int? Submitter { get; set; }
        public int? SubmitterCorp { get; set; }
        public int? SubmitterDept { get; set; }
        public int? SubmitterPost { get; set; }
        public string SubmitterNote { get; set; }
        public DateTime? SubmissionTime { get; set; }
        public int? SubmitNodeId { get; set; }
        public int? LastSubmitNodeId { get; set; }
        public int? NextSubmitNodeId { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedUser { get; set; }
        public DateTime? CreationDate { get; set; }
        public int? CreationUser { get; set; }
    }
}
