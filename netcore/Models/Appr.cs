using System;
using System.Collections.Generic;

#nullable disable

namespace netcore.Models
{
    public partial class Appr
    {
        public int ApprId { get; set; }
        public int? ApprFlowId { get; set; }
        public int? SourceId { get; set; }
        public string Tile { get; set; }
        public string Note { get; set; }
        public DateTime? SubmissionTime { get; set; }
        public int? Submitter { get; set; }
        public int? SubmitterCorp { get; set; }
        public int? SubmitterDept { get; set; }
        public int? SubmitterPost { get; set; }
        public string SubmitterPhone { get; set; }
        public string ApprNote { get; set; }
        public string Status { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedUser { get; set; }
        public DateTime? CreationDate { get; set; }
        public int? CreationUser { get; set; }
    }
}
