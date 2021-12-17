using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;

namespace KNXManager.ACU
{
    public interface IAcuErrorHandler
    {
        public List<ACUnit> ACUnits { get; set; }
        public string GetErrorDescriptionByCode(ACError error);
    }
}
