﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;
using Knx.Bus.Common;
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

        public void Bus_OnGaValueReceived(GroupValueEventArgs obj)
        {
            if(ACUnits.Any(unit => unit.ErrorFlagGA == obj.Address) && (bool)obj.Value == true)
            {
                var ACunit = ACUnits.FirstOrDefault(unit => unit.ErrorFlagGA == obj.Address);
                if(ACunit is not null)
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
}
