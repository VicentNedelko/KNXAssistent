using DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class ACUnit
    {
        public ACUBrand AcuBrand { get; set; }
        public string Description { get; set; }
        public string ErrorFlagGA { get; set; }
        public string ErrorValueGA { get; set; }

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
