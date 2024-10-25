using Newtonsoft.Json;
using PsySupportBot.Services;
using PsySupportBot.StateMachine;
using Telegram.Bot;

namespace PsySupportBot;

public static class Program
{
    public static async Task Main(string[] args)
    {
        var secretsJson = await File.ReadAllTextAsync("AppSettings/secrets.json");
        var secrets = JsonConvert.DeserializeObject<Secrets>(secretsJson);

        var settingsJson = await File.ReadAllTextAsync("AppSettings/app_settings.json");
        var settings = JsonConvert.DeserializeObject<AppSettings>(settingsJson);
        
        var botClient = new TelegramBotClient(secrets.ApiKeys.TelegramKey);
        var subscriptionService = new SubscriptionService(botClient, settings.SubscribeChannelId);
        
        var chatStateMachine = new ChatStateMachine(botClient, settings);
        var chatStateController = new ChatStateController(chatStateMachine);
        
        var telegramBot = new TelegramBotController(botClient, subscriptionService, chatStateController);
        
        telegramBot.StartBot();
        await Task.Delay(Timeout.Infinite);

    }
}