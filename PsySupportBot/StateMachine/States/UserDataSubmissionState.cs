using PsySupportBot.Data;
using PsySupportBot.Extensions;
using PsySupportBot.Services;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;

namespace PsySupportBot.StateMachine.States;

public class UserDataSubmissionState : ChatStateBase
{
    private readonly UsersDataProvider _usersDataProvider;
    private readonly ITelegramBotClient _botClient;
    private readonly ChatId _managerChannelId;
    
    public UserDataSubmissionState(ChatStateMachine stateMachine, ITelegramBotClient botClient, 
        UsersDataProvider usersDataProvider, ChatId managerChannelId) : base(stateMachine)
    {
        _usersDataProvider = usersDataProvider;
        _botClient = botClient;
        _managerChannelId = managerChannelId;
    }

    public override Task HandleMessage(Message message)
    {
        return Task.CompletedTask;
    }
    
    private async Task SendUserInfoToManager(long chatId)
    {
        try
        {
            var userProfile = _usersDataProvider.GetUserData(chatId);
            var userInfo = BuildUserInfo(userProfile);
            
            await _botClient.SafeSendTextMessageAsync(_managerChannelId, userInfo);

            _usersDataProvider.ClearUserData(chatId);
        }
        catch (ApiRequestException exception)
        {
            await ErrorNotificationService.Instance.SendErrorNotification(exception);
        }

    }
    
    private string BuildUserInfo(UserData userProfile)
    {
        var userInfo = "Информация о клиенте:" +
                       $"\nФИО: {userProfile.Name}" +
                       $"\nНомер телефона: {userProfile.Phone}";
        
        if (!string.IsNullOrEmpty(userProfile.TelegramName))
        {
            userInfo += $"\nTelegram: @{userProfile.TelegramName}";
        }
        
        if (!string.IsNullOrEmpty(userProfile.LastQuestion))
        {
            userInfo += $"\nПоследний отправленный вопрос боту: {userProfile.LastQuestion}";
        }

        return userInfo;
    }
    
    public override async Task OnEnter(long chatId)
    {
        await base.OnEnter(chatId);

        var response = "Спасибо за обращение. Данные переданы нашему специалисту, он свяжется с Вами в скором времени.";
        await _botClient.SafeSendTextMessageAsync(chatId, response);
        await _stateMachine.TransitTo<IdleState>(chatId);
    }
    
    public override async Task OnExit(long chatId)
    {
        await base.OnExit(chatId);
        await SendUserInfoToManager(chatId);
    }
}