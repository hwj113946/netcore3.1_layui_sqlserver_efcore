using System;
using System.Collections.Generic;

#nullable disable

namespace netcore.Models
{
    public partial class AppCorpBank
    {
        public int CorpBankId { get; set; }
        public int? CorpId { get; set; }
        public string BankProvince { get; set; }
        public string BankCity { get; set; }
        public string BankName { get; set; }
        public string BankAccount { get; set; }
        public string BankNo { get; set; }
        public string Note { get; set; }
        public string Status { get; set; }
        public DateTime? LastModifiedDate { get; set; }
        public int? LastModifiedUser { get; set; }
        public DateTime? CreationDate { get; set; }
        public int? CreationUser { get; set; }
    }
}
