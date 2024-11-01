using PsySupportBot.Data;
using PsySupportBot.Extensions;
using PsySupportBot.Services;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace PsySupportBot.StateMachine.States;

public class QuestionState : ChatStateBase
{
    private readonly ITelegramBotClient _botClient;
    private readonly IChatGptService _chatGptService;
    private readonly UsersDataProvider _usersDataProvider;

    public QuestionState(ChatStateMachine chatStateMachine, ITelegramBotClient botClient,
        IChatGptService chatGptService, UsersDataProvider usersDataProvider) : base(chatStateMachine)
    {
        _botClient = botClient;
        _chatGptService = chatGptService;
        _usersDataProvider = usersDataProvider;
    }

    public override async Task HandleMessage(Message message)
    {
        var chatId = message.Chat.Id;
        var question = message.Text;
        
        _usersDataProvider.SaveLastQuestion(chatId, question);

        await _botClient.SafeSendChatActionAsync(chatId, ChatAction.Typing);

        var answer = await _chatGptService.GetAnswerFromChatGpt(question);
        // var answer = "await _chatGptService.GetAnswerFromChatGpt(question);";

        if (answer != null)
        {
            await _botClient.SafeSendTextMessageAsync(chatId, answer);
        }
        else
        {
            await HandleNullAnswer(chatId);
            return;
        }
        
        var replyMarkup = new InlineKeyboardMarkup(new[]
        {
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Задать вопрос", GlobalData.QUESTION)
            },
            new[]
            {   
                InlineKeyboardButton.WithCallbackData("Консультация специалиста", GlobalData.SPECIALIST)
            },
            new[]
            {
                InlineKeyboardButton.WithCallbackData("Я все узнал спасибо", GlobalData.DONE)
            }
        });

        var response = "Хотите задать еще вопрос?";
        await _botClient.SafeSendTextMessageAsync(chatId, response, replyMarkup: replyMarkup);
        await _stateMachine.TransitTo<IdleState>(chatId);
        
    }

    private async Task HandleNullAnswer(long chatId)
    {
        var response = "Пожалуйста, задайте Ваш вопрос снова.";
        
        var button1 = InlineKeyboardButton.WithCallbackData("Задать вопрос", GlobalData.QUESTION);
        var button2 = InlineKeyboardButton.WithCallbackData("Консультация специалиста", GlobalData.SPECIALIST);
        
        var keyboard = new InlineKeyboardMarkup(new[]
        {
            new[] { button1 },
            new[] { button2 }
        });
        
        await _botClient.SafeSendTextMessageAsync(chatId, response, replyMarkup: keyboard);
    }

    public override async Task OnEnter(long chatId)
    {
        await base.OnEnter(chatId);

        var response = "Задайте Ваш вопрос. На ответ мне может потребоваться около минуты.";
        await _botClient.SafeSendTextMessageAsync(chatId, response);
        
    }
}