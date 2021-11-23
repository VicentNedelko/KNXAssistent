using DAL.Models;
using Knx.Bus.Common;
using Knx.Bus.Common.Configuration;
using Knx.Bus.Common.KnxIp;
using Knx.Falcon.Sdk;
using System.Linq;

namespace KNXManager.BusConnection
{
    public class BusCommunicator : IBusCommunicator
    {
        public DiscoveryResult[] Interfaces { get; set; }
        public KnxInterface ActiveInt { get; set; } = new();
        public Bus bus { get; set; }
        public BusCommunicator()
        {
            DiscoveryClient discoveryClient = new(AdapterTypes.All);
            Interfaces = discoveryClient.Discover();
        }
        public bool CheckConnection(string interfaceIp)
        {
            bus = new Bus(new KnxIpTunnelingConnectorParameters(interfaceIp, 0x0e57, false));
            using (bus)
            {
                bus.Connect();
                if (bus.CheckCommunication() == CheckCommunicationResult.Ok)
                {
                    return true;
                }
                bus.Disconnect();
                return false;
            }
        }

        public void SetInterface(string interfaceIp)
        {
            if (bus is not null && bus.State == BusConnectionStatus.Connected)
            {
                bus.Disconnect();
            }
            bus = new Bus(new KnxIpTunnelingConnectorParameters(interfaceIp, 0x057, false));

            ActiveInt.Ip = interfaceIp;
            ActiveInt.Name = (Interfaces.FirstOrDefault(i => i.IpAddress.ToString() == interfaceIp)).FriendlyName;
            ActiveInt.Mac = Interfaces.FirstOrDefault(i => i.IpAddress.ToString() == interfaceIp).MacAddress.ToString();
            ActiveInt.IndividualAddress = Interfaces.FirstOrDefault(i => i.IpAddress.ToString() == interfaceIp).IndividualAddress.ToString();
        }
    }
}
