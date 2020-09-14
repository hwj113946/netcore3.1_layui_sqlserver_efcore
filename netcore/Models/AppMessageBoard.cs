using System;
using System.Collections.Generic;

#nullable disable

namespace netcore.Models
{
    public partial class AppMessageBoard
    {
        public int MessageId { get; set; }
        public string MessageType { get; set; }
        public int? SourceId { get; set; }
        public string MessageContent { get; set; }
        public DateTime? CreationDate { get; set; }
        public int? CreationUser { get; set; }
    }
}
