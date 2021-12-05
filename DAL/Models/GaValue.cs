using DAL.Enums;
using Knx.Bus.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class GaValue
    {
        public GroupAddress Address { get; set; }
        public DptType? Type { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
        public string Unit { get; set; }
        public double? UpperTh { get; set; }
        public double? LowerTh { get; set; }
        public bool? Notification  { get; set; }
        public bool IsThreshold { get; set; }
    }
}
