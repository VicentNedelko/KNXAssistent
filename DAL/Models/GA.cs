using DAL.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DAL.Models
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
