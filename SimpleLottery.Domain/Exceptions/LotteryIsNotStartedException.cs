namespace SimpleLottery.Domain.Exceptions;

public class LotteryIsNotStartedException : Exception
{
    public LotteryIsNotStartedException()
    {
    }

    public LotteryIsNotStartedException(string? message) : base(message)
    {
    }
}
