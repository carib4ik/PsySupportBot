using PsySupportBot.Data;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;
using PsySupportBot.Extensions;

namespace PsySupportBot.StateMachine.States;

public class DoneState : ChatStateBase
{
    private readonly ITelegramBotClient _botClient;

    public DoneState(ChatStateMachine stateMachine, ITelegramBotClient botClient) : base(stateMachine)
    {
        _botClient = botClient;
    }

    public override Task HandleMessage(Message message)
    {
        return Task.CompletedTask;
    }

    public override async Task OnEnter(long chatId)
    {
        await base.OnEnter(chatId);
        
        var response = "Thank you for using our services.\n" +  
                       "I can provide you with an instant initial consultation on any question of interest to you, " +  
                       "or forward your information to our specialist for a more detailed consultation.";
        
        var keyboard = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Ask a question", GlobalData.QUESTION)
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Specialist consultation", GlobalData.SPECIALIST)
            },

        });
        
        await _botClient.SafeSendTextMessageAsync(chatId, response, replyMarkup: keyboard);
        await _stateMachine.TransitTo<IdleState>(chatId);
    }
}