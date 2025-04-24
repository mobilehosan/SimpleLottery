using SimpleLottery.Application;

namespace SimpleLottery.Infrastructure;

public class InputOutputService : IInputOutputService
{
    public void PrintText(string text)
    {
        Console.Write(text);
    }

    public string? GetTextFromUser(string text)
    {
        PrintText(text);
        return Console.ReadLine();
    }
}
