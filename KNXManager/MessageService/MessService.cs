using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KNXManager.MessageService
{
    public class MessService : IMessService
    {
        public List<Mess> Messages { get; set; }
        public MessService()
        {
            Messages = new();
        }

        public event Action Notify;

        public void AddDangerMessage(string message)
        {
            Messages.Add(new Mess
            {
                TimeStamp = DateTime.Now,
                Information = message,
                StyleType = "alert alert-danger",
            });
            Notify?.Invoke();
        }

        public void AddInfoMessage(string message)
        {
            Messages.Add(new Mess
            {
                TimeStamp = DateTime.Now,
                Information = message,
                StyleType = "alert alert-info",
            });
            Notify?.Invoke();
        }

        public void AddWarningMessage(string message)
        {
            Messages.Add(new Mess
            {
                TimeStamp = DateTime.Now,
                Information = message,
                StyleType = "alert alert-warning",
            });
            Notify?.Invoke();
        }

    }
}
