using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Extensions.Polling;

namespace KNXManager.BotManager
{
    public interface IBot
    {
        public Task SendMessageAsync(string message);
    }
}
