using System;
using System.Collections.Generic;

#nullable disable

namespace netcore.Models
{
    public partial class ApprTranTemp
    {
        public int TranTemp { get; set; }
        public int? ApprFlowId { get; set; }
        public int? ApprTypeId { get; set; }
        public int? SourceId { get; set; }
        public int? CurrRect { get; set; }
        public string CurrNodeName { get; set; }
        public string CurrNodeCode { get; set; }
        public string CurrNodeType { get; set; }
        public string NextNodeCode { get; set; }
        public string NextNodeType { get; set; }
        public int? UpperRect { get; set; }
        public string UpperNodeCode { get; set; }
        public int? WN { get; set; }
    }
}
