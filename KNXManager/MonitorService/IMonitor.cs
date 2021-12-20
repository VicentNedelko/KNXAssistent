using DAL.Models;
using Knx.Bus.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KNXManager.MonitorService
{
    public interface IMonitor
    {
        public List<GaValue> gaValues { get; set; }
        public List<GA> gaSbcList { get; set; }
        public event Func<Task> OnGaReceived;
        public void Bus_GroupValueSbcReceived(GroupValueEventArgs obj);
        public void StopMonitor();
        public void StartMonitor();
    }
}
