using SimpleLottery.Domain.Aggregates;
using SimpleLottery.Domain.Options;

namespace SimpleLottery.Application;

public interface ILotteryRunner
{
    void StartNewLottery();

    LotteryResult DoDraw(int userTicketsCount);

    LotteryOptions? GetLotteryOptions();
}