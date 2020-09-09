using System;
using System.Collections.Generic;

#nullable disable

namespace netcore.Models
{
    public partial class AppDept
    {
        public int DeptId { get; set; }
        public int? CorpId { get; set; }
        public string DeptCode { get; set; }
        public string DeptName { get; set; }
        public string Note { get; set; }
        public string Status { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedUser { get; set; }
        public DateTime? CreationDate { get; set; }
        public int? CreationUser { get; set; }
    }
}
