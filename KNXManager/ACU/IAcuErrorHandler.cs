using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DAL.Models;
using Knx.Bus.Common;

namespace KNXManager.ACU
{
    public interface IAcuErrorHandler
    {
        public List<ACUnit> ACUnits { get; set; }
        public List<ACError> ACErrors { get; set; }
        public List<ErrorValue> ErrorValues { get; set; }
        public void Bus_OnACUerrorValueReceived(GroupValueEventArgs obj);
        public event Func<Task> OnErrorReceived;

        public void StartMonitor();
        public void StopMonitor();
    }
}
