namespace SimpleLottery.Domain.Services;

public interface IRandomizer
{
    int GetRandomNumber(int from, int to);
}
