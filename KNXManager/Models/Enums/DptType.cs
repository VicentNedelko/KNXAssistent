using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KNXManager.Models.Enums
{
    public enum DptType
    {
        Switch = 1001,
        Temperature = 9001,
        Brightness = 9004,
        Percent = 5001,
        Unknown = 0,
        RawValue = 1,
    }
}
