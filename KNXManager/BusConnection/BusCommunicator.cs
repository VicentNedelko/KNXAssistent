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

namespace KNXManager.BusConnection
{
    public class BusCommunicator : IBusCommunicator
    {
        private readonly IFileService _fileService;
        public DiscoveryResult[] Interfaces { get; set; }
        public KnxInterface ActiveInt { get; set; } = new();
        public Bus bus { get; set; }
        public List<GaValue> gaValues { get; set; }
        public List<GA> gaSbcList { get; set; }
        public BusCommunicator(IFileService fileService)
        {
            DiscoveryClient discoveryClient = new(AdapterTypes.All);
            Interfaces = discoveryClient.Discover();
            _fileService = fileService;
            gaValues = new();
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

        public void StartMonitor()
        {
            gaSbcList = _fileService.ReadSbcFromFile();
            if(bus.State != BusConnectionStatus.Connected)
            {
                bus.Connect();
            }
            bus.GroupValueReceived += Bus_GroupValueReceived;
        }

        public void StopMonitor()
        {
            bus.GroupValueReceived -= Bus_GroupValueReceived;
            if(bus.State == BusConnectionStatus.Connected)
            {
                bus.Disconnect();
            }
            _fileService.WriteSbcValueToFile(gaValues);
        }

        // for test perposes
        private void Bus_GroupValueReceived(GroupValueEventArgs obj)
        {
            var addingGA = new GaValue
            {
                Address = obj.Address,
                Type = DptType.RawValue,
                Description = "Description unavailable",
                Value = obj.Value.ToString(),
                Unit = "Raw format"
            };
            gaValues.Add(addingGA);
        }

        // actual method (NOT test)
        private void Bus_GroupValueSbcReceived(GroupValueEventArgs obj)
        {
            if(gaSbcList.Any(ga => ga.Address == obj.Address))
            {
                var checkedGA = gaSbcList.First(ga => ga.Address == obj.Address);
                var addingGA = new GaValue
                {
                    Address = GroupAddress.Parse(checkedGA.GAddress),
                    Type = checkedGA.GType,
                    Description = checkedGA.Description,
                };
                addingGA.Value = addingGA.Type switch
                {
                    DptType.Switch => new Dpt1().ToTypedValue(obj.Value).ToString(),
                    DptType.Percent => new Dpt5().ToTypedValue(obj.Value).ToString(),
                    DptType.Temperature => new Dpt9().ToTypedValue(obj.Value).ToString(),
                    DptType.Brightness => new Dpt9().ToTypedValue(obj.Value).ToString(),
                    _ => obj.Value.ToString(),
                };
                addingGA.Unit = addingGA.Type switch
                {
                    DptType.Switch => string.Empty,
                    DptType.Percent => "%",
                    DptType.Temperature => "°C",
                    DptType.Brightness => "Lux",
                    _ => string.Empty,
                };
                gaValues.Add(addingGA);
            }
        }
    }
}
