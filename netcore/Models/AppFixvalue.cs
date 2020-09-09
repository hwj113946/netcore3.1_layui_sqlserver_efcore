using System;
using System.Collections.Generic;

#nullable disable

namespace netcore.Models
{
    public partial class AppFixvalue
    {
        public int FixvalueId { get; set; }
        public int FixvalueTypeId { get; set; }
        public string FixvalueCode { get; set; }
        public string FixvalueName { get; set; }
        public string Status { get; set; }
        public string Note { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedUser { get; set; }
        public DateTime? CreationDate { get; set; }
        public int? CreationUser { get; set; }
    }
}
