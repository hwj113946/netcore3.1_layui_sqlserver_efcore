using System;
using System.Collections.Generic;

#nullable disable

namespace netcore.Models
{
    public partial class AppPost
    {
        public int PostId { get; set; }
        public int? ParentPostId { get; set; }
        public int? DeptId { get; set; }
        public string PostCode { get; set; }
        public string PostName { get; set; }
        public string Note { get; set; }
        public string Status { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedUser { get; set; }
        public DateTime? CreationDate { get; set; }
        public int? CreationUser { get; set; }
    }
}
