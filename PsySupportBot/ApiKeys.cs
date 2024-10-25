namespace PsySupportBot;

public class Secrets
{
    public ApiKeysSettings ApiKeys { get; init; }
}

public class ApiKeysSettings
{
    public string TelegramKey { get; init; }
    public string OpenAiKey { get; init; }
}