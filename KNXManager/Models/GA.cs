using KNXManager.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KNXManager.Models
{
    public class GA
    {
        //public Guid Id { get; set; }
        public string GAddress { get; set; }
        public DptType? GType { get; set; }
        public string Description { get; set; }


        public override string ToString()
        {
            string result = string.Concat(GAddress, " > ", GType);
            return result;
        }
    }
}
