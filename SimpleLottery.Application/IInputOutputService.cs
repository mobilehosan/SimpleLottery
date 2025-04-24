namespace SimpleLottery.Application;

public interface IInputOutputService
{
    void PrintText(string text);
    string? GetTextFromUser(string text);
}
