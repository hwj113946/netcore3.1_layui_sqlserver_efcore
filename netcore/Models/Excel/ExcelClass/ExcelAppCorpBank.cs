using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace netcore.Models.Excel.ExcelClass
{
    public class ExcelAppCorpBank
    {
        [Column(0)]public string CorpCode { get; set; }
        [Column(1)] public string CorpName { get; set; }
        [Column(2)]public string BankProvince { get; set; }
        [Column(3)]public string BankCity { get; set; }
        [Column(4)]public string BankName { get; set; }
        [Column(5)]public string BankAccount { get; set; }
        [Column(6)]public string BankNo { get; set; }
        [Column(7)]public string Note { get; set; }
        [Column(8)] public string Status { get; set; }
    }
}
