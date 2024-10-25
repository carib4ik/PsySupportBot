using Telegram.Bot.Types;

namespace PsySupportBot;

public class AppSettings
{
    public ChatId ManagerChannelId { get; init; }
    public ChatId SubscribeChannelId { get; init; }
    public string GptPrompt { get; init; }
    public string AgencyName { get; init; }
}