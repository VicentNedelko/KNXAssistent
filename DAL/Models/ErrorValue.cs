using DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
{
    public class ErrorValue
    {
        public ACUBrand BrandName { get; set; }
        public string AcuDescription { get; set; }
        public DateTime TimeStamp { get; set; }
        public string Value { get; set; }

    }
}
