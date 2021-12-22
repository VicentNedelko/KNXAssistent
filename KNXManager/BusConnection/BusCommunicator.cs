using DAL.Models;
using DAL.Enums;
using Knx.Bus.Common;
using Knx.Bus.Common.Configuration;
using Knx.Bus.Common.KnxIp;
using Knx.Falcon.Sdk;
using KNXManager.FileService;
using System.Collections.Generic;
using System.Linq;
using Knx.Bus.Common.DatapointTypes;
using KNXManager.MessageService;
using System;
using KNXManager.BotManager;
using System.Threading.Tasks;
using KNXManager.ACU;

namespace KNXManager.BusConnection
{
    public class BusCommunicator : IBusCommunicator
    {
        private readonly IMessService _messService;
        public DiscoveryResult[] Interfaces { get; set; }
        public KnxInterface ActiveInt { get; set; } = new();
        public Bus bus { get; set; }
        public string Information { get; set; }
        public int handlerGvrNumber { get; set; }
        public int handlerScNumber { get; set; }
        public int handlerAcuNumber { get; set; }

        public BusCommunicator(IMessService messService)
        {
            DiscoveryClient discoveryClient = new(AdapterTypes.All);
            Interfaces = discoveryClient.Discover();
            _messService = messService;
        }

        public string CheckConnection(string interfaceIp)
        {
            using Bus bus_check = new(new KnxIpTunnelingConnectorParameters(interfaceIp, 0x0e57, false));
            bus_check.Connect();
            return bus_check.CheckCommunication().ToString();
        }

        public void SetInterface(string interfaceIp)
        {
            ActiveInt.Ip = interfaceIp;
            ActiveInt.Name = (Interfaces.FirstOrDefault(i => i.IpAddress.ToString() == interfaceIp)).FriendlyName;
            ActiveInt.Mac = Interfaces.FirstOrDefault(i => i.IpAddress.ToString() == interfaceIp).MacAddress.ToString();
            ActiveInt.IndividualAddress = Interfaces.FirstOrDefault(i => i.IpAddress.ToString() == interfaceIp).IndividualAddress.ToString();
            ActiveInt.State = (bus is not null) ? bus.State.ToString() : "BUS connection is not established";
        }
    }
}
