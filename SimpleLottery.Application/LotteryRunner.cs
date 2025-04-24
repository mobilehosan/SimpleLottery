using Microsoft.Extensions.Options;
using SimpleLottery.Domain.Aggregates;
using SimpleLottery.Domain.Options;
using SimpleLottery.Domain.Services;

namespace SimpleLottery.Application;

public class LotteryRunner(
    IRandomizer randomizer, 
    IInputOutputService outputService,
    IOptions<TiersOption> tiersOption,
    IOptions<LotteryOptions> lotteryOptions) : ILotteryRunner
{
    private readonly IRandomizer _randomizer = randomizer;
    private LotteryOptions? _lotteryOptions;
    private TiersOption? _tiers;

    private Lottery? _lottery;

    public void StartNewLottery() 
    {
        _tiers = tiersOption.Value;
        _lotteryOptions = lotteryOptions.Value;
        _lottery = new Lottery(_tiers, _lotteryOptions, _randomizer);
    }

    public LotteryResult DoDraw(int userTicketsCount)
    {
        if (_lottery is null) 
        {
            var text = "New lottery is not started!";
            outputService.PrintText(text);
            throw new NullReferenceException(text);
        }
        return _lottery.Draw(userTicketsCount);
    }

    public LotteryOptions? GetLotteryOptions() => _lotteryOptions;
}
