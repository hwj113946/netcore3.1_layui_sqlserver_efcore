using System;
using System.Collections.Generic;

#nullable disable

namespace netcore.Models
{
    public partial class AppUser
    {
        public int UserId { get; set; }
        public string UserCode { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string IdCardNumber { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        public string Address { get; set; }
        public int? CorpId { get; set; }
        public int? DeptId { get; set; }
        public int? PostId { get; set; }
        public string Status { get; set; }
        public DateTime? ModifyPasswordDate { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedUser { get; set; }
        public DateTime? CreationDate { get; set; }
        public int? CreationUser { get; set; }
    }
}
