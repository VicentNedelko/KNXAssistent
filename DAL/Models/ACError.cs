using DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class ACError
    {
        public ACUBrand AcuBrand { get; set; }
        public string ErrorCode { get; set; }
        public string ErrorDescription { get; set; }
        public string Comment { get; set; }

        public static string GetCodeDescription(string code)
        {
            return code; // test value
        }
    }
}
