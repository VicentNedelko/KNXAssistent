using Knx.Bus.Common;
using Knx.Bus.Common.Configuration;
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
    public class BusCommunicator : IBusCommunicator
    {
        public DiscoveryResult[] Interfaces { get; set; }

        public KnxInterface ActiveInterface { get; set; } = new();
        //public string Ip { get; set; }
        //public string InterfaceName { get; set; }
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
            bus.Connect();
            ActiveInterface.Ip = interfaceIp;
            ActiveInterface.Name = (Interfaces.FirstOrDefault(i => i.IpAddress.ToString() == interfaceIp)).FriendlyName;
            ActiveInterface.Mac = Interfaces.FirstOrDefault(i => i.IpAddress.ToString() == interfaceIp).MacAddress.ToString();
            ActiveInterface.IndividualAddress = Interfaces.FirstOrDefault(i => i.IpAddress.ToString() == interfaceIp).IndividualAddress.ToString();
        }
    }
}
