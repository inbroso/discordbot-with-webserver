using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace discordbot.Modules
{
    public class RoleInfo
    {
        public string Name { get; set; }
        public string Color { get; set; }
        public ulong RoleId { get; set; }
        public int Position { get; set; }
    }
}
