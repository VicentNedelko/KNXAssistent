using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class KnxInterface
    {
        public string Ip { get; set; }
        public string Name { get; set; }
        public bool? Connected { get; set; }
        public string Mac { get; set; }
        public string IndividualAddress { get; set; }
    }
}
