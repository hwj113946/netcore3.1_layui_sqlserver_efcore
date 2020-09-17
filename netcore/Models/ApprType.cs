using System;
using System.Collections.Generic;

#nullable disable

namespace netcore.Models
{
    public partial class ApprType
    {
        public int ApprTypeId { get; set; }
        public string ApprTypeCode { get; set; }
        public string ApprTypeName { get; set; }
        public string TableName { get; set; }
        public string TablePkName { get; set; }
        public string TableStatusName { get; set; }
        public string TableApprIdName { get; set; }
        public string ApprStartStatus { get; set; }
        public string ApprEndStatus { get; set; }
        public string ApprCancelStatus { get; set; }
        public string PageViewUrl { get; set; }
        public string TransProcName { get; set; }
        public string Status { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedUser { get; set; }
        public DateTime? CreationDate { get; set; }
        public int? CreationUser { get; set; }
    }
}
