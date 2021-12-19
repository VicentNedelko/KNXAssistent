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
        private readonly IFileService _fileService;
        private readonly IMessService _messService;
        private readonly IBot _bot;
        private readonly IAcuErrorHandler _acuErrorHandler;
        public int handlerGvrNumber;
        private int _handlerScNumber;
        public DiscoveryResult[] Interfaces { get; set; }
        public KnxInterface ActiveInt { get; set; } = new();
        public Bus bus { get; set; }
        public List<GaValue> gaValues { get; set; }
        public List<GA> gaSbcList { get; set; }
        public event Func<Task> OnGaReceived;

        public string Information { get; set; }
        public string ConnectionState { get; set; } = "Not Applied";
        public BusCommunicator(IFileService fileService, IMessService messService, IBot bot)
        {
            DiscoveryClient discoveryClient = new(AdapterTypes.All);
            Interfaces = discoveryClient.Discover();
            _fileService = fileService;
            gaValues = new();
            _messService = messService;
            _bot = bot;
            gaSbcList = _fileService.ReadSbcFromFile();
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
            ActiveInt.State = (bus is not null) ? bus.State.ToString() : "Not connected to KNX";
        }

        public void StartMonitor()
        {
            LetsCommunicate();
            bus.GroupValueReceived += Bus_GroupValueSbcReceived;
            bus.GroupValueReceived += _acuErrorHandler.Bus_OnGaValueReceived;
            handlerGvrNumber++;
            bus.StateChanged += Bus_StateChanged;
            _handlerScNumber++;
            OnGaReceived?.Invoke();
            _messService.AddInfoMessage($"Start monitoring on {ActiveInt.Ip}-{ActiveInt.Name}");
        }

        private void LetsCommunicate()
        {
            bus ??= new(new KnxIpTunnelingConnectorParameters(ActiveInt.Ip, 0x0e57, false));
            if (!bus.IsConnected)
            {
                bus.Connect();
            }
            ActiveInt.State = bus?.State.ToString();
            //ConnectionState = bus?.State.ToString();
        }

        private void LetsStop()
        {
            if(handlerGvrNumber + _handlerScNumber == 0)
            {
                bus.Disconnect();
                bus.Dispose();
            }
        }

        private void Bus_StateChanged(BusConnectionStatus obj)
        {
            ActiveInt.State = obj.ToString();
            _fileService.WriteSbcValueToFile(gaValues);
            _messService.AddWarningMessage($"Interface change state to - {obj}");
        }

        public void StopMonitor()
        {
            bus.GroupValueReceived -= Bus_GroupValueSbcReceived;
            handlerGvrNumber--;
            bus.StateChanged -= Bus_StateChanged;
            _handlerScNumber--;
            LetsStop();
            _fileService.WriteSbcValueToFile(gaValues);
            _messService.AddInfoMessage($"Stop monitoring on {ActiveInt.Ip}-{ActiveInt.Name}");
        }

        private void Bus_GroupValueSbcReceived(GroupValueEventArgs obj)
        {
            if (gaSbcList.Any(ga => ga.Address == obj.Address) && bus.IsConnected)
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
                addingGA.Notification = checkedGA.Notification;
                gaValues.Add(addingGA);
                if (checkedGA.Notification) { _bot.SendMessageAsync($"Info! {addingGA.Description} = {addingGA.Value} {addingGA.Unit}"); }
                OnGaReceived?.Invoke();
            }
        }

        public void StartACUHandler()
        {
            LetsCommunicate();
            bus.GroupValueReceived += Bus_ACUErrorHandler;
            _messService.AddInfoMessage($"Start ACU Handling on {ActiveInt.Ip}-{ActiveInt.Name}");
        }

        private void Bus_ACUErrorHandler(GroupValueEventArgs obj)
        {
            throw new NotImplementedException();
        }

        public void StopACUHandler()
        {
            bus.GroupValueReceived -= Bus_ACUErrorHandler;
            LetsStop();
            _messService.AddInfoMessage($"Stop ACU Handling on {ActiveInt.Ip}-{ActiveInt.Name}");
        }
    }
}
