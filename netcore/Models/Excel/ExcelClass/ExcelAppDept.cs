using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace netcore.Models.Excel.ExcelClass
{
    public class ExcelAppDept
    {
        [Column(0)]public string CorpName { get; set; }
        [Column(1)]public string DeptCode { get; set; }
        [Column(2)]public string DeptName { get; set; }
        [Column(3)]public string Note { get; set; }
        [Column(4)] public string Status { get; set; }
    }
}
