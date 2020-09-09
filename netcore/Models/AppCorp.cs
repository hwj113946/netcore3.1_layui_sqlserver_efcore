using System;
using System.Collections.Generic;

#nullable disable

namespace netcore.Models
{
    public partial class AppCorp
    {
        public int CorpId { get; set; }
        public string CorpCode { get; set; }
        public string CorpName { get; set; }
        public string Fax { get; set; }
        public string Zip { get; set; }
        public string Email { get; set; }
        public string ContractPersonName { get; set; }
        public string ContractPersonPhone { get; set; }
        public string ContractPersonIdentity { get; set; }
        public string LawPersonName { get; set; }
        public string LawPersonPhone { get; set; }
        public string LawPersonIdentity { get; set; }
        public string Address { get; set; }
        public string TaxRqNumber { get; set; }
        public string Note { get; set; }
        public string Status { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedUser { get; set; }
        public DateTime? CreationDate { get; set; }
        public int? CreationUser { get; set; }
    }
}
