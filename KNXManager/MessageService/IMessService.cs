using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KNXManager.MessageService
{
    public interface IMessService
    {
        public List<Mess> Messages { get; set; }
        public void AddInfoMessage(string message);
        public void AddWarningMessage(string message);
        public void AddDangerMessageAsync(string message);
        public event Action Notify;
    }
}
