using DAL.Enums;
using Knx.Bus.Common;
using Knx.Falcon;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class GA
    {
        public string GAddress { get; set; }
        [JsonIgnore]
        public GroupAddress? Address { get; set; }
        public DptType? GType { get; set; }
        public string Description { get; set; }
        //
        public string Unit { get; set; }
        public double? UpperTh { get; set; }
        public double? LowerTh { get; set; }
        public bool Notification { get; set; }
        public bool IsThreshold { get; set; }


        public override string ToString()
        {
            string result = string.Concat(GAddress.ToString(), " > ", GType);
            return result;
        }

        public static DptType IntToDpt(int dpt) => dpt switch
        {
            1001 => DptType.Switch,
            9001 => DptType.Temperature,
            9004 => DptType.Brightness,
            5001 => DptType.Percent,
            1 => DptType.RawValue,
            _ => DptType.Unknown,
        };
    }
}
