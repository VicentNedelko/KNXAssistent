using KNXManager.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KNXManager.Models
{
    public class GAWithTh
    {
        public string GAddress { get; set; }
        public DptType? GType { get; set; }
        public string Description { get; set; }
        public double UpperTh { get; set; }
        public double LowerTh { get; set; }
    }
}
