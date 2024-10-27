using Newtonsoft.Json;
using PsySupportBot.AppSettings;
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
        var settings = JsonConvert.DeserializeObject<AppSettings.AppSettings>(settingsJson);
        
        var botClient = new TelegramBotClient(secrets.ApiKeys.TelegramKey);
        
        ErrorNotificationService.Initialize(botClient, settings.ErrorsLogChannelId, settings.ErrorsFilePath);
        
        var subscriptionService = new SubscriptionService(botClient, settings.SubscribeChannelId);
        IChatGptService chatGptService = new ChatGptService(secrets.ApiKeys.OpenAiKey, settings);
        var usersDataProvider = new UsersDataProvider();
        
        var chatStateMachine = new ChatStateMachine(botClient, settings, chatGptService, usersDataProvider);
        var chatStateController = new ChatStateController(chatStateMachine);
        
        var telegramBot = new TelegramBotController(botClient, subscriptionService, chatStateController);
        
        telegramBot.StartBot();
        await Task.Delay(Timeout.Infinite);

    }
}