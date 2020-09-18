using System;
using System.Collections.Generic;

#nullable disable

namespace netcore.Models
{
    public partial class FlowLinePro
    {
        public int LineProId { get; set; }
        public int? FlowId { get; set; }
        public string LineCode { get; set; }
        public string Sql { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedUser { get; set; }
        public DateTime? CreationDate { get; set; }
        public int? CreationUser { get; set; }
    }
}
