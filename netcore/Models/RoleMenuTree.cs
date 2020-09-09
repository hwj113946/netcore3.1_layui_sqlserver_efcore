using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace netcore.Models
{
    public class RoleMenuTree
    {
        public int id { get; set; }
        public int pid { get; set; }
        public string title { get; set; }
        public string menu_type { get; set; }
        public string @checked { get; set; }
        public string spread { get; set; }
    }
}
