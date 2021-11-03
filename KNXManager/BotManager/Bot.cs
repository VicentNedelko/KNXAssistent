using KNXManager.Globals;
using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace KNXManager.BotManager
{
    public class Bot : IBot
    {
        readonly TelegramBotClient botClient;
        public Bot()
        {
            botClient = new TelegramBotClient(Secret.Tbot);
            botClient.StartReceiving();
        }
        public async Task SendMessageAsync(string message)
        {
            _ = await botClient.SendTextMessageAsync(
                              chatId: Secret.GIRAChatId,
                              text: message,
                              parseMode: ParseMode.Markdown,
                              disableNotification: true);
        }
    }
}
