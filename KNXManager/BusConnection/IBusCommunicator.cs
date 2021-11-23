using DAL.Models;
using Knx.Bus.Common.KnxIp;
using Knx.Falcon.Sdk;

namespace KNXManager.BusConnection
{
    public interface IBusCommunicator
    {
        public DiscoveryResult[] Interfaces { get; set; }
        public KnxInterface ActiveInt { get; set; }
        public Bus bus { get; set; }
        public bool CheckConnection(string interfaceIp);
        public void SetInterface(string interfaceIp);
    }
}
