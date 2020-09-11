using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace netcore.Models.Excel.ExcelClass
{
    public class ExcelAppUser
    {
        [Column(0)] public string CorpName { get; set; }
        [Column(1)] public string DeptName { get; set; }
        [Column(2)] public string PostName { get; set; }
        [Column(3)] public string UserCode { get; set; }
        [Column(4)] public string UserName { get; set; }
        [Column(5)] public string Phone { get; set; }
        [Column(6)] public string Email { get; set; }
        [Column(7)] public string Address { get; set; }
        [Column(8)] public string Status { get; set; }
    }
}
