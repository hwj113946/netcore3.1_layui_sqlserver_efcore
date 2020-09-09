using System;
using System.Collections.Generic;

#nullable disable

namespace netcore.Models
{
    public partial class AppFixvalueType
    {
        public int FixvalueTypeId { get; set; }
        public string FixvalueTypeCode { get; set; }
        public string FixvalueTypeName { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedUser { get; set; }
        public DateTime? CreationDate { get; set; }
        public int? CreationUser { get; set; }
    }
}
