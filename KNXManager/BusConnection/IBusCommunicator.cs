using DAL.Models;
using Knx.Bus.Common.KnxIp;
using Knx.Falcon.Sdk;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KNXManager.BusConnection
{
    public interface IBusCommunicator
    {
        public int handlerGvrNumber { get; set; }
        public int handlerScNumber { get; set; }
        public int handlerAcuNumber { get; set; }
        public DiscoveryResult[] Interfaces { get; set; }
        public KnxInterface ActiveInt { get; set; }
        public Bus bus { get; set; }
        public string CheckConnection(string interfaceIp);
        public void SetInterface(string interfaceIp);

    }
}
