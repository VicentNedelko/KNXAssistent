using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;
using KNXManager.BusConnection;

namespace KNXManager.ACU
{
    public class AcuErrorHandler : IAcuErrorHandler
    {
        private IBusCommunicator _busCommunicator;

        public AcuErrorHandler(IBusCommunicator busCommunicator)
        {
            _busCommunicator = busCommunicator;
        }
        public List<ACUnit> Acus { get; set; }

        public string GetErrorDescriptionByCode(ACError error)
        {
            return "";
        }
    }
}
