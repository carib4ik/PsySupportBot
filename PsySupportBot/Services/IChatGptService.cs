namespace PsySupportBot.Services;

public interface IChatGptService
{
    public Task<string?> GetAnswerFromChatGpt(string question);
}