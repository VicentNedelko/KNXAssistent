﻿using DAL.Models;
using Knx.Bus.Common.KnxIp;
using Knx.Falcon.Sdk;
using System.Collections.Generic;

namespace KNXManager.BusConnection
{
    public interface IBusCommunicator
    {
        public DiscoveryResult[] Interfaces { get; set; }
        public KnxInterface ActiveInt { get; set; }
        public Bus bus { get; set; }
        public List<GaValue> gaValues { get; set; }
        public string ConnectionState { get; set; }
        public string CheckConnection(string interfaceIp);
        public void SetInterface(string interfaceIp);

        public void StartMonitor();
        public void StopMonitor();
    }
}
