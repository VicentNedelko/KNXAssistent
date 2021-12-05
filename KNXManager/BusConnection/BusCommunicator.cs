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

namespace KNXManager.BusConnection
{
    public class BusCommunicator : IBusCommunicator
    {
        private readonly IFileService _fileService;
        private readonly IMessService _messService;
        public DiscoveryResult[] Interfaces { get; set; }
        public KnxInterface ActiveInt { get; set; } = new();
        public Bus bus { get; set; }
        public List<GaValue> gaValues { get; set; }
        public List<GA> gaSbcList { get; set; }

        public string Information { get; set; }
        public string ConnectionState { get; set; } = "Not Applied";
        public BusCommunicator(IFileService fileService, IMessService messService)
        {
            DiscoveryClient discoveryClient = new(AdapterTypes.All);
            Interfaces = discoveryClient.Discover();
            _fileService = fileService;
            gaValues = new();
            _messService = messService;
        }

        public string CheckConnection(string interfaceIp)
        {
            bus = new Bus(new KnxIpTunnelingConnectorParameters(interfaceIp, 0x0e57, false));
            using (bus)
            {
                bus.Connect();
                string state = bus.CheckCommunication().ToString();
                bus.Disconnect();
                return state;
            }
        }

        public void SetInterface(string interfaceIp)
        {
            ActiveInt.Ip = interfaceIp;
            ActiveInt.Name = (Interfaces.FirstOrDefault(i => i.IpAddress.ToString() == interfaceIp)).FriendlyName;
            ActiveInt.Mac = Interfaces.FirstOrDefault(i => i.IpAddress.ToString() == interfaceIp).MacAddress.ToString();
            ActiveInt.IndividualAddress = Interfaces.FirstOrDefault(i => i.IpAddress.ToString() == interfaceIp).IndividualAddress.ToString();
        }

        public void StartMonitor()
        {
            gaSbcList = _fileService.ReadSbcFromFile();
            bus = new(new KnxIpTunnelingConnectorParameters(ActiveInt.Ip, 0x0e57, false));
            bus.Connect();
            bus.GroupValueReceived += Bus_GroupValueSbcReceived;
            bus.StateChanged += Bus_StateChanged;
            _messService.AddInfoMessage($"Start monitoring on {ActiveInt.Ip}-{ActiveInt.Name}");
        }

        private void Bus_StateChanged(BusConnectionStatus obj)
        {
            ConnectionState = obj.ToString();
            bus.Dispose();
            _fileService.WriteSbcValueToFile(gaValues);
            _messService.AddWarningMessage($"Interface change state to - {obj}");
        }

        public void StopMonitor()
        {
            bus.GroupValueReceived -= Bus_GroupValueSbcReceived;
            bus.StateChanged -= Bus_StateChanged;
            bus.Disconnect();
            bus.Dispose();
            _fileService.WriteSbcValueToFile(gaValues);
            _messService.AddInfoMessage($"Stop monitoring on {ActiveInt.Ip}-{ActiveInt.Name}");
        }

        private void Bus_GroupValueSbcReceived(GroupValueEventArgs obj)
        {
            if (gaSbcList.Any(ga => ga.Address == obj.Address))
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
