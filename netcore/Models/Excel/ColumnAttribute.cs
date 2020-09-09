using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace netcore.Models.Excel
{
    public class ColumnAttribute:Attribute
    {
        public ColumnAttribute(int index)
        {
            Index = index;
        }
        public int Index { get; set; }
    }
}
