using DAL.Converters;
using DAL.Enums;
using Knx.Bus.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class ACUnit
    {
        public ACUBrand AcuBrand { get; set; }
        public string Description { get; set; }
        [JsonConverter(typeof(GAJsonConverter))]
        public GroupAddress ErrorFlagGA { get; set; }

        [JsonConverter(typeof(GAJsonConverter))]
        public GroupAddress ErrorValueGA { get; set; }

        public static ACUBrand IntToAcuBrand(int code)
        {
            return code switch
            {
                0 => ACUBrand.DAIKIN,
                1 => ACUBrand.MITSUBISHI_ELECTRIC,
                2 => ACUBrand.MITSUBISHI_HEAVY,
                _ => ACUBrand.UNKNOWN,
            };
        }
    }
}
