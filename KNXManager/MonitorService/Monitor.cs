using DAL.Enums;
using DAL.Models;
using Knx.Bus.Common;
using Knx.Bus.Common.DatapointTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KNXManager.FileService;
using KNXManager.BotManager;
using KNXManager.BusConnection;
using KNXManager.MessageService;
using Knx.Bus.Common.Configuration;

namespace KNXManager.MonitorService
{
    public class Monitor : IMonitor
    {
        public List<GaValue> gaValues { get; set; }
        public List<GA> gaSbcList { get; set; }

        public event Func<Task> OnGaReceived;
        private readonly IFileService _fileService;
        private readonly IBot _bot;
        private readonly IBusCommunicator _busCommunicator;
        private readonly IMessService _messService;

        public Monitor(IFileService fileService, IBot bot, IBusCommunicator busCommunicator, IMessService messService)
        {
            _fileService = fileService;
            _bot = bot;
            _busCommunicator = busCommunicator;
            _messService = messService;
            gaSbcList = _fileService.ReadSbcFromFile();
            gaValues = new();
        }

        public void Bus_GroupValueSbcReceived(GroupValueEventArgs obj)
        {
            if (gaSbcList.Any(ga => ga.Address == obj.Address) && _busCommunicator.bus.IsConnected)
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

        public void StartMonitor()
        {
            LetsCommunicate();
            _busCommunicator.ActiveInt.State = _busCommunicator.bus?.State.ToString();
            _busCommunicator.bus.GroupValueReceived += Bus_GroupValueSbcReceived;
            _busCommunicator.handlerGvrNumber++;
            _busCommunicator.handlerScNumber++;
            OnGaReceived?.Invoke();
            _messService.AddInfoMessage($"Start monitoring on {_busCommunicator.ActiveInt.Ip}-{_busCommunicator.ActiveInt.Name}");
        }

        public void StopMonitor()
        {
            _busCommunicator.bus.GroupValueReceived -= Bus_GroupValueSbcReceived;
            _busCommunicator.handlerGvrNumber--;
            LetsStop();
            _fileService.WriteSbcValueToFile(gaValues);
            _messService.AddInfoMessage($"Stop monitoring on {_busCommunicator.ActiveInt.Ip}-{_busCommunicator.ActiveInt.Name}");
        }

        private void LetsCommunicate()
        {
            _busCommunicator.bus ??= new(new KnxIpTunnelingConnectorParameters(_busCommunicator.ActiveInt.Ip, 0x0e57, false));
            if (!_busCommunicator.bus.IsConnected)
            {
                _busCommunicator.bus.Connect();
            }
            _busCommunicator.ActiveInt.State = _busCommunicator.bus?.State.ToString();
        }

        private void LetsStop()
        {
            if (_busCommunicator.handlerGvrNumber + _busCommunicator.handlerScNumber == 0)
            {
                _busCommunicator.bus.Disconnect();
                _busCommunicator.bus.Dispose();
            }
        }
    }
}
