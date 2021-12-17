using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;
using KNXManager.BusConnection;
using KNXManager.FileService;

namespace KNXManager.ACU
{
    public class AcuErrorHandler : IAcuErrorHandler
    {
        public List<ACUnit> ACUnits { get; set; }
        public List<ACError> ACErrors { get; set; }
        private IFileService _fileService;

        public AcuErrorHandler(IFileService fileService)
        {
            _fileService = fileService;
            ACUnits = _fileService.ReadACUsFromFile();
        }

        public string GetErrorDescriptionByCode(ACError error)
        {
            return "";
        }
    }
}
