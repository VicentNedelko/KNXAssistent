using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;
using Knx.Bus.Common;
using Knx.Bus.Common.Configuration;
using Knx.Bus.Common.DatapointTypes;
using KNXManager.BusConnection;
using KNXManager.FileService;
using KNXManager.MessageService;

namespace KNXManager.ACU
{
    public class AcuErrorHandler : IAcuErrorHandler
    {
        public List<ACUnit> ACUnits { get; set; }
        public List<ACError> ACErrors { get; set; }
        public List<ErrorValue> ErrorValues { get; set; }

        private readonly IFileService _fileService;
        private readonly IBusCommunicator _busCommunicator;
        private readonly IMessService _messService;

        public event Func<Task> OnErrorReceived;

        public AcuErrorHandler(IFileService fileService, IBusCommunicator busCommunicator, IMessService messService)
        {
            _fileService = fileService;
            _busCommunicator = busCommunicator;
            _messService = messService;
            ACUnits = _fileService.ReadACUsFromFile();
            ACErrors = _fileService.ReadErrorsFromFile();
            ErrorValues = new();
        }

        public void Bus_OnACUerrorValueReceived(GroupValueEventArgs obj)
        {
            if (ACUnits.Any(unit => unit.ErrorFlagGA == obj.Address))
            {
                if ((bool)obj.Value == true)
                {
                    var ACunit = ACUnits.FirstOrDefault(unit => unit.ErrorFlagGA == obj.Address);
                    if (ACunit is not null)
                    {
                        var rawCode = _busCommunicator.bus.ReadValue(ACunit.ErrorValueGA);
                        var textCode = new Dpt16(Encoding.ASCII).ToValue(rawCode);
                        ErrorValues.Add(new ErrorValue
                        {
                            BrandName = ACunit.AcuBrand,
                            AcuDescription = ACunit.Description,
                            TimeStamp = DateTime.Now,
                            Value = (textCode is not null) ? ACError.GetCodeDescription(textCode.ToString()) : "Can't get CODE. Raw is NULL.",
                        });
                        _messService.AddInfoMessage($"ACU - {ACunit.Description} caught error with value - {ACError.GetCodeDescription(textCode.ToString())}");
                        OnErrorReceived?.Invoke();
                    }
                    else
                    {
                        _messService.AddDangerMessage($"Can't find ACU with Error Flag - {obj.Address}");
                    }
                }
            }
        }

        public void StartMonitor()
        {
            LetsCommunicate();
            _busCommunicator.ActiveInt.State = _busCommunicator.bus?.State.ToString();
            _busCommunicator.bus.GroupValueReceived += Bus_OnACUerrorValueReceived;
            _busCommunicator.handlerAcuNumber++;
            _messService.AddInfoMessage($"Start ACU monitoring on {_busCommunicator.ActiveInt.Ip}-{_busCommunicator.ActiveInt.Name}");
        }

        public void StopMonitor()
        {
            _busCommunicator.bus.GroupValueReadReceived -= Bus_OnACUerrorValueReceived;
            _busCommunicator.handlerAcuNumber--;
            LetsStop();
            _fileService.WriteErrorValuesToFileAsync(ErrorValues);
            _messService.AddInfoMessage($"Stop ACU monitoring on {_busCommunicator.ActiveInt.Ip}-{_busCommunicator.ActiveInt.Name}");

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
