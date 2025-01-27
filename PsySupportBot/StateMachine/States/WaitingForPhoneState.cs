using System.Text.RegularExpressions;
using PsySupportBot.Extensions;
using PsySupportBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace PsySupportBot.StateMachine.States;

public class WaitingForPhoneState : ChatStateBase
{
    private readonly UsersDataProvider _usersDataProvider;
    private readonly ITelegramBotClient _botClient;
    
    
    public WaitingForPhoneState(ChatStateMachine stateMachine, ITelegramBotClient botClient, 
        UsersDataProvider usersDataProvider) : base(stateMachine)
    {
        _usersDataProvider = usersDataProvider;
        _botClient = botClient;
    }
    public override async Task HandleMessage(Message message)
    {
        var chatId = message.Chat.Id;
        
        if (!Regex.IsMatch(message.Text, @"^[+\-()\d\s]*$"))
        {
            var response = "The phone number is entered incorrectly.\nPlease enter the number without letters";
            await _botClient.SafeSendTextMessageAsync(chatId, response);
            return;
        }

        _usersDataProvider.SetUserPhone(chatId, message.Text);
        
        await _stateMachine.TransitTo<UserDataSubmissionState>(chatId);
    }
    
    public override async Task OnEnter(long chatId)
    {
        await base.OnEnter(chatId);

        var response = "Please enter your phone number.";
        await _botClient.SafeSendTextMessageAsync(chatId, response);
    }
}