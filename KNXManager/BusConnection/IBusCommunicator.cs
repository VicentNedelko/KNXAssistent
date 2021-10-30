using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KNXManager.BusConnection
{
    public interface IBusCommunicator
    {
        public bool CheckConnection(string interfaceIp);
        public void GetInterface(string interfaceIp);
    }
}
