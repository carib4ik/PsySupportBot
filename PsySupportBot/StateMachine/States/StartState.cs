using PsySupportBot.Data;
using PsySupportBot.Extensions;
using PsySupportBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace PsySupportBot.StateMachine.States;

public class StartState : ChatStateBase
{
    private readonly ITelegramBotClient _botClient;
    private readonly string _agencyName;
    
    public StartState(ChatStateMachine stateMachine, ITelegramBotClient botClient, string agencyName,
        IChatGptService chatGptService) : base(stateMachine)
    {
        _botClient = botClient;
        _agencyName = agencyName;
    }

    public override Task HandleMessage(Message message)
    {
        return Task.CompletedTask;
    }
    
    public override async Task OnEnter(long chatId)
    {
        await base.OnEnter(chatId);
        await SendGreeting(chatId);
    }

    private async Task SendGreeting(long chatId)
    {
        var greetings = $"Приветствую!\nЯ первый психологический бот с искустренным интелектом, созданный агентством психологической поддержки *{_agencyName}*." +
                        "\nЯ могу оказать Вам мгновенную первичную консультацию по любому интересующему Вас вопросу," +
                        " либо передать Ваши данные нашему специалисту для более детальной консультации.";
        
        var questionButton = InlineKeyboardButton.WithCallbackData("Задать вопрос", GlobalData.QUESTION);
        var specialistButton = InlineKeyboardButton.WithCallbackData("Связаться со специалистом", GlobalData.SPECIALIST);
        
        var keyboard = new InlineKeyboardMarkup(new[]
        {
            new[] { questionButton, specialistButton }
        });
        
        await _botClient.SafeSendTextMessageAsync(chatId, greetings.EscapeMarkdownV2(), replyMarkup: keyboard, parseMode: ParseMode.MarkdownV2);
        await _stateMachine.TransitTo<IdleState>(chatId);
    }
}