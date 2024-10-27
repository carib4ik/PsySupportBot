using System.Collections.Concurrent;
using PsySupportBot.Data;

namespace PsySupportBot.Services;

public class UsersDataProvider
{
    private readonly ConcurrentDictionary<long, UserData> _userDatas = new();

    public void SetUserName(long chatId, string name)
    {
        var userState = _userDatas.GetOrAdd(chatId, new UserData());
        userState.Name = name;
    }
    
    public void SetTelegramName(long chatId, string? telegramName)
    {
        var userState = _userDatas.GetOrAdd(chatId, new UserData());
        userState.TelegramName = telegramName;
    }
    
    public void SetUserPhone(long chatId, string? phone)
    {
        var userState = _userDatas.GetOrAdd(chatId, new UserData());
        userState.Phone = phone;
    }
    
    public void SaveLastQuestion(long chatId, string? question)
    {
        var data = _userDatas.GetOrAdd(chatId, new UserData());
        data.LastQuestion = question;
    }
    
    public UserData GetUserData(long chatId)
    {
        return _userDatas[chatId];
    }

    public void ClearUserData(long chatId)
    {
        _userDatas.TryRemove(chatId, out _);
    }
}