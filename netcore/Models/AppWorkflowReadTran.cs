using System;
using System.Collections.Generic;

#nullable disable

namespace netcore.Models
{
    public partial class AppWorkflowReadTran
    {
        public int ReadTranId { get; set; }
        public int? ApprTranId { get; set; }
        public int? ApprId { get; set; }
        public int? ReadUserId { get; set; }
        public int? ReadCorpId { get; set; }
        public int? ReadDeptId { get; set; }
        public int? ReadPostId { get; set; }
        public string ReadNote { get; set; }
        public DateTime? ReadDate { get; set; }
        public string Status { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedUser { get; set; }
        public DateTime? CreationDate { get; set; }
        public int? CreationUser { get; set; }
    }
}
