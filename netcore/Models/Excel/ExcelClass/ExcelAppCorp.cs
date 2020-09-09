using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace netcore.Models.Excel.ExcelClass
{
    public class ExcelAppCorp
    {
        [Column(0)]public string CorpCode { get; set; }
        [Column(1)]public string CorpName { get; set; }
        [Column(2)]public string Fax { get; set; }
        [Column(3)]public string Zip { get; set; }
        [Column(4)]public string Email { get; set; }
        [Column(5)]public string ContractPersonName { get; set; }
        [Column(6)]public string ContractPersonPhone { get; set; }
        [Column(7)]public string ContractPersonIdentity { get; set; }
        [Column(8)]public string LawPersonName { get; set; }
        [Column(9)]public string LawPersonPhone { get; set; }
        [Column(10)]public string LawPersonIdentity { get; set; }
        [Column(11)]public string Address { get; set; }
        [Column(12)]public string TaxRqNumber { get; set; }
        [Column(13)]public string Note { get; set; }
        [Column(14)] public string Status { get; set; }
    }
}
