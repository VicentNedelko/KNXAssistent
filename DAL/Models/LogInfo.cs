using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class LogInfo
    {
        public DateTime TimeStamp { get; set; }
        public string Action { get; set; }
        public string UserName { get; set; }
        public string IP { get; set; }
        public string Auth { get; set; }
        public string Loc { get; set; }
        public string Client { get; set; }
        public string ListType { get; set; }
    }
}
