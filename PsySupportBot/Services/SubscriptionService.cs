using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace PsySupportBot.Services;

public class SubscriptionService
{
    public ChatId SubscribeChatId { get; }
    private ITelegramBotClient _botClient;
    
    private readonly List<ChatMemberStatus> _allowedStatuses = [ChatMemberStatus.Member, 
        ChatMemberStatus.Administrator, ChatMemberStatus.Creator];

    public SubscriptionService(ITelegramBotClient telegramBotClient, ChatId subscribeChatId)
    {
        _botClient = telegramBotClient;
        SubscribeChatId = subscribeChatId;
    }

    public async Task<bool> IsSubscribed(long userId)
    {
        try
        {
            var user = await _botClient.GetChatMemberAsync(SubscribeChatId, userId);

            if (_allowedStatuses.Contains(user.Status))
            {
                return true;
            }
            
        }
        catch(ApiRequestException exception)
        {
            Console.WriteLine(exception);
            return false;
        }

        return false;
    }
}