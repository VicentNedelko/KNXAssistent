using Knx.Bus.Common.KnxIp;
using Knx.Falcon.Sdk;
using KNXManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KNXManager.BusConnection
{
    public interface IBusCommunicator
    {
        public DiscoveryResult[] Interfaces { get; set; }
        public KnxInterface ActiveInterface { get; set; }
        public Bus bus { get; set; }
        public bool CheckConnection(string interfaceIp);
        public void SetInterface(string interfaceIp);
    }
}
