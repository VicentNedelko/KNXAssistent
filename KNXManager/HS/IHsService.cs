using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KNXManager.HS
{
    public interface IHsService
    {
        public string HostIp { get; set; }
        public string Login { get; set; }
        public string Pass { get; set; }
        public string Error { get; set; }
        public Task<string> GetLoginActionsAsync();
        public string[] GetParsedLoginActions(string logData);
        public LogInfo[] GetLogValues(string[] parsedLogins);
    }
}
