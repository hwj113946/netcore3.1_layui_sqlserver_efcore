using System;
using System.Collections.Generic;

#nullable disable

namespace netcore.Models
{
    public partial class AppNews
    {
        public int NewsId { get; set; }
        public int? NewsTypeId { get; set; }
        public string NewsTitle { get; set; }
        public string NewsAuthor { get; set; }
        public string NewsCoverImageUrl { get; set; }
        public DateTime? NewsReleaseTime { get; set; }
        public string NewsContent { get; set; }
        public int? BrowseNumber { get; set; }
        public string Status { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedUser { get; set; }
        public DateTime? CreationDate { get; set; }
        public int? CreationUser { get; set; }
    }
}
