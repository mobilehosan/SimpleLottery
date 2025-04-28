using Microsoft.Extensions.Options;
using SimpleLottery.Domain.Aggregates;
using SimpleLottery.Domain.Exceptions;
using SimpleLottery.Domain.Options;
using SimpleLottery.Domain.Services;

namespace SimpleLottery.Application;

public class LotteryRunner(
    IRandomizer randomizer, 
    IInputOutputService inputOutputService,
    IOptions<TiersOption> tiersOption,
    IOptions<LotteryOptions> lotteryOptions) : ILotteryRunner
{
    private readonly IRandomizer _randomizer = randomizer;
    private LotteryOptions? _lotteryOptions;

    private Lottery? _lottery;

    public void StartNewLottery() 
    {
        var tiers = tiersOption.Value;
        _lotteryOptions = lotteryOptions.Value;
        _lottery = new Lottery(tiers, _lotteryOptions, _randomizer);
    }

    public LotteryResult DoDraw(int userTicketsCount)
    {
        if (_lottery is null) 
        {
            var text = "New lottery is not started!";
            inputOutputService.PrintText(text);
            throw new LotteryIsNotStartedException(text);
        }
        return _lottery.Draw(userTicketsCount);
    }

    public LotteryOptions? GetLotteryOptions() => _lotteryOptions;
}
