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
        public string Code { get; set; }
        public string Description { get; set; }
    }
}
