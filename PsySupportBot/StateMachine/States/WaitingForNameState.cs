using PsySupportBot.Extensions;
using PsySupportBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace PsySupportBot.StateMachine.States;

public class WaitingForNameState : ChatStateBase
{
    private readonly UsersDataProvider _usersDataProvider;
    private readonly ITelegramBotClient _botClient;
    
    
    public WaitingForNameState(ChatStateMachine stateMachine, ITelegramBotClient botClient, 
        UsersDataProvider usersDataProvider) : base(stateMachine)
    {
        _usersDataProvider = usersDataProvider;
        _botClient = botClient;
    }

    public override async Task HandleMessage(Message message)
    {
        var chatId = message.Chat.Id;
        
        _usersDataProvider.SetUserName(chatId, message.Text);
        _usersDataProvider.SetTelegramName(chatId, message.From?.Username);

        await _stateMachine.TransitTo<WaitingForPhoneState>(chatId);
    }

    public override async Task OnEnter(long chatId)
    {
        await base.OnEnter(chatId);

        var respose = "Пожалуйста, введите ваше ФИО.";
        await _botClient.SafeSendTextMessageAsync(chatId, respose);
    }
}