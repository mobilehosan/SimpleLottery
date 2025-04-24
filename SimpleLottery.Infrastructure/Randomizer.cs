using SimpleLottery.Domain.Services;

namespace SimpleLottery.Infrastructure;

public class Randomizer : IRandomizer
{
    private readonly Random _random = new();
    public int GetRandomNumber(int from, int to)
    {
        return _random.Next(from, to);
    }
}
