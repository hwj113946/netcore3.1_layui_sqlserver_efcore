using System;
using System.Collections.Generic;

#nullable disable

namespace netcore.Models
{
    public partial class AppWorkflowApprTranTemp
    {
        public int Id { get; set; }
        public int? ApprFlowId { get; set; }
        public int? ApprTypeId { get; set; }
        public int? DocId { get; set; }
        public string CurrNodeCode { get; set; }
        public string CurrNodeType { get; set; }
        public string CurrNoteName { get; set; }
        public string NextNodeCode { get; set; }
        public string NextNodeType { get; set; }
        public int? UpperRect { get; set; }
        public string UpperNode { get; set; }
        public int? CurrRect { get; set; }
        public int? WN { get; set; }
        public DateTime? CreationDate { get; set; }
    }
}
